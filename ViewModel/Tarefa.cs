using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuilhotiNest.ViewModel
{
    public class Tarefa
    {
        public string Nome { get; set; } = "TaskDefault";
        public string Material { get; set; }
      

        public double Espessura { get; set; }

        public ObservableCollection<Document> Documents { get; set; } = new ObservableCollection<Document>();
        public ObservableCollection<Layout> Layouts { get; set; } = new ObservableCollection<Layout>();
        public Tarefa(string mat, double esp, double[] dim, string nome = "Default")
        {
            Material = mat;
            Nome = nome;
            Espessura = esp;
            
            Layouts.Add(new Layout(01, dim, "m 0 0 h 3000 v 1200 h -3000 z", this));
        }
    }
}
