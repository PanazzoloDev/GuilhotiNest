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
using Inventor;

namespace GuilhotiNestv01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        oInventor inv;
        oItem it_foco;
        oLayout layout_ativo;
        List<oItem> itens;
        List<oItem> Occorencias;
        TreeViewItem trv;
        System.Windows.Point centro;

        TranslateTransform trf;
        RotateTransform rtf;
        TransformGroup grp;
        ScaleTransform sc;
        System.Windows.Point ant;

        object[] linha;
        Dictionary<int,string> tarefas;
        List<object[]> Parts;

        bool mover = false;
        bool colisao = false;
        bool copiar = false;

        public MainWindow()
        {
            InitializeComponent();
            inv = new oInventor();    
        }           
        private void frm_Principal_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point pos = e.GetPosition(Layout);
            lb_x.Content = Math.Round(pos.X,2);
            lb_y.Content = Math.Round(pos.Y, 2);
            if (mover && it_foco != null)
            {
                //Se o comando Colisão estiver ativo
                if (colisao)
                {
                    trf.X = pos.X - ant.X;
                    trf.Y = pos.Y - ant.Y;
                    if (verif_colisao(it_foco, layout_ativo))
                    {
                        it_foco.geo.Visibility = Visibility.Collapsed;
                    }else
                    {
                        it_foco.geo.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    trf.X = pos.X - ant.X;
                    trf.Y = pos.Y - ant.Y;
                }              
            }
        }
        private void frm_Principal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mover && it_foco != null)
            {
                it_foco = null;
                mover = false;
                colisao = false;
                cmd_colisao.Stroke = Brushes.DarkBlue;
                cmd_move.Stroke = Brushes.DarkBlue;
            }
            else
            {
                //Verifica se o tipo do objeto selecionado é do tipo Path
                if (Mouse.DirectlyOver.GetType() != typeof(System.Windows.Shapes.Path))
                {
                    return;
                }

                //Obtém o item que foi selecionado, e verifica se não é o layout ativo
                System.Windows.Shapes.Path pt = (System.Windows.Shapes.Path)Mouse.DirectlyOver;
                if (pt.Tag == null)
                {
                    return;
                }

                if (mover && !copiar)
                {
                    //Recupera as transformacoes já existentes p/ modificar
                    it_foco = (oItem)pt.Tag;
                    grp = (TransformGroup)it_foco.geo.Data.Transform;
                    trf = (TranslateTransform)grp.Children[2];
                    rtf = (RotateTransform)grp.Children[1];

                    System.Windows.Point pos = e.GetPosition(Layout);
                    ant = new System.Windows.Point(pos.X - trf.X, pos.Y - trf.Y);
                }
                else if (!mover && copiar)
                {
                    it_foco = (oItem)((System.Windows.Shapes.Path)Mouse.DirectlyOver).Tag;
                    it_foco = add_occ(it_foco, false);
                }
            }
        }
        private void btn_import_inventor_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opdlg = new OpenFileDialog();
            opdlg.ShowDialog();
            if (System.IO.File.Exists(opdlg.FileName) == false)
            {
                MessageBox.Show("Arquivo não selecionado/Inválido");
                return;
            }
            itens = inv.Read_Itens(opdlg.FileName);
            //OCCS
            Occorencias = new List<oItem>();
            foreach (oItem occ in itens)
            {
                for (int _ = 1;  _ <= occ.qtde; _++){
                    Occorencias.Add(occ);
                }
            }
            //FIM OCCS
            var tasks = itens.GroupBy(o => o.mat);
            int id = 0;
            tarefas = new Dictionary<int, string>();
            Parts = new List<object[]>();

            foreach (IGrouping<string,oItem> Material in tasks)
            {
                TreeViewItem trvm = new TreeViewItem() { Header = Material.Key, Foreground = Brushes.Cyan, FontWeight = FontWeights.Bold,Tag= "mat" };
                tarefas.Add(id++, Material.Key);
                foreach(oItem it in itens)
                {
                    if(it.mat == Material.Key)
                    {
                        TreeViewItem trvi = new TreeViewItem() { Header = it.item, Foreground = Brushes.White , Tag = it };
                        linha = new object[2];  linha[0] = id; linha[1] = it; 
                        Parts.Add(linha);
                        trvm.Items.Add(trvi);
                    }
                }
                arvore.Items.Add(trvm);
            }        
        }
        private void frm_Principal_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }
        private void frm_Principal_MouseWheel(object sender, MouseWheelEventArgs e)
        {
        }
        private void arvore_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            trv = (TreeViewItem)e.NewValue;
            if(trv.Tag.ToString() != "mat")
            {
                it_foco = (oItem)trv.Tag;
                lb_item.Content = it_foco.item;
                lb_ordem.Content = it_foco.ordem;
                lb_qtde.Content = it_foco.qtde;
                lb_area.Content = it_foco.area;
                lb_dim_x.Content = it_foco.dim_x;
                lb_dim_y.Content = it_foco.dim_y;
                img.Source = RenderToBitmap2(it_foco.geo);
            }else
            {
                it_foco = null;
                img.Source = null;
                lb_item.Content = '-';
                lb_ordem.Content = '-';
                lb_qtde.Content = '-';
                lb_area.Content = '-';
                lb_dim_x.Content = '-';
                lb_dim_y.Content = '-';
            }
        }
        private BitmapFrame RenderToBitmap2(System.Windows.Shapes.Path it)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)Math.Round(it.Data.Bounds.Width*1.5625,0), (int)Math.Round(it.Data.Bounds.Height*1.5625,0), 150, 150, PixelFormats.Pbgra32);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            drawingContext.DrawGeometry(Brushes.DarkBlue,new Pen(Brushes.Red,1), it.Data);
            drawingContext.Close();
            renderBitmap.Render(drawingVisual);
            return BitmapFrame.Create(renderBitmap);
        }
        private void arvore_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (trv.IsMouseOver && trv.Tag.ToString() != "mat")
                {
                    //Recupera e clona o item da arvore
                    it_foco = (oItem)trv.Tag;
                    it_foco = add_occ(it_foco, true);
                }
            }
            catch
            {

            }
        }
        private void frm_Principal_KeyDown(object sender, KeyEventArgs e)
        {
            if (it_foco == null && e.Key == Key.M)
            {
                mover = true;
                cmd_move.Stroke = Brushes.White;
            }
            else if(it_foco != null && e.Key == Key.R)
            {
                rtf.CenterX = centro.X * layout_ativo.escala;
                rtf.CenterY = centro.Y * layout_ativo.escala;
                rtf.Angle = rtf.Angle + 5;

            }else if(it_foco != null && e.Key == Key.Delete)
            {
                mover = false;
                Layout.Children.Remove(it_foco.geo);
                it_foco = null;
                cmd_move.Stroke = Brushes.DarkBlue;
            }else if(it_foco != null && mover && e.Key == Key.B)
            {
                if (colisao)
                {
                    colisao = false;
                    cmd_colisao.Stroke = Brushes.DarkBlue;
                }
                else
                {
                    colisao = true;
                    cmd_colisao.Stroke = Brushes.White;
                }

            }else if(it_foco == null && e.Key == Key.C)
            {
                if (copiar)
                {
                    copiar = false;
                    cmd_copiar.Stroke = Brushes.DarkBlue;
                }else
                {
                    cmd_copiar.Stroke = Brushes.White;
                    copiar = true;
                }
            }
        }
        private void btn_novo_layout_Click(object sender, RoutedEventArgs e)
        {
            //Exibe o formulário p/ criação de retalhos/Chapas
            frm_import novo_lay = new frm_import(tarefas);
            novo_lay.ShowDialog();
            
            //Após fechado, verifica se foi criado alguma chapa
            if(novo_lay.lay != null)
            {
                layout_ativo = novo_lay.lay;
            }else
            {
                return;
            }
            //Calcula e redimensiona o retalho de acordo com as medidas
            double[] vh = new double[2];
            vh[0] = (Layout.ActualWidth * 0.9) / layout_ativo.tam_orig.Width;
            vh[1] = (Layout.ActualHeight * 0.9) / layout_ativo.tam_orig.Height;

            TransformGroup tr = (TransformGroup)layout_ativo.geo.Data.Transform;
            ScaleTransform st = (ScaleTransform)tr.Children[1];
            TranslateTransform t = (TranslateTransform)tr.Children[0];

            st.ScaleX = vh.Min();
            st.ScaleY = vh.Min();
            Layout.Children.Add(layout_ativo.geo);
            layout_ativo.escala = vh.Min();
            t.X = ((Layout.ActualWidth - layout_ativo.geo.RenderedGeometry.Bounds.Width) / 2) / layout_ativo.escala;
            t.Y = ((Layout.ActualHeight - layout_ativo.geo.RenderedGeometry.Bounds.Height) / 2) / layout_ativo.escala;

            layout_ativo.min_x = (int)(layout_ativo.geo.Data.Bounds.Left / layout_ativo.escala);
            layout_ativo.max_x = layout_ativo.min_x + (int)(layout_ativo.geo.Data.Bounds.Width / layout_ativo.escala);

            layout_ativo.min_y = (int)(layout_ativo.geo.Data.Bounds.Top / layout_ativo.escala);
            layout_ativo.max_y = layout_ativo.min_y + (int)(layout_ativo.geo.Data.Bounds.Height / layout_ativo.escala);
        }
        private oItem add_occ(oItem it_foco,bool novo)
        {
            it_foco = it_foco.Clone(it_foco);

            //Instancia e vincula as transformacoes
            if (novo && layout_ativo != null)
            {
                grp = new TransformGroup();
                trf = new TranslateTransform();
                rtf = new RotateTransform();
                sc = new ScaleTransform(layout_ativo.escala, layout_ativo.escala);
                grp.Children.Add(sc);
                grp.Children.Add(rtf);
                grp.Children.Add(trf);

                it_foco.geo.Data.Transform = grp;
            }
            else if(!novo && layout_ativo != null)
            {
                grp = (TransformGroup)it_foco.geo.Data.Transform;
                trf = (TranslateTransform)grp.Children[2];
                rtf = (RotateTransform)grp.Children[1];
                sc = (ScaleTransform)grp.Children[0];

                grp = new TransformGroup();
                grp.Children.Add(sc);
                grp.Children.Add(rtf);
                grp.Children.Add(trf);
                it_foco.geo.Data.Transform = grp;
            }else
            {
                MessageBox.Show("Nenhum retalho/Layout encontrado!");
                return null;
            }
            //Insere o clone no espaco de desenho
            it_foco.geo.Tag = it_foco;
            Layout.Children.Add(it_foco.geo);

            //Define o click como centro
            ant = new System.Windows.Point((it_foco.dim_x / 2) * layout_ativo.escala, (it_foco.dim_y / 2) * layout_ativo.escala);
            centro = new System.Windows.Point(it_foco.dim_x / 2, it_foco.dim_y / 2);
            mover = true; cmd_move.Stroke = Brushes.White;
            return it_foco;
        }
        private bool verif_colisao(oItem foco, oLayout layout_ativo)
        {
            foreach (UIElement occ in Layout.Children)
            {
                System.Windows.Shapes.Path oc = ((System.Windows.Shapes.Path)occ);
                if(foco.geo != oc)
                {
                    IntersectionDetail inc = foco.geo.Data.FillContainsWithDetail(oc.Data);
                    IntersectionDetail det = layout_ativo.geo.Data.FillContainsWithDetail(foco.geo.Data);
                    if (inc != IntersectionDetail.NotCalculated && inc != IntersectionDetail.Empty && oc.Tag != null && oc != foco.geo || det != IntersectionDetail.FullyContains)
                    {
                        return true;
                    }
                }
            }       
            return false; 
        }
        private void btn_auto_Click(object sender, RoutedEventArgs e)
        {
            DateTime inicio = DateTime.Now;
            Geometry Retalho = layout_ativo.geo.Data;
            foreach (oItem foco in Occorencias)
            {
                if (foco.mat == layout_ativo.material)
                {
                        if (Retalho.GetArea() < layout_ativo.geo.Data.GetArea() * 0.3) return;

                        grp = new TransformGroup();
                        trf = new TranslateTransform();
                        sc = new ScaleTransform(layout_ativo.escala, layout_ativo.escala);
                        rtf = new RotateTransform();

                        grp.Children.Add(sc);
                        grp.Children.Add(rtf);
                        grp.Children.Add(trf);
                        rtf.CenterX = (foco.dim_x / 2) * layout_ativo.escala;
                        rtf.CenterY = (foco.dim_y / 2) * layout_ativo.escala;

                        foco.geo.Tag = foco;
                        foco.geo.Data.Transform = grp;
                        int desloc_mm = 4;

                        for (int x = layout_ativo.min_x + 2; x <= layout_ativo.max_x; x = x + desloc_mm)
                        {
                            for (int y = layout_ativo.min_y + 2; y <= layout_ativo.max_y; y = y + desloc_mm)
                            {
                                for (int rot = 0; rot <= 270; rot = rot + 90)
                                {
                                    if(Retalho.FillContains(new System.Windows.Point(x,y)))
                                    {
                                        //rtf.Angle = rot;
                                        //trf.Y = y * layout_ativo.escala;
                                        //trf.X = x * layout_ativo.escala;
                                        //if (Retalho.FillContainsWithDetail(foco.geo.Data) == IntersectionDetail.FullyContains)
                                        //{
                                        //    Layout.Children.Add(foco.geo);
                                        //    Retalho = new CombinedGeometry(GeometryCombineMode.Exclude, Retalho, foco.geo.Data);
                                        //    goto proximo;
                                        //}
                                    }
                                }
                            }
                        }
                        proximo:
                        continue;
                }
            }
            MessageBox.Show((DateTime.Now - inicio).ToString());
        }
    }
}
