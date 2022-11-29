using Inventor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;

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
                var qtde = aDoc.ComponentDefinition.Occurrences.AllReferencedOccurrences[doc];
                if (doc.DocumentType != DocumentTypeEnum.kPartDocumentObject) continue;
                if (doc.PropertySets[3][17].Value == "Sheet Metal")
                {
                    Document Item = ExtrairDados(doc);
                    if(Item != null) {
                        Documentos.Add(Item);
                        Item.Quantidade = qtde.Count;
                    }

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
            Face fc;Point min; Point max;
            if (sm_def.HasFlatPattern == false)
            {
                return null;
            }
            try
            {
                 fc = sm_def.FlatPattern.BottomFace;
                 min = fc.Evaluator.RangeBox.MinPoint;
                 max = fc.Evaluator.RangeBox.MaxPoint;
            }
            catch
            {return null;}

            int of = 0;
            try{of = pDoc.PropertySets[4]["os"].Value.ToString();}catch{}

            PathGeometry geo = new PathGeometry();
            foreach (EdgeLoop loop in fc.EdgeLoops)
            {
                List<param> Loop_Ordenado = Ordenar(loop,min);
                PathFigure contorno = new PathFigure() ;
                contorno.StartPoint = Loop_Ordenado[0].Ponto;
                //contorno.IsClosed = true;
                for( int i = 1; i <= Loop_Ordenado.Count-1;i=i+1)
                {
                    param par  = Loop_Ordenado[i];
                    if (par.Linha.GeometryType == CurveTypeEnum.kCircleCurve)
                    {
                        System.Windows.Point centro = Conversor(par.Linha.Geometry.Center, min);
                        geo.AddGeometry(new EllipseGeometry(centro, par.Linha.Geometry.Radius *10, par.Linha.Geometry.Radius*10));
                    }
                    else if (par.Linha.GeometryType == CurveTypeEnum.kLineSegmentCurve)
                    {
                        contorno.Segments.Add(new System.Windows.Media.LineSegment(par.Ponto,true));            
                    }
                    else if (par.Linha.GeometryType == CurveTypeEnum.kCircularArcCurve)
                    {
                        SweepDirection dir;
                        if (par.Linha.Geometry.Normal.Z < 0)
                        {dir = SweepDirection.Clockwise;}else{dir = SweepDirection.Counterclockwise;}

                        bool largearc = par.Linha.Geometry.SweepAngle > Math.PI ? true : false;
                        System.Windows.Size size = new System.Windows.Size(par.Linha.Geometry.Radius * 10, par.Linha.Geometry.Radius * 10);
                        
                        contorno.Segments.Add(new ArcSegment(par.Ponto, size, 0, largearc, dir, true));
                    }                                    
                    //Console.WriteLine(par.Linha.TransientKey + " - x= ;" + Math.Round(par.Ponto.X, 2) + "; y= ;" + Math.Round(par.Ponto.Y, 2));
                }
                geo.Figures.Add(contorno);
            }
           
            Document doc = new Document(pDoc.DisplayName.ToString(), pDoc.PropertySets[3][10].Value, geo, 2.65, Math.Round((max.X - min.X) * 10, 2), Math.Round((max.Y - min.Y) * 10, 2), Math.Round(((max.Y - min.Y) * (max.X - min.X) * 10), 2),of,1);
            return doc;
        }
        private System.Windows.Point Conversor(Point inv, Point min)
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
            return new System.Windows.Point(x,y);
            //return new System.Windows.Point(x,y);
        }
        internal class param
        {
            internal Edge Linha;
            internal System.Windows.Point Ponto;
        }
        private List<param> Ordenar(EdgeLoop loop,Point min)
        {
            List<param> list = new List<param>();
            denovo:
            foreach (Edge linha in loop.Edges)
            {
                System.Windows.Point pt1 = Conversor(linha.StartVertex.Point, min);
                System.Windows.Point pt2 = Conversor(linha.StopVertex.Point, min);
                if (list.Count > 0 && list.Last().Ponto.Equals(pt1))
                {
                    list.Add(new param() { Linha = linha, Ponto = Conversor(linha.StopVertex.Point, min) });
                }
                else if(list.Count > 0 && list.Last().Ponto.Equals(pt2))
                {
                    list.Add(new param() { Linha = linha, Ponto = Conversor(linha.StartVertex.Point, min) });
                }else if(list.Count == 0)
                {
                    list.Add(new param() { Linha = linha, Ponto = Conversor(linha.StartVertex.Point, min) });
                }
                if(list.Count == loop.Edges.Count)
                {
                    list.Add(new param() { Linha = list[0].Linha, Ponto = Conversor(list[0].Linha.StartVertex.Point, min) });
                }            
            }
            if (list.Count < loop.Edges.Count)
            {
                goto denovo;
            }
            return list;
        }
    }
}
