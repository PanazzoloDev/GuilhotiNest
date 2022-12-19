using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GuilhotiNest.ViewModel
{
    [Serializable()]
    public static class Controle
    {
        public static ObservableCollection<object> Tree { get; set; } = new ObservableCollection<object>();
        public static ObservableCollection<Grupo> Grupos { get; set; } = new ObservableCollection<Grupo>();
        public static ObservableCollection<Tarefa> Tarefas { get; set; } = new ObservableCollection<Tarefa>();
        public static ObservableCollection<Document> Documentos { get; set; } = new ObservableCollection<Document>();

        public static Layout Layout_Ativo { get; set; }
        public static Occurrences ActiveOcc { get; set; }
        public static Canvas Design { get; set; }
        
        //Comandos de Criação
        public static void Importar_Inventor(string caminho, TreeView tree)
        {
            InventorApp Inv = new InventorApp();

            //Documents é populado
            List<Document> xDocs = new List<Document>();
            if (caminho.Contains(".ipt"))
            {
                
                Document doc = Inv.Importar_ipt(caminho);
                if (doc != null) xDocs.Add(doc);
                Documentos.Add(doc);
            }
            else
            {
                Inv.Importar_iam(caminho,ref xDocs);
                foreach (Document d in xDocs) { Documentos.Add(d); }
            }
            //Se não existem grupos criados
            var Grupo_Inventor = xDocs.GroupBy(x => new { x.Material, x.Espessura }).ToList();
            //Para cada Material-Espessura recebida do Inventor verifica se ja é existente.
            foreach (var MatEsp in Grupo_Inventor)
            {
                if(Grupos == null || Grupos.Count == 0)
                {
                    Criar_Grupo(MatEsp.Key.Material, MatEsp.Key.Espessura, MatEsp.ToList());
                }
                else
                {
                    Grupo grp;
                    try { grp = (from x in Grupos where x.Espessura == MatEsp.Key.Espessura && x.Material == MatEsp.Key.Material select x).First(); } catch { grp = null; };
                    //Se não existe ainda esse Material-Espessura.
                    if (grp == null)
                    {
                        Criar_Grupo(MatEsp.Key.Material, MatEsp.Key.Espessura, MatEsp.ToList());
                    }
                    //Se existe e o mesmo foi encontrado.
                    else
                    {
                        Inserir_Grupo(grp, MatEsp.ToList());
                    }
                }

            }
        }
        public static void Salvar_WorksSpace()
        {
            string destino = @"C:\Users\pcp02\Desktop\Importações\Workspace.bin";
            Stream SaveFileStream = File.Create(destino);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(SaveFileStream, Tree);
            SaveFileStream.Close();
        }
        public static void AbrirWorkspace(string caminho)
        {
            Stream openFileStream = File.OpenRead(caminho);
            BinaryFormatter deserializer = new BinaryFormatter();
            Tree = (ObservableCollection<object>)deserializer.Deserialize(openFileStream);
            foreach (var obj in Tree)
            {
                if (obj.GetType() == typeof(Grupo))
                {
                    Grupos.Add((Grupo)obj);
                    foreach (var doc in ((Grupo)obj).Documents)
                    {
                        Documentos.Add(doc);
                        doc.Criar_Thumbnail();
                    }
                }
                if (obj.GetType() == typeof(Tarefa)) { Tarefas.Add((Tarefa)obj); }

            }
            openFileStream.Close();
        }
        public static Layout NovoLayout_Retangular(double Alt_mm, double Comp_mm, Tarefa Parent)
        {
            string cmd = string.Format("M 0 0 h{0} v{1} h-{0}", Comp_mm, Alt_mm);
            Geometry geo = Geometry.Parse(cmd).Clone();
            double esc = (new[] { ((Design.ActualWidth * 0.95) / geo.Bounds.Width), ((Design.ActualHeight * 0.8) / geo.Bounds.Height) }).Min();

            double Y = (Design.ActualHeight * 0.1);
            double X = (Design.ActualWidth * 0.025);

            System.Windows.Point Posic = new System.Windows.Point(X, Y);
            return new Layout(01, esc, geo, Parent, Posic,1);
        }

        //Comandos de Alteração
        public static void Arranjar()
        {
            ActiveOcc.Limites = ActiveOcc.Design.Data.GetRenderBounds(new Pen());
            Layout_Ativo.Retalho = Geometry.Combine(Layout_Ativo.Retalho, ActiveOcc.Design.Data, GeometryCombineMode.Exclude, null);
            List<Cortes> list = new List<Cortes>()
            {
                new Cortes(ActiveOcc.Design.Data.Bounds.Right, Cortes.Orientacao.Vertical, Layout_Ativo),
                new Cortes(ActiveOcc.Design.Data.Bounds.Top, Cortes.Orientacao.Horizontal, Layout_Ativo)
            };
            foreach (var cor in list)
            {
                Design.Children.Add(cor.Design);
                Layout_Ativo.Limites.Add(cor);
                ActiveOcc.Cortes.Add(cor);
            }
            ActiveOcc = null;
        }
        public static void StartMove(Occurrences Occ)
        {
            ActiveOcc = Occ;
            Layout_Ativo.Retalho = Geometry.Combine(Layout_Ativo.Retalho, ActiveOcc.Design.Data, GeometryCombineMode.Union, null);
            foreach (var cor in ActiveOcc.Cortes)
            {
                Design.Children.Remove(cor.Design);
                Layout_Ativo.Limites.Remove(cor);
            }
            ActiveOcc.Cortes.Clear();
        }
        public static void DeleteOcc()
        {
            Layout_Ativo.Retalho = Geometry.Combine(Layout_Ativo.Retalho, ActiveOcc.Design.Data, GeometryCombineMode.Union, null);
            Design.Children.Remove(ActiveOcc.Design);
            foreach (var cor in ActiveOcc.Cortes)
            {
                Design.Children.Remove(cor.Design);
                Layout_Ativo.Limites.Remove(cor);
            }
            ActiveOcc.Cortes.Clear();
            Layout_Ativo.Occs.Remove(ActiveOcc);
            ActiveOcc.Parent.Occs.Remove(ActiveOcc);
        }

        private static void Criar_Grupo(string material, double esp, List<Document> itens)
        {
            Grupo grupo = new Grupo(material, esp);
            foreach (var item in itens)
            {
                grupo.Documents.Add(item);
            }
            Tree.Add(grupo);
            Grupos.Add(grupo);
        }
        private static void Inserir_Grupo(Grupo grp, List<Document> itens)
        {
            foreach (var item in itens)
            {
                grp.Documents.Add(item);
            }
        }
    }
}

