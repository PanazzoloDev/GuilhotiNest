using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GuilhotiNest.ViewModel
{
    [Serializable()]
    public class Layout
    {
        public int Número { get; set; }
        public int Repeticoes { get; set; }

        public double Altura { get; set; } = 1200;
        public double Comprimento { get; set; } = 3000;
        public double Escala { get; set; }

        public System.Windows.Point Posição { get; set; }
        public Geometry Data { get; set; }
        public Tarefa Parent { get; set; }
        public ObservableCollection<Occurrences> Occs { get; set; }
        public ObservableCollection<Cortes> Limites { get; set; }
        public Path Design { get; set; }
        public Geometry Retalho { get; set; }

        private TransformGroup _Trans_Group;
        private TranslateTransform _Translate;
        private RotateTransform _Rotate;
        private ScaleTransform _Scale;

        public Layout(int id, double esc, Geometry geometry, Tarefa pai, System.Windows.Point posic, int rep = 1)
        {
            this.Número = id;
            this.Posição = posic;
            this.Parent = pai;
            this.Repeticoes = rep;
            this.Data = geometry;
            this.Escala = esc;

            Criar_Design();
            this.Design.Data.Transform = Aplicar_Transformacoes();

            this.Occs = new ObservableCollection<Occurrences>();
            this.Limites = new ObservableCollection<Cortes>();
            this.Retalho = this.Data.Clone();

        }
        public void Alterar_Context(Canvas Design)
        {
            Design.Children.Clear();
            Design.Children.Add(this.Design);
            foreach (Occurrences occ in this.Occs)
            {
                Design.Children.Add(occ.Design);
            }
        }
        private void Criar_Design()
        {
            Design = new Path()
            {
                Fill = Brushes.SlateGray,
                Stroke = Brushes.MidnightBlue,
                StrokeThickness = 1,
                Data = this.Data
            };
        }
        private TransformGroup Aplicar_Transformacoes()
        {
            _Trans_Group = new TransformGroup();
            _Scale = new ScaleTransform(this.Escala, this.Escala);
            _Translate = new TranslateTransform(Posição.X, Posição.Y);

            _Trans_Group.Children.Add(_Scale);
            _Trans_Group.Children.Add(_Translate);
            
            return _Trans_Group;
        }

    }
}
