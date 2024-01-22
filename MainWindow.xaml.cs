using AutomateClickerBrielina.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;
using AutoIt;
using AutomateClickerBrielina.Entidades;
using AutomateClickerBrielina.Util;
using System.Runtime.InteropServices;

namespace AutomateClickerBrielina
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool CliqueSelecionado = false;
        private bool MonitoraJanelas = false;
        private string janelaAtiva = "";
        Task taskExecute;
        CancellationTokenSource cancellationTokenSource;

        public static List<Clique> Cliques;
        public int PosXVal = 0;
        public int PosYVal = 0;
        public bool SelecionarClique = false;

        public MainWindow()
        {
            InitializeComponent();
            inicializaTimer();
            janelaAtiva = AutoIt.AutoItX.WinGetTitle("[ACTIVE]");
            Cliques = new List<Clique>();
        }

        private async void btnInciarClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() => PararRobo());
            iniciaCliques(loop: false);
        }

        private void btnLoopClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() => PararRobo());
            iniciaCliques(loop: true);
        }

        void iniciaCliques(bool loop)
        {
            Console.Children.Clear();
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            taskExecute = Task.Run(() =>
            {
                imprimeConsole($"--CTRL + ENTER para Parar--");
                int numExecCount = 0;
                int NumCliquesCount = 0;
                int NumCliquesGeralCount = 0;
                alteraContadores(numExecCount, NumCliquesCount, NumCliquesGeralCount);

                try
                {
                    do
                    {

                        if (loop)
                        {
                            int tempoSeguranca = 5;
                            imprimeConsole($"Pausa de segurança {tempoSeguranca}s");
                            Esperar(tempoSeguranca);

                            numExecCount++;
                            NumCliquesCount = 0;
                            alteraContadores(numExecCount, NumCliquesCount, NumCliquesGeralCount);
                        }

                        foreach (var clique in Cliques)
                        {
                            if (clique.Imagem)
                            {
                                bool sucesso = false;
                                for (int i = 1; i < 10; i++)
                                {
                                    (bool Existe, int X, int Y) saida = CapturaTelas.ValidaMoveImagem(clique.FileName);
                                    if (saida.Existe)
                                    {
                                        clique.posX = saida.X;
                                        clique.posY = saida.Y;
                                        sucesso = true;
                                        break;
                                    }
                                    else
                                    {
                                        Thread.Sleep(1000);
                                    }
                                }

                                if (sucesso)
                                {
                                    AutoItX.MouseMove(clique.posX, clique.posY);
                                    AutoItX.MouseClick();

                                    NumCliquesCount++;
                                    NumCliquesGeralCount++;
                                    alteraContadores(numExecCount, NumCliquesCount, NumCliquesGeralCount);
                                }
                                else
                                {
                                    MessageBox.Show("Imagem não encontrada!");
                                }
                            }
                            else
                            {
                                if (clique.PreSleep > 0)
                                {
                                    imprimeConsole($"Aguardando {(clique.PreSleep)}s");
                                    Esperar(clique.PreSleep);
                                }

                                for (int i = 0; i < clique.qtdCliques; i++)
                                {
                                    clickTaskValida(clique.posX, clique.posY, !token.IsCancellationRequested);

                                    NumCliquesCount++;
                                    NumCliquesGeralCount++;

                                    alteraContadores(numExecCount, NumCliquesCount, NumCliquesGeralCount);

                                    imprimeConsole($"Clique X:{clique.posX} Y:{clique.posY}");
                                    imprimeConsole($"Aguardando {(clique.TempoIntervalo)}s");
                                    Esperar(clique.TempoIntervalo);
                                }

                                if (clique.PosSleep > 0)
                                {
                                    imprimeConsole($"Aguardando {(clique.PosSleep)}s");
                                    Esperar(clique.PosSleep);
                                }
                            }
                        }

                    } while (loop && !token.IsCancellationRequested);

                    imprimeConsole($"FIM");
                }
                catch
                {
                    imprimeConsole("Loop Encerrado!");
                }
            }, token);
        }

        void clickTaskValida(int posX, int posY, bool continua)
        {
            if (continua)
            {
                AutoItX.MouseClick("LEFT", posX, posY);
            }
            else
            {
                throw new OperationCanceledException();
            }
        }

        private void btnNomeJanelaClick(object sender, RoutedEventArgs e)
        {
            MonitoraJanelas = !MonitoraJanelas;
            AvisoLbl.Visibility = MonitoraJanelas ? Visibility.Visible : Visibility.Hidden;
            AvisoLbl.Content = "Monitora Janelas";

            if (MonitoraJanelas)
            {
                Util.Timer.Inicia();
            }
            else
            {
                Util.Timer.Para();
            }
        }

        public async void imprimeConsole(string message)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Console.Children.Add(new Label() { Content = message, Foreground = new SolidColorBrush(Colors.White) });
                Util.Timer.Para();
                ScrollConsole.ScrollToEnd();
            }));
        }

        public async void alteraContadores(int Exec, int Clique, int CliqueGeral)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                NumCliquesGeral.Content = $"Cliques Geral: {Exec}";
                NumCliques.Content = $"Cliques: {Clique}";
                NumExec.Content = $"Loops: {CliqueGeral}";
            }));
        }

        void inicializaTimer()
        {
            Util.Timer.Tick += () =>
            {
                string janelaAtivaAtual = AutoIt.AutoItX.WinGetTitle("[ACTIVE]");
                if (janelaAtiva != janelaAtivaAtual)
                {
                    janelaAtiva = janelaAtivaAtual;
                    imprimeConsole($"Janela Selecionada: {janelaAtiva}");
                }
            };
        }

        private void btnGerenciarClick(object sender, RoutedEventArgs e)
        {
            new GerenciaFluxo(Cliques).Show();
        }

        private void btnStopClick(object sender, RoutedEventArgs e)
        {
            if (taskExecute != null)
            {
                cancellationTokenSource.Cancel();
                imprimeConsole("Comando enviado, necessario esperar");
                imprimeConsole("o tempo de intervalo.");
            }
        }

        private void btnSavesClick(object sender, RoutedEventArgs e)
        {
            ViewerSaves.Children.Clear();
            SavesModal.Visibility = Visibility.Visible;
            SavesModalFundo.Visibility = Visibility.Visible;
            QtdCliquesInputName.Content = $"{Cliques.Count} Cliques";

            List<Save> Saves = JsonUtil.CarregaJsons();
            foreach (var save in Saves)
            {
                StackPanel painel = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Height = 30,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(5)
                };
                Label saveName = new Label() { Content = save.Nome };
                Label saveQtdCliques = new Label() { Content = save.Sequencia.Count + " Cliques" };
                Button btnCarregar = new Button() { Content = "Carregar" };
                btnCarregar.Click += (sd, ev) =>
                {
                    Cliques = save.Sequencia;
                    MessageBox.Show($"Save {save.Nome} carregado.");
                    SavesModal.Visibility = Visibility.Hidden;
                    SavesModalFundo.Visibility = Visibility.Hidden;
                };

                Button btnExcluir = new Button() { Content = "Excluir" };
                btnExcluir.Click += (sd, ev) =>
                {
                    Saves.Remove(save);
                    Util.JsonUtil.ModifiarJson(Saves);
                    MessageBox.Show($"Save {save.Nome} Excluido.");
                    SavesModal.Visibility = Visibility.Hidden;
                    SavesModalFundo.Visibility = Visibility.Hidden;
                };

                painel.Children.Add(saveName);
                painel.Children.Add(saveQtdCliques);
                painel.Children.Add(btnCarregar);
                painel.Children.Add(btnExcluir);
                ViewerSaves.Children.Add(painel);
            }
        }

        private void btnModalSalvarClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NovoSaveInputName.Text) && Cliques.Count > 0)
            {
                List<Save> Saves = JsonUtil.CarregaJsons();
                Saves.Add(new Save() { Nome = NovoSaveInputName.Text, Sequencia = Cliques });
                Util.JsonUtil.ModifiarJson(Saves);
                NovoSaveInputName.Text = "";

                SavesModal.Visibility = Visibility.Hidden;
                SavesModalFundo.Visibility = Visibility.Hidden;
            }
            else
            {
                MessageBox.Show($"Nome Obrigatorio e Cliques precisam ser maior que 0.");
            }
        }

        private void btnModalFecharSaves(object sender, RoutedEventArgs e)
        {
            SavesModal.Visibility = Visibility.Hidden;
            SavesModalFundo.Visibility = Visibility.Hidden;
        }

        private void btnTesteClick(object sender, RoutedEventArgs e)
        {
            var teste = CapturaTelas.ListaNomesPrints();
            (bool Existe, int X, int Y) saida = CapturaTelas.ValidaMoveImagem(teste.LastOrDefault());
            if (saida.Existe)
            {
                AutoItX.MouseMove(saida.X, saida.Y);
                AutoItX.MouseClick("Left");
                AutoItX.MouseClick("Left");
                imprimeConsole($"Print encontrado " +
                                $"\nclique realizado em X:{saida.X} Y:{saida.Y}");
            }
            else
            {
                imprimeConsole("Print não encontrado");
            }
        }

        async Task PararRobo()
        {
            while (!IsKeyPressed(VirtualKeyCode.CONTROL) || !IsKeyPressed(VirtualKeyCode.RETURN))
            {
                //travaaq
            }


            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();
            MessageBox.Show("Cancelamento solicitado");
        }

        static bool IsKeyPressed(VirtualKeyCode keyCode)
        {
            return (GetAsyncKeyState(keyCode) & 0x8000) != 0;
        }

        public void Esperar(int segundos)
        {
            int tempo = segundos * 2;

            while (tempo > 0 && !cancellationTokenSource.IsCancellationRequested)
            {
                Thread.Sleep(500);
                tempo--;
            }
        }

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(VirtualKeyCode vKey);

        public enum VirtualKeyCode : int
        {
            CONTROL = 0x11,
            RETURN = 0x0D,
        }

        //Em Desenvolvimento
        void novoMetodoFluxo()
        {
            Fluxo fluxo = new Fluxo();
            fluxo.AdicionaAcao(this, "Teste1", 300, 300);

            Laco c2 = new Laco() { Repeticoes = 5 };
            c2.AdicionaAcao(this, "Teste2", 350, 350);

            fluxo.AdicionaLaco(c2);

            fluxo.AdicionaAcao(this, "Teste3", 300, 300);

            fluxo.Run();
        }
    }
}
