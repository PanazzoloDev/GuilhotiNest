using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GuilhotiNest
{
    /// <summary>
    /// Interaction logic for frm_import.xaml
    /// </summary>
    public partial class frm_import : Window
    {
        public frm_import()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void txt_cmd_chapa_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Geometry lay = Geometry.Parse(txt_cmd_chapa.Text);
                Path geo = new Path() {Data = lay.Clone(), Fill = Brushes.SteelBlue, Stroke = Brushes.DarkBlue, StrokeThickness = 1 };

                double[] vh = new double[2];
                vh[0] = 304 / lay.Bounds.Width;
                vh[1] = 222.4 / lay.Bounds.Height;

                TransformGroup grp = new TransformGroup();
                TranslateTransform tf = new TranslateTransform();
                ScaleTransform st = new ScaleTransform();
                st.CenterY = -vh[1] / 2;
                st.CenterX = -vh[0] / 2;

                st.ScaleX = vh.Min();
                st.ScaleY = vh.Min();

                grp.Children.Add(tf);
                grp.Children.Add(st);
                geo.Data.Transform = grp;
                rep_chapa.Children.Clear();
                rep_chapa.Children.Add(geo);
            }
            catch
            {
                return;        
            }

        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_criar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
