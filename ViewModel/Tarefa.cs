using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GuilhotiNest.ViewModel
{

    public class Tarefa
    {
        public string Nome { get; set; } = "TaskDefault";
        public string Material { get; set; }
        public double Espessura { get; set; }

        public Canvas Design { get; set; }
        public ObservableCollection<Document> Documents { get; set; } = new ObservableCollection<Document>();
        public ObservableCollection<Layout> Layouts { get; set; } = new ObservableCollection<Layout>();
        public Tarefa(string mat, double esp, double[] dim, string nome = "TaskDefault")
        {
            this.Material = mat;
            this.Nome = nome;
            this.Espessura = esp;

            Layouts.Add(Controle.NovoLayout_Retangular(1200, 3000,this));

        }
    }
}
