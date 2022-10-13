using GuilhotiNest.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for frm_novatarefa.xaml
    /// </summary>
    public partial class frm_novatarefa : Window
    {
        Tarefa task;
        public frm_novatarefa(ref Tarefa task)
        {
            InitializeComponent();
            this.task = task;
            dg_workspace.ItemsSource = Controle.Documentos;            
        }
        private void btn_add_doc_Click(object sender, RoutedEventArgs e)
        {
            foreach(var cell in dg_workspace.SelectedItems)
            {
                task.Documents.Add((Document)cell);
            }
            Update_Source();
        }
        private void btn_remove_doc_Click(object sender, RoutedEventArgs e)
        {
            List<Document> docs = new List<Document>();
            foreach (var doc in dg_task.SelectedItems) { docs.Add((Document)doc); }
            foreach(var obj in docs)
            {
                task.Documents.Remove(obj);
            }
            
            Update_Source();
        }
        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btn_criar_tarefa_Click(object sender, RoutedEventArgs e)
        {
            double esp = 0;
            if (string.IsNullOrWhiteSpace(cb_material.Text))
            {
                MessageBox.Show("Material inválido", "Parâmetro Incorreto", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!double.TryParse(cb_espessura.Text, out esp))
            {
                MessageBox.Show("Espessura inválida", "Parâmetro Incorreto", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            task.Espessura = esp;
            task.Nome = txt_nome_task.Text;
            task.Material = cb_material.Text;
            Controle.Tarefas.Add(task);
            Controle.Tree.Add(task);
            this.Close();
        }
        private void Update_Source()
        {
            dg_task.ItemsSource = null;
            cb_espessura.ItemsSource = null;
            cb_material.ItemsSource = null;

            dg_task.ItemsSource = task.Documents;
            cb_espessura.ItemsSource = task.Documents.GroupBy(x => x.Espessura).ToList();
            cb_material.ItemsSource = task.Documents.GroupBy(x => x.Material).ToList();
        }
    }
}
