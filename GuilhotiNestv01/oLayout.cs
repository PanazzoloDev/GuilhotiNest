using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GuilhotiNestv01
{
    public class oLayout
    {
        public Path geo;
        public int qtde;
        public string material;
        public double escala;
        public int id = 1000;
        public Size tam_orig;
        public int min_x, max_x, min_y, max_y;

        public oLayout(int qtd, string mat, double esc, Path g,Size tam)
        {
            qtde = qtd;
            material = mat;
            escala = esc;
            geo = g;
            id = id++;
            tam_orig = tam;
        }
    }
}