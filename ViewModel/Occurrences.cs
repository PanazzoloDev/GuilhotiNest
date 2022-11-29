using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GuilhotiNest.ViewModel
{
    public class Occurrences
    {
        public bool Arranjado { get; set; } = false;
        public double Altura { get; set; }
        public double Comprimento { get; set; }
        public double Escala { get; set; } = 1;

        public Geometry Data { get; set; }
        public Document Parent { get; set; }
        public Path Design { get; set; }
        public Layout layout { get; set; }
        public System.Windows.Rect Limites { get; set; }

        public List<Cortes> Cortes {get;set;}
        private TransformGroup _Trans_Group;
        private ScaleTransform _Scale;
        private TranslateTransform _Translate;
        private RotateTransform _Rotate;
        //Construtor
        public Occurrences(Document Pai, Layout layout,double[] Dim_Layout)
        {
            Cortes = new List<ViewModel.Cortes>();
            this.Parent = Pai;
            this.layout = layout;
            this.Arranjado = true;
            this.Data = Geometry.Parse(Pai.Geometria).Clone();
            Criar_Design();
            Aplicar_Transformacoes(Dim_Layout);

            this.Altura = this.Data.Bounds.Height;
            this.Comprimento = this.Data.Bounds.Width;
            layout.Occs.Add(this);
            this.Design.Tag = this;
        }
        //Métodos publicos
        public void Mover_Colisao(System.Windows.Point pt, Geometry layout)
        {
            double x_antigo = _Translate.X;
            double y_antigo = _Translate.Y;

            _Translate.X = pt.X - (this.Comprimento / 2);
            if (Interno(layout) == true)
            {
                _Translate.X = x_antigo;
            }

            _Translate.Y = pt.Y - (this.Altura / 2);
            if (Interno(layout) == true)
            {
                _Translate.Y = y_antigo;
            }
        }
        public void Mover(System.Windows.Point mouse)
        {
            _Translate.X = mouse.X - (this.Comprimento / 2);
            _Translate.Y = mouse.Y - (this.Altura / 2);
        }
        public void Rotate(int angulo)
        {
            _Rotate.CenterX = (this.Comprimento / 2);
            _Rotate.CenterY = (this.Altura / 2);
            _Rotate.Angle = _Rotate.Angle + angulo;
        }

        private void Criar_Design()
        {
            Design = new Path()
            {
                Fill = Brushes.SteelBlue,
                Stroke = Brushes.MidnightBlue,
                StrokeThickness = 1,
                Data = this.Data,
                Cursor = System.Windows.Input.Cursors.Hand
            };
        }
        private void Aplicar_Transformacoes(double[] dim_layout)
        {
            this.Escala = (new[] { ((dim_layout[0] * .9) / 3000), ((dim_layout[1] * 0.8) / 1200) }).Min();
            double offset_y = (dim_layout[1] * 0.1);
            double offset_x = (dim_layout[0] * 0.05);

            this._Trans_Group = new TransformGroup();
            _Scale = new ScaleTransform(this.Escala, this.Escala);
            _Translate = new TranslateTransform((layout.Comprimento/2)*layout.Escala, (layout.Altura / 2) * layout.Escala);
            _Rotate = new RotateTransform(0, (this.Comprimento / 2), (this.Altura / 2));

            _Trans_Group.Children.Add(_Scale);
            _Trans_Group.Children.Add(_Rotate);
            _Trans_Group.Children.Add(_Translate);

            Design.Data.Transform = _Trans_Group;
        }
        private System.Windows.Point FindEquivalente(System.Windows.Point pt, List<Occurrences> Arranjados)
        {
            
            List<double[]> Xs = new List<double[]>();
            List<double[]> Ys = new List<double[]>();

            for (int i = 0; i <  Arranjados.Count; i ++)
            {
                if (Arranjados[i] == this) continue;
                Ys.Add(new double[2] { Arranjados[i].Limites.Top , Math.Abs(pt.Y - Arranjados[i].Limites.Top)});
                Xs.Add(new double[2] { Arranjados[i].Limites.Left, Math.Abs(pt.X - Arranjados[i].Limites.Left)});
            }
            double x, y;
            try
            {
                x = Xs.Min(z => z[0]);
                y = Ys.Min(z => z[0]);
            }
            catch
            {
                x = 0;
                y = 0;
                return new System.Windows.Point(x, y);
            }
            if(Math.Abs(pt.X-x) <= Math.Abs(pt.Y - y) && Math.Abs(pt.X - x) < 40)
            {
                return new System.Windows.Point(x, pt.Y);
            }else if (Math.Abs(pt.X - x) > Math.Abs(pt.Y - y) && Math.Abs(pt.Y - y) < 40)
            {
                return new System.Windows.Point(pt.X, y);
            }
            else
            {
                x = 0;
                y = 0;
                return new System.Windows.Point(x, y);
            }
            
        }
        private bool Interno(Geometry layout)
        {
            IntersectionDetail resultado = layout.FillContainsWithDetail(this.Design.Data);
            //IntersectionDetail resultado = layout.FillContainsWithDetail(this.Design.RenderedGeometry);
            if (resultado == IntersectionDetail.FullyContains || resultado == IntersectionDetail.Empty) return false;
            return true;
        }
    }
}
