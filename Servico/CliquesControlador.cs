using AutomateClickerBrielina.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateClickerBrielina.Servico
{
    public class CliquesControlador
    {
        public List<Clique> Cliques;

        public CliquesControlador()
        {
            Cliques = new List<Clique>();
        }

        public void Remove(Clique clique)
        {
            Cliques.Remove(clique);
        }

        internal void Add(Clique clique)
        {
            Clique ultimoClique = Cliques.LastOrDefault();
            if (ultimoClique != null)
                clique.Id = ultimoClique.Id + 1;
            else
                clique.Id = 0;

            Cliques.Add(clique);
        }

        public void Edit(Clique clique)
        {
            var cliqueAtual = Cliques.FirstOrDefault(x => x.Id == clique.Id);
            if (cliqueAtual != null)
            {
                cliqueAtual.Tipo = clique.Tipo;
                cliqueAtual.posX = clique.posX;
                cliqueAtual.posY = clique.posY;
                cliqueAtual.PreSleep = clique.PreSleep;
                cliqueAtual.PosSleep = clique.PosSleep;
                cliqueAtual.qtdCliques = clique.qtdCliques;
                cliqueAtual.TempoIntervalo = clique.TempoIntervalo;
                cliqueAtual.FileName = clique.FileName;
                cliqueAtual.Imagem = clique.Imagem;
            }
        }

        public void AddUp(Clique refClique, Clique newClique)
        {
            List<Clique> newListCliques = new List<Clique>();
            for (int i = 0; i < Cliques.Count; i++)
            {
                if (refClique == Cliques[i])
                    newListCliques.Add(newClique);

                newListCliques.Add(Cliques[i]);
            }

            Cliques.Clear();
            Cliques = newListCliques;
        }

        public void AddDown(Clique refClique, Clique newClique)
        {
            List<Clique> newListCliques = new List<Clique>();
            for (int i = 0; i < Cliques.Count; i++)
            {
                newListCliques.Add(Cliques[i]);

                if (refClique == Cliques[i])
                    newListCliques.Add(newClique);
            }

            Cliques.Clear();
            Cliques = newListCliques;   
        }
    }
}
