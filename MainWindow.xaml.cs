using GuilhotiNest.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GuilhotiNest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Layout Layout_Ativo;
        Occurrences Occ_Ativo;
        bool Mover = false;
        bool Colisao = false;
        bool Copiar = false;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void frm_Principal_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mover && Occ_Ativo != null && Colisao == true)
            {
                Occ_Ativo.Mover_Colisao(e.GetPosition(Design),Layout_Ativo.Retalho);
            }
            else if (Mover && Occ_Ativo != null && Colisao == false)
            {               
                Occ_Ativo.Mover(e.GetPosition(Design));
            }
        }
        private void frm_Principal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Occ_Ativo != null)
                {
                    Occ_Ativo.Limites = Occ_Ativo.Design.Data.GetRenderBounds(new Pen());
                    Layout_Ativo.Retalho = Geometry.Combine(Layout_Ativo.Retalho, Occ_Ativo.Design.Data, GeometryCombineMode.Exclude, null);
                    Cortes cor = new Cortes(Occ_Ativo.Design.Data.Bounds.Right, Cortes.Orientacao.Vertical, Layout_Ativo);
                    Cortes cor2 = new Cortes(Occ_Ativo.Design.Data.Bounds.Top, Cortes.Orientacao.Horizontal, Layout_Ativo);
                    Layout_Ativo.Limites.Add(cor); Layout_Ativo.Limites.Add(cor2);
                    Occ_Ativo.Cortes.Add(cor); Occ_Ativo.Cortes.Add(cor2);
                    Design.Children.Add(cor.Design); Design.Children.Add(cor2.Design);
                    Occ_Ativo = null; Mover = false;
                }
                else if (Occ_Ativo == null && Mover == true && ((Path)e.Source).Tag.GetType() == typeof(Occurrences))
                {
                    Occ_Ativo = (Occurrences)((Path)e.Source).Tag;
                    Layout_Ativo.Retalho = Geometry.Combine(Layout_Ativo.Retalho, Occ_Ativo.Design.Data, GeometryCombineMode.Union, null);
                    Design.Children.Remove(Occ_Ativo.Cortes[0].Design);
                    Design.Children.Remove(Occ_Ativo.Cortes[1].Design);
                    Layout_Ativo.Limites.Remove(Occ_Ativo.Cortes[0]);
                    Layout_Ativo.Limites.Remove(Occ_Ativo.Cortes[1]);
                    Occ_Ativo.Cortes.Clear();
                }
                else if (Occ_Ativo == null && Copiar && ((Path)e.Source).Tag.GetType() == typeof(Occurrences))
                {
                    Document pt = ((Occurrences)((Path)e.Source).Tag).Parent;
                    Occ_Ativo = pt.Criar_Occurrence(new[] { Design.ActualWidth, Design.ActualHeight }, Layout_Ativo);
                    Design.Children.Add(Occ_Ativo.Design);

                    Mover = true; Copiar = false;
                    cmd_copiar.Stroke = Brushes.White;
                    cmd_move.Stroke = Brushes.Yellow;
                }
            }
            catch
            {

            }

        }
        private void btn_import_inventor_Click(object sender, RoutedEventArgs e)
        {
            //string valor = "Teste";
            //var res = ToBinaryString(Encoding.UTF8, valor);

            //string valor2 = "Teste2";
            //var res2 = ToBinaryString(Encoding.UTF8, valor2);

            //MessageBox.Show(res +'\n'+res2 );
            Microsoft.Win32.OpenFileDialog DialogOpen = new Microsoft.Win32.OpenFileDialog();
            DialogOpen.Multiselect = false;
            DialogOpen.FileName = string.Empty;
            if (DialogOpen.ShowDialog() != true) return;

            if (!System.IO.File.Exists(DialogOpen.FileName)) return;
            Controle.Importar_Inventor(DialogOpen.FileName, arvore);
            arvore.ItemsSource = null;
            arvore.ItemsSource = Controle.Tree;
        }
        static string ToBinaryString(Encoding encoding, string text)
        {
            return string.Join("", encoding.GetBytes(text).Select(n => Convert.ToString(n, 2).PadLeft(8, '0')));
        }
        private void frm_Principal_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }
        private void frm_Principal_MouseWheel(object sender, MouseWheelEventArgs e)
        {
        }
        private void arvore_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.DataContext = e.NewValue;
            try
            {
                if (e.NewValue.GetType() == typeof(Tarefa))
                {
                    Layout_Ativo = ((Tarefa)e.NewValue).Layouts[0];
                    Layout_Ativo.Alterar_Context(Design);
                    Itens_Tarefa.DataContext = Layout_Ativo.Parent;
                }
                else if (e.NewValue.GetType() == typeof(Layout))
                {
                    Layout_Ativo = (Layout)e.NewValue;
                    Layout_Ativo.Alterar_Context(Design);
                    Itens_Tarefa.DataContext = Layout_Ativo.Parent;
                }
                else
                {
                    Design.DataContext = null;
                    Itens_Tarefa.DataContext = null;
                }
            } catch { }


    }
        private void arvore_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
        private void frm_Principal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.R && Occ_Ativo != null)
            {
                Occ_Ativo.Rotate(90);
            }
            else if (e.Key == Key.M)
            {
                if (Mover == false)
                {
                    Mover = true;
                    Occ_Ativo = null;
                    cmd_move.Stroke = Brushes.Yellow;
                }
                else
                {
                    Mover = false;
                    cmd_move.Stroke = Brushes.White;
                }

            }
            else if (e.Key == Key.Escape)
            {
                if (Occ_Ativo != null)
                {
                    Design.Children.Remove(Occ_Ativo.Design);
                    Layout_Ativo.Occs.Remove(Occ_Ativo);
                }
                Mover = false;
                Colisao = false;
                cmd_colisao.Stroke = Brushes.White;
                cmd_copiar.Stroke = Brushes.White;
                cmd_move.Stroke = Brushes.White;
            }
            else if (e.Key == Key.B)
            {
                if (Colisao == false)
                {
                    Colisao = true;
                    cmd_colisao.Stroke = Brushes.Yellow;
                }
                else
                {
                    Colisao = false;
                    cmd_colisao.Stroke = Brushes.White;
                }
            }
            else if (e.Key == Key.C)
            {
                if (Copiar == false)
                {
                    Copiar = true;
                    cmd_copiar.Stroke = Brushes.Yellow;
                }
                else
                {
                    Copiar = false;
                    cmd_copiar.Stroke = Brushes.White;
                }
            }
        }
        private void btn_nova_tarefa_Click(object sender, RoutedEventArgs e)
        {
            if (Controle.Grupos.Count == 0) { MessageBox.Show("Não é permitido a criação de tarefas vazias.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning); return;}
            Tarefa task = new Tarefa("SAE 1020", 2.65, new[] {Design.ActualWidth,Design.ActualHeight});
            frm_novatarefa frm = new frm_novatarefa(ref task);
            frm.ShowDialog();
            arvore.ItemsSource = null;
            arvore.ItemsSource = Controle.Tree;

        }
        private void btn_auto_Click(object sender, RoutedEventArgs e)
        {
        //    int desloc_mm = 1;
        //    List<int> widths = new List<int>();
        //    DateTime inicio = DateTime.Now;

        //    //For por Comprimento
        //    for (int x = layout_ativo.min_x + 2; x <= layout_ativo.max_x; x = x + desloc_mm)
        //    {
        //        widths.Clear();
        //        //For por Altura                  
        //        for (int y = layout_ativo.min_y + 2; y <= layout_ativo.max_y; y = y + desloc_mm)
        //        {
        //            foreach (oItem foco in Occorencias)
        //            {
        //                if (foco.mat != layout_ativo.material)
        //                {
        //                    continue;
        //                }
        //                if (Retalho.GetArea() < layout_ativo.geo.Data.GetArea() * 0.1) return;

        //                foco.geo.Tag = foco;
        //                foco.geo.Data.Transform = Definir_Foco(foco, layout_ativo.escala);

        //                //Se o retalho contiver o ponto em questão
        //                if (Retalho.FillContains(new System.Windows.Point(x * layout_ativo.escala, y * layout_ativo.escala)))
        //                {
        //                    trf.Y = y * layout_ativo.escala;
        //                    trf.X = x * layout_ativo.escala;
        //                    if (foco.geo.Data.FillContainsWithDetail(Retalho) == IntersectionDetail.FullyInside)
        //                    {
        //                        Layout.Children.Add(foco.geo);//Add no layout
        //                        Retalho = new CombinedGeometry(GeometryCombineMode.Exclude, Retalho, foco.geo.Data);//Recorta o retalho
        //                        Occorencias.Remove(foco); //Remove das occorrencias pendentes
        //                        y = (int)(foco.geo.Data.Bounds.Bottom / layout_ativo.escala);
        //                        widths.Add((int)(foco.geo.Data.Bounds.Right / layout_ativo.escala));
        //                        goto Found;
        //                    }
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //            Found:
        //            continue;

        //        }
        //        x = widths.Count == 0 ? x : widths.Min();

        //    }
        }
        private void Itens_Tarefa_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(e.Source.GetType() == typeof(ListView))
            {
                try
                {
                    Document pt = (Document)((ListView)e.Source).SelectedItem;
                    Occ_Ativo = pt.Criar_Occurrence(new[] { Design.ActualWidth, Design.ActualHeight }, Layout_Ativo);
                    Design.Children.Add(Occ_Ativo.Design);
                    Mover = true; cmd_move.Stroke = Brushes.Yellow;
                }
                catch{}
            }
            

        }

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog prnt = new PrintDialog();
            if (prnt.ShowDialog() == true)
            {
                //Size pageSize = new Size(prnt.PrintableAreaWidth, prnt.PrintableAreaHeight);
                //Design.Measure(pageSize);
                //Design.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));
                
                prnt.PrintQueue = System.Printing.LocalPrintServer.GetDefaultPrintQueue();
                prnt.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
                prnt.PrintVisual(Design, "Printing Canvas");

            }
        }
    }
}
