using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GuilhotiNestv01
{
    class oItem
    {
        public Path geo;
        int qtde;
        string item;
        int ordem;
        double dim_x;
        double dim_y;
        double area;

        //public oItem(string data, int cod, int num_ordem, int qt,double x, double y, double a)
        public oItem(string data, string cod, int qt, double x, double y, double a)
        {
            geo = new Path();
            GeometryConverter conv = new GeometryConverter();
            geo.Data = (Geometry)conv.ConvertFromString(data);
            geo.Fill = Brushes.DarkBlue;
            geo.Stroke = Brushes.Red;
            geo.StrokeThickness = 1;
            qtde = qt;
            item = cod;

            geo.Data = geo.Data.Clone();
            //ordem = num_ordem;
            dim_x = x;
            //dim_y = y;
            area = a;
        }

    }
}
