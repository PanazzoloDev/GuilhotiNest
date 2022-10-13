﻿using Inventor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace GuilhotiNest.ViewModel
{
    class InventorApp
    {

        internal ApprenticeServerComponent AppServer;
        //Construtor
        public InventorApp()
        {
            AppServer = new ApprenticeServerComponent();
        }
        //Métodos públicos
        public void Importar_iam(string caminho, ref List<Document> Documentos)
        {
            List<Document> Itens = new List<Document>();
            ApprenticeServerDocument aDoc = AppServer.Open(caminho);

            foreach (ApprenticeServerDocument doc in aDoc.ReferencedDocuments)
            {
                if (doc.DocumentType != DocumentTypeEnum.kPartDocumentObject) continue;
                if (doc.PropertySets[3][17].Value == "Sheet Metal")
                {
                    Document Item = ExtrairDados(doc);
                    if(Item != null) Documentos.Add(Item);
                }
            }
        }
        public Document Importar_ipt(string caminho)
        {
            ApprenticeServerDocument Doc = AppServer.Open(caminho);
            if (Doc.DocumentType != DocumentTypeEnum.kPartDocumentObject)
            {
                return null;
            }
            if (Doc.PropertySets[3][17].Value != "Sheet Metal")
            {
                return null;
            }
            return ExtrairDados(Doc);
        }
        //Métodos Internos á classe
        private Document ExtrairDados(ApprenticeServerDocument pDoc)
        {
            SheetMetalComponentDefinition sm_def = pDoc.ComponentDefinition as SheetMetalComponentDefinition;

            if (sm_def.HasFlatPattern == false)
            {
                return null;
            }

            Face fc = sm_def.FlatPattern.BottomFace;
            Point min = fc.Evaluator.RangeBox.MinPoint;
            Point max = fc.Evaluator.RangeBox.MaxPoint;

            string comando = string.Empty;
            List<param> parametros;

            foreach (EdgeLoop loop in fc.EdgeLoops)
            {
                bool inicio = true;
                parametros = new List<param>();
                for (int i = 1; i <= loop.Edges.Count; i++)
                {
                    param par = new param();
                    par.indice = i;
                    par.p1 = loop.Edges[i].StartVertex.Point;
                    par.p2 = loop.Edges[i].StopVertex.Point;
                    par.externo = loop.IsOuterEdgeLoop;
                    parametros.Add(par);
                }
                Point pt = loop.Edges[1].StartVertex.Point;
                for (int i = 0; i <= parametros.Count - 1; i++)
                { 
                    param par = parametros[i];
                    if (par.p1.IsEqualTo(pt) && pt != par.p1)
                    {
                        comando = comando + Classificador(inicio,loop.Edges[par.indice], pt, min, par.p2, par.externo);
                        pt = par.p2; parametros.RemoveAt(i); i = -1;
                        inicio = false;
                    }
                    else if (par.p2.IsEqualTo(pt))
                    {
                        comando = comando + Classificador(inicio,loop.Edges[par.indice], pt, min, par.p1, par.externo);
                        pt = par.p1; parametros.RemoveAt(i); i = -1;
                        inicio = false;
                    }
                }
                comando = comando + " z ";
            }
            comando = comando.Replace(',', '.');
            Document doc = new Document(pDoc.DisplayName.ToString(), pDoc.PropertySets[3][10].Value, comando,2.65, Math.Round((max.X - min.X) * 10, 2), Math.Round((max.Y - min.Y) * 10, 2), Math.Round(((max.Y - min.Y) * (max.X - min.X) * 10), 2),0,1);
            return doc;
        }
        private string Conversor(Point inv, Point min)
        {
            double x, y;
            string cmd = string.Empty;
            if (min.X >= 0)
            {
                x = Math.Round((inv.X - Math.Abs(min.X)) * 10, 3);
            }
            else
            {
                x = Math.Round((inv.X + Math.Abs(min.X)) * 10, 3);
            }
            if (min.Y >= 0)
            {
                y = Math.Round((inv.Y - Math.Abs(min.Y)) * 10, 3);
            }
            else
            {
                y = Math.Round((inv.Y + Math.Abs(min.Y)) * 10, 3);
            }
            cmd = x + " " + y + " ";
            return cmd;
            //return new System.Windows.Point(x,y);
        }
        private string Classificador(bool inicial, Edge linha, Point pt_inicio, Point min_inv, Point pt_fim, bool loopinterno)
        {
            string cmd = string.Empty;
            if (inicial)
            {
                if (linha.GeometryType == CurveTypeEnum.kLineSegmentCurve)
                {
                    cmd = string.Format(" M {0} L {1}", Conversor(pt_inicio, min_inv), Conversor(pt_fim, min_inv));
                }
                else if (linha.GeometryType == CurveTypeEnum.kCircularArcCurve)
                {
                    double raio = Math.Round((linha.Geometry.Radius * 10), 2);
                    int obl = 0, grande = 0;
                    if (Esquerda(linha)) obl = 1;
                    if (Math.Round((linha.Geometry.SweepAngle * 180) / Math.PI, 2) > 180) grande = 1;
                    cmd = string.Format(" M {0} A {1} {1} 0 {4} {2} {3}", Conversor(pt_inicio, min_inv), raio, obl, Conversor(pt_fim, min_inv),grande);
                }
                else
                {
                    double raio = Math.Round((linha.Geometry.Radius), 2);
                    double centro_x = Math.Round((linha.Geometry.Center.X * 10), 2);
                    double centro_y = Math.Round((linha.Geometry.Center.Y * 10), 2);

                    Point PT0 = AppServer.TransientGeometry.CreatePoint(linha.Geometry.Center.X - raio, linha.Geometry.Center.Y - 0.001, linha.Geometry.Center.Z);
                    Point PT1 = AppServer.TransientGeometry.CreatePoint(linha.Geometry.Center.X - raio, linha.Geometry.Center.Y, linha.Geometry.Center.Z);
                    cmd = string.Format(" M {0} A {1} {1} 0 1 0 {2}", Conversor(PT1, min_inv), raio*10, Conversor(PT0, min_inv));
                }
            }
            else
            {
                if (linha.GeometryType == CurveTypeEnum.kLineSegmentCurve)
                {
                    cmd = string.Format(" L {0}", Conversor(pt_fim, min_inv));
                }
                else if (linha.GeometryType == CurveTypeEnum.kCircularArcCurve)
                {
                    double raio = Math.Round((linha.Geometry.Radius * 10), 2);
                    int obl = 0, grande = 0;
                    if (Esquerda(linha)) obl = 1;
                    if (Math.Round((linha.Geometry.SweepAngle * 180) / Math.PI, 2) > 180) grande = 1;
                    cmd = string.Format(" A {0} {0} 0 {3} {1} {2}",raio, obl, Conversor(pt_fim, min_inv),grande);
                }
                else
                {

                }
            }

            return cmd;
        }
        private bool Esquerda(Edge linha)
        {
            double X = (linha.StartVertex.Point.X - linha.StopVertex.Point.X);
            double Y = (linha.StartVertex.Point.Y - linha.StopVertex.Point.Y);
            double Z = (linha.StartVertex.Point.Z - linha.StopVertex.Point.Z);

            System.Windows.Point geocentro;
            geocentro = new System.Windows.Point(linha.Evaluator.RangeBox.MinPoint.X + ((linha.Evaluator.RangeBox.MaxPoint.X - linha.Evaluator.RangeBox.MinPoint.X) / 2), linha.Evaluator.RangeBox.MinPoint.Y + ((linha.Evaluator.RangeBox.MaxPoint.Y - linha.Evaluator.RangeBox.MinPoint.Y) / 2));


            if (Y > 0)
            {

                if (geocentro.X < linha.Geometry.Center.X)
                {
                    return true;
                }
                else
                {
                    return false;
                }     
            }
            else if (Y < 0)
            {
                if (geocentro.X < linha.Geometry.Center.X)
                {
                    return false;
                    //MsgBox "Descendo pra direita: Direita"
                }
                else
                {
                    return true;
                    //MsgBox "Descendo pra direita: Esquerda"
                }
            }
            else
            {
                if (X > 0)
                {
                    if (geocentro.Y < linha.Geometry.Center.Y)
                    {
                        return true;
                        //MsgBox "Descendo pra direita: Direita"
                    }
                    else
                    {
                        return false;
                        //MsgBox "Descendo pra direita: Esquerda"
                    }
                }
                else if (X < 0)
                {
                    if (geocentro.Y < linha.Geometry.Center.Y)
                    {
                        return true;
                        //MsgBox "Descendo pra direita: Direita"
                    }
                    else
                    {
                        return false;
                        //MsgBox "Descendo pra direita: Esquerda"
                    }
                }
                else
                {
                    return false;
                }
            }

        }
        //Classe Interna
        internal class param
        {
            internal int indice;
            internal Point p1;
            internal Point p2;
            internal bool externo;
        }
    }
}