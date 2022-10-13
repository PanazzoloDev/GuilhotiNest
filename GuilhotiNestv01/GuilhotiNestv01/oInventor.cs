﻿using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GuilhotiNestv01
{
    class oInventor
    {
        oItem it;
        Inventor.Application ThisApplication;
        
        public oInventor()
        {
            try
            {
                ThisApplication = Marshal.GetActiveObject("Inventor.Application") as Inventor.Application;
            }
            catch
            {
                Type tp = Type.GetTypeFromProgID("Inventor.Application");
                ThisApplication = System.Activator.CreateInstance(tp) as Inventor.Application;
                Thread.Sleep(5000);
            }

        }
        public oItem Read_Item(string caminho)
        {
            string comando = string.Empty;
            List<param> parametros;
            PartDocument pDoc = ThisApplication.Documents.Open(caminho, true) as PartDocument;
            SheetMetalComponentDefinition sm_def = pDoc.ComponentDefinition as SheetMetalComponentDefinition;
            if(sm_def.HasFlatPattern == false)
            {
                sm_def.Unfold();
                sm_def.FlatPattern.ExitEdit();
            }
            
            Face fc = sm_def.FlatPattern.BottomFace;
            Point min = fc.Evaluator.RangeBox.MinPoint;
            Point max = fc.Evaluator.RangeBox.MaxPoint;

            foreach (EdgeLoop loop in fc.EdgeLoops)//para cada contorno na face principal
            {
                parametros = new List<param>();
                for (int i = 2; i <= loop.Edges.Count; i++)//para cada linha no contorno
                {
                    param par = new param();
                    par.indice = i;
                    par.p1 = loop.Edges[i].StartVertex.Point;
                    par.p2 = loop.Edges[i].StopVertex.Point;
                    parametros.Add(par);
           
                }
                Point pt = loop.Edges[1].StartVertex.Point;
                comando = comando + " M " + Conversor(pt, min);
                for (int i = 0; i <= parametros.Count-1; i ++)
                {
                    param par = parametros[i];
                    if (par.p1.IsEqualTo(pt) && pt != par.p1)
                    {
                        //comando = comando + " L " + Math.Round(par.p2.X * 10, 2) + " " + Math.Round(par.p2.Y * 10, 2);
                        comando = comando + Classificador(loop.Edges[par.indice]) + Conversor(par.p2, min);
                        pt = par.p2; parametros.RemoveAt(i); i = -1;
                    }else if (par.p2.IsEqualTo(pt))
                    {
                        //comando = comando + " L " + Math.Round(par.p1.X * 10, 2) + " " + Math.Round(par.p1.Y * 10, 2);
                        comando = comando + Classificador(loop.Edges[par.indice]) + Conversor(par.p1, min);
                        pt = par.p1; parametros.RemoveAt(i); i = -1; 
                    }
                }
                comando = comando + " z ";
            }
            
            comando = comando.Replace(',', '.');
            //it = new oItem(comando,int.Parse(pDoc.DisplayName.Replace(".ipt",string.Empty)),5,5.5,5.5,500);
            it = new oItem(comando, pDoc.DisplayName.ToString(),5,(max.X - min.X)*10, (max.Y - min.Y) * 10, ((max.Y - min.Y) * (max.X - min.X)*10));

            return it;
        }
        public string Conversor(Point inv, Point min)
        {
            double x, y;
            string cmd = string.Empty;
            if (min.X >= 0)
            {
                x = Math.Round((inv.X - Math.Abs(min.X)) * 10, 2);
            }
            else
            {
                x = Math.Round((inv.X + Math.Abs(min.X)) * 10, 2);
            }
            if (min.Y >= 0)
            {
                y = Math.Round((inv.Y - Math.Abs(min.Y)) * 10, 2);
            }
            else
            {
                y = Math.Round((inv.Y + Math.Abs(min.Y)) * 10, 2);
            }
            cmd = x + " " + y + " ";
            return cmd;
        }
        public string Classificador(Edge linha)
        {
            string cmd = string.Empty;
            if (linha.GeometryType == CurveTypeEnum.kLineSegmentCurve)
            {
                cmd = " L ";
            }
            else if (linha.GeometryType == CurveTypeEnum.kCircularArcCurve)
            {
                double dim_x = Math.Round((linha.Evaluator.RangeBox.MaxPoint.X - linha.Evaluator.RangeBox.MinPoint.X)*10,2);
                double dim_y = Math.Round((linha.Evaluator.RangeBox.MaxPoint.Y - linha.Evaluator.RangeBox.MinPoint.Y) * 10, 2);
                double Angulo = Math.Round((linha.Geometry.SweepAngle * 180) / Math.PI,2);

                if(linha.Geometry.Center.Y > (linha.Evaluator.RangeBox.MinPoint.Y + (dim_y / 2)))
                {
                    cmd = " A " + dim_x + " " + dim_y + " " + Angulo + " 0 1 ";
                }
                else
                {
                    cmd = " A " + dim_x + " " + dim_y + " " + Angulo + " 0 0 ";
                }


                
            }
            else
            {

            }
            return cmd;
        }
    }
    public class param
    {
        public int indice;
        public Point p1;
        public Point p2;
    }
}
