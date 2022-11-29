using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GuilhotiNest.ViewModel
{
    public class Document
    {
        public string Nome { get; set; }
        public string Material { get; set; }
        public string Geometria { get; set; }

        public double Espessura { get; set; }
        public double Area { get; set; } = 0;
        public double DimensaoH { get; set; } = 0;
        public double DimensaoV { get; set; } = 0;

        public int Ordem { get; set; } = 00000;
        public int Quantidade { get; set; } = 1;

        public ObservableCollection<Occurrences> Occs { get; set; }
        public System.Windows.Media.Imaging.BitmapFrame Thumbnail { get; set; }

        public Document(string nome, string material, PathGeometry cmd, double esp = 0, double dim_x = 0, double dim_y = 0, double area = 0, int ord = 000000, int qtde = 1)
        {
            Nome = nome;
            Material = material;
            Geometria = cmd.ToString();
            Espessura = esp;
            DimensaoH = dim_x;
            DimensaoV = dim_y;
            Area = area;
            Ordem = ord;
            Quantidade = qtde;
            Thumbnail = Criar_Thumbnail(System.Windows.Media.Geometry.Parse(Geometria));
            Occs = new ObservableCollection<Occurrences>();
            //Occs = Criar_Occurrences(this);
        }
        //private ObservableCollection<Occurrences> Criar_Occurrences(Document Reference)
        //{
        //    ObservableCollection<Occurrences> ocs = new ObservableCollection<Occurrences>();
        //    for(int i = 1; i <= Reference.Quantidade; i++)
        //    {
        //        ocs.Add(new Occurrences(Reference));
        //    }
        //    return ocs;
        //}
        public Occurrences Criar_Occurrence(double[] dim_canvas, Layout lay)
        {
            Occurrences occ = new Occurrences(this,lay, dim_canvas);
            this.Occs.Add(occ);
            return occ;        
        }

        private static BitmapFrame Criar_Thumbnail(Geometry geo)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)Math.Round((geo.Bounds.Width + 20) * 1.5625, 0), (int)Math.Round((geo.Bounds.Height + 20) * 1.5625, 0), 150, 150, PixelFormats.Pbgra32);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            drawingContext.DrawGeometry(Brushes.DarkBlue, new Pen(Brushes.Red, 2), geo);
            drawingContext.Close();
            renderBitmap.Render(drawingVisual);
            return BitmapFrame.Create(renderBitmap);
        }
    }
}
