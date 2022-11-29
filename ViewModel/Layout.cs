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
    public class Layout
    {
        public int Número { get; set; }
        public int Repeticoes { get; set; }

        public double Altura { get; set; } = 1200;
        public double Comprimento { get; set; } = 3000;
        public double Escala { get; set; }

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

        public Layout(int id, double[] dim, string cmd, Tarefa pai, int rep = 1)
        {
            Número = id;
            Parent = pai;
            Repeticoes = rep;
            Data = Geometry.Parse(cmd).Clone();

            Criar_Design();
            Design.Data.Transform = Aplicar_Transformacoes(dim);

            Occs = new ObservableCollection<Occurrences>();
            Limites = new ObservableCollection<Cortes>();
            Retalho = this.Data.Clone();

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
        private TransformGroup Aplicar_Transformacoes(double[] dim)
        {
            this.Escala = (new[] { ((dim[0] * 0.95) / 3000), ((dim[1] * 0.8) / 1200) }).Min();
            double offset_y = (dim[0] * 0.1);
            double offset_x = (dim[1] * 0.025);

            _Trans_Group = new TransformGroup();
            _Scale = new ScaleTransform(this.Escala, this.Escala);
            _Translate = new TranslateTransform(offset_x, offset_y);

            _Trans_Group.Children.Add(_Scale);
            _Trans_Group.Children.Add(_Translate);
            return _Trans_Group;
        }
    }
}
