using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace GuilhotiNestv01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool cmd_move = false;
        MatrixTransform trf;
        List<oItem> oItens;
        oItem it_foco;
        int id;
        Point startpoint;


        public MainWindow()
        {
            InitializeComponent();
            oItens = new List<oItem>();
        }

        private void frm_Principal_MouseMove(object sender, MouseEventArgs e)
        {
            bool intersect = false;
            if(cmd_move == true)
            {
                Point pos = e.GetPosition(layout);
                foreach (oItem it in oItens)
                {
                    if (it_foco.geo.RenderedGeometry.FillContains(it.geo.RenderedGeometry,1, ToleranceType.Relative)&& it_foco.geo.Tag != it.geo.Tag)
                    {
                        intersect = true;
                        break;
                    }
                }
                if(intersect == false)
                {
                    Matrix tr = it_foco.geo.Data.Transform.Value;
                    tr.Translate(pos.X - startpoint.X, pos.Y - startpoint.Y);
                    trf = new MatrixTransform(tr);
                    it_foco.geo.Data.Transform = trf;
                }
                else
                {
                    Matrix tr = it_foco.geo.Data.Transform.Value;
                    tr.Translate(startpoint.X - pos.X, startpoint.Y - pos.Y);
                    trf = new MatrixTransform(tr);
                    it_foco.geo.Data.Transform = trf;
                }
                startpoint = pos;

            }
        }
        private void frm_Principal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (cmd_move == true)
            {
                cmd_move = false;
                it_foco = null;
            }
            else
            {
                
                foreach (oItem it in oItens)
                {
                    if (it.geo.Data.FillContains(e.GetPosition(layout)))
                    {
                        
                        cmd_move = true;
                        it_foco = it;
                        startpoint = e.GetPosition(layout);
                        break;
                    }
                }
            }
        }
        private void btn_import_inventor_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fl_dlg = new OpenFileDialog();
            fl_dlg.Filter = "Part Files(*.ipt) | *.ipt| Assembly Files(*.iam) | *.iam";
            fl_dlg.Multiselect = false;
            fl_dlg.ShowDialog();


            if (File.Exists(fl_dlg.FileName) == false)
            {
                MessageBox.Show("Item inválido");
                return;
            }
            oInventor inv = new oInventor();
            it_foco = inv.Read_Item(fl_dlg.FileName);
            it_foco.geo.Tag = id++;
            
            oItens.Add(it_foco);
            layout.Children.Add(it_foco.geo);
        }
    }
}
