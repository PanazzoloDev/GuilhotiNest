using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GuilhotiNest.ViewModel
{
    [Serializable()]
    public class Cortes
    {
        public enum Orientacao{Vertical,Horizontal}

        public Layout Parent { get; set; }
        [field:NonSerialized()]
        public Line Design;// { get; set; }
        public Orientacao Orientação { get; set; }
        public double Medida { get; set; }

        public Cortes(double med, Orientacao orient, Layout pai)
        {
            Parent = pai;
            Medida = med;
            Orientação = orient;
            CriarDesign(med);
        }
        public Cortes(double med, Orientacao orient, Layout pai, double[] Range)
        {
            Parent = pai;
            Medida = med;
            Orientação = orient;
            CriarDesign(med, Range);
        }
        public void CriarDesign(double med, double[] Range = null)
        {
            if(Range == null)
            {
                if (Orientação == Orientacao.Horizontal)
                {
                    Design = new Line()
                    {
                        Stroke = Brushes.Red,
                        StrokeThickness = 1,
                        X1 = Parent.Design.Data.Bounds.Left,
                        X2 = Parent.Design.Data.Bounds.Right,
                        Y1 = med,
                        Y2 = med
                    };
                }
                else
                {
                    Design = new Line()
                    {
                        Stroke = Brushes.Red,
                        StrokeThickness = 1,
                        X1 = med,
                        X2 = med,
                        Y1 = Parent.Design.Data.Bounds.Top,
                        Y2 = Parent.Design.Data.Bounds.Bottom
                    };
                }
            }
            else
            {
                if (Orientação == Orientacao.Horizontal)
                {
                    Design = new Line()
                    {
                        Stroke = Brushes.Red,
                        StrokeThickness = 1,
                        X1 = Range[0],
                        X2 = Range[1],
                        Y1 = med,
                        Y2 = med
                    };
                }
                else
                {
                    Design = new Line()
                    {
                        Stroke = Brushes.Red,
                        StrokeThickness = 1,
                        X1 = med,
                        X2 = med,
                        Y1 = Range[0],
                        Y2 = Range[1]
                    };
                }
            }
        }
    }
}
