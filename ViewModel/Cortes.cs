using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GuilhotiNest.ViewModel
{
    public class Cortes
    {
        public enum Orientacao{Vertical,Horizontal}

        public Layout Parent { get; set; }
        public Line Design { get; set; }
        public Orientacao Orientação { get; set; }
        public double Medida { get; set; }

        public Cortes(double med, Orientacao orient, Layout pai)
        {
            Parent = pai;
            Medida = med;
            Orientação = orient;
            CriarDesign(med);
        }
        public void CriarDesign(double med)
        {
            if(Orientação == Orientacao.Horizontal)
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
    }
}
