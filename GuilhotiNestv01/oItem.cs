using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using System.Windows;

namespace GuilhotiNestv01
{
    class oItem
    {
        public System.Windows.Shapes.Path geo;
        public int qtde;
        public string item;
        public string mat;
        public int ordem;
        public double dim_x;
        public double dim_y;
        public double area;

        //public oItem(string data, int cod, int num_ordem, int qt,double x, double y, double a)
        public oItem(string data, string cod, int qt, double x, double y, double a, string mt)
        {
            geo = new System.Windows.Shapes.Path();
            GeometryConverter conv = new GeometryConverter();
            geo.Data = (Geometry)conv.ConvertFromString(data);
            geo.Fill = Brushes.DarkBlue;
            geo.Stroke = Brushes.Red;
            geo.StrokeThickness = 1;
            qtde = qt;
            item = cod;
            geo.Cursor = System.Windows.Input.Cursors.Hand;

            geo.Data = geo.Data.Clone();
            //ordem = num_ordem;
            dim_x = x;
            dim_y = y;
            area = a;
            mat = mt;
        }
        public oItem(Geometry data, string cod, int qt, double x, double y, double a, string mt)
        {
            geo = new System.Windows.Shapes.Path();
            geo.Data = data.Clone();
            geo.Fill = Brushes.DarkBlue;
            geo.Stroke = Brushes.Red;
            geo.StrokeThickness = 2;
            qtde = qt;
            item = cod;
            geo.Cursor = System.Windows.Input.Cursors.Hand;
            //ordem = num_ordem;
            dim_x = x;
            dim_y = y;
            area = a;
            mat = mt;
        }
        public System.Windows.Shapes.Path ClonePath(System.Windows.Shapes.Path antigo)
        {
            geo = new System.Windows.Shapes.Path();
            GeometryConverter conv = new GeometryConverter();
            geo.Data = antigo.Data;
            geo.Fill = Brushes.DarkBlue;
            geo.Stroke = Brushes.Red;
            geo.StrokeThickness = 1;
            geo.Cursor = System.Windows.Input.Cursors.Hand;
            return geo;
        }
        public oItem Clone(oItem antigo)
        {
            oItem it = new oItem(antigo.geo.Data, antigo.item, antigo.qtde, antigo.dim_x, antigo.dim_y, antigo.area, antigo.mat);

            return it;
        }
    }
}
