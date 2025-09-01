using AutoIt;
using AutomateClickerBrielina.Entidades;
using AutomateClickerBrielina.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomateClickerBrielina.Servico
{
    public class ExecucaoCliques
    {
        private Thread lacoCliques = null;
        private Thread execucaoCliques = null;
        public MainWindow MainWindow;

        public void Inicia(bool loop, CliquesControlador cliquesControlador)
        {
            lacoCliques = new Thread(async () =>
            {
                MainWindow.imprimeConsole($"Inicio Execução.");
                MainWindow.statusBtnsCliques(true);
                do
                {

                    if (loop)
                    {
                        int tempoSeguranca = 5;
                        MainWindow.imprimeConsole($"Pausa de segurança {tempoSeguranca}s");
                        Esperar(tempoSeguranca);
                    }

                    foreach (var clique in cliquesControlador.Cliques)
                    {
                        RealizaClique(clique);
                    }
                } while (loop);

                MainWindow.statusBtnsCliques(false);
                MainWindow.imprimeConsole($"Fim Execução.");
            });
            lacoCliques.SetApartmentState(ApartmentState.MTA);
            lacoCliques.Start();
        }

        public void Finaliza()
        {
            lacoCliques.Abort();
        }

        void RealizaClique(Clique clique)
        {
            bool sucesso = false;

            if (clique.PreSleep > 0)
            {
                MainWindow.imprimeConsole($"Aguardando {(clique.PreSleep)}s");
                Esperar(clique.PreSleep);
            }
            for (int i = 0; i < clique.qtdCliques; i++)
            {
                clique = ValidarCoordenadas(clique);
                int tentativas = 2;
                while (!clique.Sucesso && tentativas <= 10)
                {
                    MainWindow.imprimeConsole($"Clique {clique.Tipo} tentativa: {tentativas}");
                    clique = ValidarCoordenadas(clique);
                    tentativas++;
                }

                if (clique.Sucesso)
                {
                    ExecutarClique(clique);

                    MainWindow.imprimeConsole($"Clique {clique.Tipo} X:{clique.posX} Y:{clique.posY}");
                }
                else
                {
                    MainWindow.imprimeConsole($"Clique {clique.Tipo} nome:");
                    MainWindow.imprimeConsole($"    {clique.FileName}");
                    MainWindow.imprimeConsole($"    não encontrado.");
                }

                if (clique.TempoIntervalo > 0)
                {
                    MainWindow.imprimeConsole($"Aguardando {(clique.TempoIntervalo)}s");
                    Esperar(clique.TempoIntervalo);
                }
            }

            if (clique.PosSleep > 0)
            {
                MainWindow.imprimeConsole($"Aguardando {(clique.PosSleep)}s");
                Esperar(clique.PosSleep);
            }
        }

        Clique ValidarCoordenadas(Clique clique)
        {
            switch (clique.Tipo)
            {
                case Enums.TipoCliqueEnum.Posicional:
                    clique.Sucesso = true;
                    break;
                case Enums.TipoCliqueEnum.Imagem:
                    clique.Sucesso = false;
                    (bool Existe, int X, int Y) saida = CapturaTelas.ValidaImagem(clique.FileName);
                    clique.posX = saida.X;
                    clique.posY = saida.Y;
                    clique.Sucesso = saida.Existe;
                    break;
                default:
                    break;
            }
            return clique;
        }

        void ExecutarClique(Clique clique)
        {
            AutoItX.MouseClick("LEFT", clique.posX, clique.posY);
        }

        void teste()
        {
            var thread = new Thread(async () =>
            {
            });
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Start();

            thread.Abort();
        }

        public void Esperar(int segundos)
        {
            int tempo = segundos * 2;

            while (tempo > 0)
            {
                Thread.Sleep(500);
                tempo--;
            }
        }
    }
}
