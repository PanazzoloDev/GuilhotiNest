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

namespace GuilhotiNestv01
{
    
    /// <summary>
    /// Interaction logic for frm_import.xaml
    /// </summary>
    public partial class frm_import : Window
    {
        public oLayout lay;
        Size dim;
        Path geo;

        public frm_import(Dictionary<int,string> mat)
        {
            InitializeComponent();
            cb_materiais.ItemsSource = mat;
            cb_materiais.SelectedValuePath = "Key";
            cb_materiais.DisplayMemberPath = "Value";
        }
        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rep_chapa.Children.Clear();
                GeometryConverter conv = new GeometryConverter();

                geo = new Path();              
                geo.Data = (Geometry)conv.ConvertFromString(txt_cmd_chapa.Text);
                geo.Data = geo.Data.Clone();
                geo.Fill = Brushes.SteelBlue;
                geo.Stroke = Brushes.DarkBlue;
                geo.StrokeThickness = 1;

                double[] vh = new double[2];
                dim = new Size(geo.Data.Bounds.Width, geo.Data.Bounds.Height);
                vh[0] = 304 / dim.Width;
                vh[1] = 222.4 / dim.Height;

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
                rep_chapa.Children.Add(geo);
            }
            catch
            {
                MessageBox.Show("Comando inválido.");
            }

        }
        private void btn_criar_Click(object sender, RoutedEventArgs e)
        {
            if(cb_materiais.SelectedItem == null || cb_materiais.SelectedItem.ToString() == string.Empty)
            {
                MessageBox.Show("Material não informado.");
                return;
            }
            lay = new oLayout(2, cb_materiais.Text, 1, geo,dim);
            this.Close();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
                rep_chapa.Children.Clear();           
        }
    }
}
