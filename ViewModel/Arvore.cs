using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuilhotiNest.ViewModel
{
    public class Arvore
    {
        public static List<object> Tree = new List<object>();
        public static ObservableCollection<Grupo> Grupos = new ObservableCollection<Grupo>();
        public static ObservableCollection<Tarefa> Tarefas = new ObservableCollection<Tarefa>();

    }
}
