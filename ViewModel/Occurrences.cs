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
    [Serializable()]
    public class Occurrences
    {
        public bool Arranjado { get; set; } = false;
        public double Altura { get; set; }
        public double Comprimento { get; set; }
        public double Escala { get; set; } = 1;

        //[field: NonSerialized()]
        public Geometry Data;// { get; set; }
        public Document Parent { get; set; }
        //[field: NonSerialized()]
        public Path Design;// { get; set; }
        public Layout layout { get; set; }
        public System.Windows.Rect Limites { get; set; }

        public List<Cortes> Cortes {get;set;}
        private TransformGroup _Trans_Group;
        private ScaleTransform _Scale;
        private TranslateTransform _Translate;
        private RotateTransform _Rotate;

        private TransformGroup _BackupTransformacao { get; set; }
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
        public void Nova_Occ()
        {

        }
        public void Mover_Colisao(System.Windows.Point pt, Geometry layout)
        {
            double x_antigo = _Translate.X;
            double y_antigo = _Translate.Y;

            double x = 1000;
            double y = 1000;

            System.Windows.Point sup_esq = new System.Windows.Point(pt.X - (this.Comprimento / 2), pt.Y - (this.Altura / 2));
            System.Windows.Point inf_dir = new System.Windows.Point(sup_esq.X + this.Comprimento, sup_esq.Y + this.Altura);

            var CortesLayout = Controle.Layout_Ativo.Limites;
            List<Parametros> verticais = new List<Parametros>();
            List<Parametros> horizontais = new List<Parametros>();

            if (CortesLayout.Count != 0)
            {
                foreach (Cortes corte in CortesLayout)
                {
                    if (corte.Orientação == ViewModel.Cortes.Orientacao.Vertical)
                    {
                        verticais.Add(new Parametros() { Corte = corte, Diferenca = Math.Abs(sup_esq.X - corte.Medida), Somar = false });
                        verticais.Add(new Parametros() { Corte = corte, Diferenca = Math.Abs(inf_dir.X - corte.Medida), Somar = true });
                    }
                    else
                    {
                        horizontais.Add(new Parametros() { Corte = corte, Diferenca = Math.Abs(sup_esq.Y - corte.Medida), Somar = false });
                        horizontais.Add(new Parametros() { Corte = corte, Diferenca = Math.Abs(inf_dir.Y - corte.Medida), Somar = true });
                    }
                }
                var vert = verticais.OrderBy(X => X.Diferenca).ToList()[0];
                var hor = horizontais.OrderBy(Y => Y.Diferenca).ToList()[0];

                if (vert.Somar) { x = vert.Corte.Medida - this.Comprimento; } else { x = vert.Corte.Medida; }
                if (hor.Somar) { y = hor.Corte.Medida - this.Altura; } else { y = hor.Corte.Medida; }
            }
            else
            {
                x = sup_esq.X;
                y = sup_esq.Y;
            }

            _Translate.X = x;
            //if (Interno(layout) == true){_Translate.X = x_antigo;}

            _Translate.Y = y;
            //if (Interno(layout) == true){_Translate.Y = y_antigo;}
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
                Fill = Brushes.White,
                Stroke = Brushes.MidnightBlue,
                StrokeThickness = 1,
                Data = this.Data,
                Cursor = System.Windows.Input.Cursors.Hand
            };
        }
        private void Aplicar_Transformacoes(double[] dim_layout)
        {
            this.Escala = layout.Escala;

            this._Trans_Group = new TransformGroup();
            _Scale = new ScaleTransform(this.Escala, this.Escala);
            _Translate = new TranslateTransform();
            _Rotate = new RotateTransform();

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
            IntersectionDetail resultado = layout.FillContainsWithDetail(this.Data);
            //IntersectionDetail resultado = layout.FillContainsWithDetail(this.Design.RenderedGeometry);
            if (resultado == IntersectionDetail.FullyContains || resultado == IntersectionDetail.Empty) return false;
            return true;
        }
        public void SalvarPosicao()
        {
            _BackupTransformacao = _Trans_Group.Clone();
        }
        public void Ultima_Posicao()
        {
            _Rotate = (RotateTransform)_BackupTransformacao.Children[1].Clone();
            _Translate = (TranslateTransform)_BackupTransformacao.Children[2].Clone();
        }

        private class Parametros
        {
            public Cortes Corte { get; set; }
            public double Diferenca { get; set; }
            public bool Somar { get; set; }
        }
    }
}
