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
        bool Mover = false;
        bool Colisao = false;
        bool Copiar = false;

        public MainWindow()
        {
            InitializeComponent();
            Controle.Design = Design;
        }
        private void frm_Principal_MouseMove(object sender, MouseEventArgs e)
        {
            if (Controle.ActiveOcc != null)
            {
                if (Mover && Colisao)
                {
                    Controle.ActiveOcc.Mover_Colisao(e.GetPosition(Design), Controle.Layout_Ativo.Retalho);
                }
                else if (Mover && Colisao == false)
                {
                    Controle.ActiveOcc.Mover(e.GetPosition(Design));
                }
            }

        }
        private void frm_Principal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(Mover)
            {
                if (Controle.ActiveOcc != null) //LARGANDO A PEÇA
                {
                    Controle.Arranjar();
                    return;
                }

                if (e.Source.GetType() != typeof(Path)){ return; }                      //SE O TIPO É DIF. DE PATH
                if (((Path)e.Source).Tag == null) { return; }                           //SE A TAG É NULL
                if (((Path)e.Source).Tag.GetType() != typeof(Occurrences)) { return; }  //SE O TIPO É DIF. DE OCCURRENCES
                var Occ = (Occurrences)((Path)e.Source).Tag;
                if (Occ.GetType() == typeof(Occurrences)) //MOVER A PEÇA
                {
                    Controle.StartMove((Occurrences)Occ);
                }
            }
            if (Copiar)
            {

            }

        }
        private void btn_import_inventor_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog DialogOpen = new Microsoft.Win32.OpenFileDialog();
            DialogOpen.Multiselect = false;
            DialogOpen.FileName = string.Empty;
            if (DialogOpen.ShowDialog() != true) return;

            if (!System.IO.File.Exists(DialogOpen.FileName)) return;
            Controle.Importar_Inventor(DialogOpen.FileName, arvore);
            arvore.ItemsSource = null;
            arvore.ItemsSource = Controle.Tree;
        }
        private void arvore_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.DataContext = e.NewValue;
            try
            {
                if (e.NewValue.GetType() == typeof(Tarefa))
                {
                    Controle.Layout_Ativo = ((Tarefa)e.NewValue).Layouts[0];
                    Controle.Layout_Ativo.Alterar_Context(Design);
                    Itens_Tarefa.DataContext = Controle.Layout_Ativo.Parent;
                }
                else if (e.NewValue.GetType() == typeof(Layout))
                {
                    Controle.Layout_Ativo = (Layout)e.NewValue;
                    Controle.Layout_Ativo.Alterar_Context(Design);
                    Itens_Tarefa.DataContext = Controle.Layout_Ativo.Parent;
                }
                else
                {
                    Design.DataContext = null;
                    Itens_Tarefa.DataContext = null;
                }
            } catch { }
        }

        private void frm_Principal_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2:
                    Controle.ActiveOcc.Rotate(90);
                    break;
                case (Key.M):
                    if (Mover == false)
                    {
                        Mover = true;
                        cmd_move.Stroke = Brushes.Yellow;
                    }
                    else
                    {
                        Mover = false;                    
                        cmd_move.Stroke = Brushes.White;
                    }
                    break;
                case (Key.Escape):
                    if (Controle.ActiveOcc != null)
                    {
                        Controle.DeleteOcc();
                    }
                    Mover = false;
                    Colisao = false;
                    cmd_colisao.Stroke = Brushes.White;
                    cmd_copiar.Stroke = Brushes.White;
                    cmd_move.Stroke = Brushes.White;
                    break;
                case (Key.B):
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
                    break;
                case (Key.C):
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
                    break;
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
        //    for (int x = Controle.Layout_Ativo.min_x + 2; x <= Controle.Layout_Ativo.max_x; x = x + desloc_mm)
        //    {
        //        widths.Clear();
        //        //For por Altura                  
        //        for (int y = Controle.Layout_Ativo.min_y + 2; y <= Controle.Layout_Ativo.max_y; y = y + desloc_mm)
        //        {
        //            foreach (oItem foco in Occorencias)
        //            {
        //                if (foco.mat != Controle.Layout_Ativo.material)
        //                {
        //                    continue;
        //                }
        //                if (Retalho.GetArea() < Controle.Layout_Ativo.geo.Data.GetArea() * 0.1) return;

        //                foco.geo.Tag = foco;
        //                foco.geo.Data.Transform = Definir_Foco(foco, Controle.Layout_Ativo.escala);

        //                //Se o retalho contiver o ponto em questão
        //                if (Retalho.FillContains(new System.Windows.Point(x * Controle.Layout_Ativo.escala, y * Controle.Layout_Ativo.escala)))
        //                {
        //                    trf.Y = y * Controle.Layout_Ativo.escala;
        //                    trf.X = x * Controle.Layout_Ativo.escala;
        //                    if (foco.geo.Data.FillContainsWithDetail(Retalho) == IntersectionDetail.FullyInside)
        //                    {
        //                        Layout.Children.Add(foco.geo);//Add no layout
        //                        Retalho = new CombinedGeometry(GeometryCombineMode.Exclude, Retalho, foco.geo.Data);//Recorta o retalho
        //                        Occorencias.Remove(foco); //Remove das occorrencias pendentes
        //                        y = (int)(foco.geo.Data.Bounds.Bottom / Controle.Layout_Ativo.escala);
        //                        widths.Add((int)(foco.geo.Data.Bounds.Right / Controle.Layout_Ativo.escala));
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
                    Controle.ActiveOcc = pt.Criar_Occurrence(new[] { Design.ActualWidth, Design.ActualHeight }, Controle.Layout_Ativo);
                    Design.Children.Add(Controle.ActiveOcc.Design);
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
                
                prnt.PrintQueue = System.Printing.LocalPrintServer.GetDefaultPrintQueue();
                prnt.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
                Controle.Layout_Ativo.Design.Fill = null;
                prnt.PrintVisual(Design, "Printing Canvas");
                Controle.Layout_Ativo.Design.Fill = Brushes.SlateGray;
            }
        }
    }
}
