using System;
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
    public static class Controle
    {
        public static List<object> Tree { get; set; } = new List<object>();
        public static ObservableCollection<Grupo> Grupos { get; set; } = new ObservableCollection<Grupo>();
        public static ObservableCollection<Tarefa> Tarefas { get; set; } = new ObservableCollection<Tarefa>();
        public static ObservableCollection<Document> Documentos { get; set; } = new ObservableCollection<Document>();

        public static Layout Layout_Ativo { get; set; }
        public static Occurrences ActiveOcc { get; set; }
        public static Canvas Design { get; set; }

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
        private static void Criar_Grupo(string material, double esp, List<Document> itens)
        {
            Grupo grupo = new Grupo(material,esp);
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
    }
}

