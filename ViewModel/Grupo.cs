using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuilhotiNest.ViewModel
{
    [Serializable()]
    public class Grupo
    {
        public string Material { get; set; }
        public double Espessura { get; set; }
        public ObservableCollection<Document> Documents { get; set; }

        public Grupo(string mat, double esp)
        {
            this.Material = mat;
            this.Espessura = esp;
            Documents = new ObservableCollection<Document>();
        }
    }
}
