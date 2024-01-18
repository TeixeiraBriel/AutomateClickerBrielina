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
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;

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
        private List<Clique> Cliques;
        Task taskExecute;
        CancellationTokenSource cancellationTokenSource;

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
            iniciaCliques(loop: false);
        }

        private void btnLoopClick(object sender, RoutedEventArgs e)
        {
            iniciaCliques(loop: true);
        }

        void iniciaCliques(bool loop)
        {

            if (AdicionarCliquePanel.Visibility != Visibility.Visible)
            {
                return;
            }

            AdicionarCliquePanel.Visibility = Visibility.Hidden;
            Console.Children.Clear();
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            taskExecute = Task.Run(() =>
            {
                try
                {
                    do
                    {
                        if (loop)
                        {
                            imprimeConsole($"Pausa de segurança 15s");
                            Thread.Sleep(15 * 1000);
                        }

                        foreach (var clique in Cliques)
                        {
                            if (clique.PreSleep > 0)
                            {
                                imprimeConsole($"Aguardando {(clique.PreSleep)}s");
                                Thread.Sleep(clique.PreSleep * 1000);
                            }

                            for (int i = 0; i < clique.qtdCliques; i++)
                            {
                                clickTaskValida(clique.posX, clique.posY, !token.IsCancellationRequested);
                                imprimeConsole($"Clique X:{clique.posX} Y:{clique.posY}");
                                imprimeConsole($"Aguardando {(clique.TempoIntervalo)}s");
                                Thread.Sleep(clique.TempoIntervalo * 1000);
                            }

                            if (clique.PosSleep > 0)
                            {
                                imprimeConsole($"Aguardando {(clique.PosSleep)}s");
                                Thread.Sleep(clique.PosSleep * 1000);
                            }
                        }

                    } while (loop && !token.IsCancellationRequested);

                    imprimeConsole($"FIM");
                    Dispatcher.Invoke(new Action(() => { AdicionarCliquePanel.Visibility = Visibility.Visible; }));
                }
                catch
                {
                    Dispatcher.Invoke(new Action(() => { AdicionarCliquePanel.Visibility = Visibility.Visible; }));
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

        private void btnPosMouseClick(object sender, RoutedEventArgs e)
        {
            Transparente novaJanelaTransparente = new Transparente(this, "Clique");
            novaJanelaTransparente.Show();
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
            Console.Children.Clear();
            foreach (var clique in Cliques)
            {
                if (clique.PreSleep > 0)
                    imprimeConsole($"Aguardar {clique.PreSleep}");

                imprimeConsole($"Clica {clique.qtdCliques} vezes");
                imprimeConsole($"PosX:{clique.posX} PosY:{clique.posY}");
                imprimeConsole($"Com intervalo de {clique.TempoIntervalo}");

                if (clique.PosSleep > 0)
                    imprimeConsole($"Aguardar {clique.PosSleep}");
            }
        }
        private void NumerosTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void AdicionarCliqueClick(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(CliqueQtdInput.Text) &&
                !string.IsNullOrEmpty(CliquesintervaloInput.Text) &&
                !string.IsNullOrEmpty(PreIntervaloInput.Text) &&
                !string.IsNullOrEmpty(PosIntervaloInput.Text) &&
                CliqueSelecionado)
            {
                Cliques.Add(new Clique()
                {
                    posX = PosXVal,
                    posY = PosYVal,
                    qtdCliques = int.Parse(CliqueQtdInput.Text),
                    TempoIntervalo = int.Parse(CliquesintervaloInput.Text),
                    PreSleep = int.Parse(PreIntervaloInput.Text),
                    PosSleep = int.Parse(PosIntervaloInput.Text)
                });

                CliqueQtdInput.Text = string.Empty;
                CliquesintervaloInput.Text = string.Empty;
                PreIntervaloInput.Text = string.Empty;
                PosIntervaloInput.Text = string.Empty;
                CliqueSelecionado = false;
            }
        }

        private void SelecionarCliqueClick(object sender, RoutedEventArgs e)
        {
            SelecionarClique = true;
            Transparente novaJanelaTransparente = new Transparente(this, "Clique");
            novaJanelaTransparente.Show();
            CliqueSelecionado = true;

            AdicionarCliqueBtnsPanel.IsEnabled = false;
        }

        private void btnRemoverUltimoClick(object sender, RoutedEventArgs e)
        {
            if (Cliques.Count > 0)
                Cliques.Remove(Cliques.Last());
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
            Transparente novaJanelaTransparente = new Transparente(this, "Print");
            novaJanelaTransparente.Show();
        }

        private void btnTesteClick_old(object sender, RoutedEventArgs e)
        {
            Fluxo fluxo = new Fluxo();

            Acao c1 = new Acao()
            {
                action = new Action(() =>
                {
                    imprimeConsole("c1");
                })
            };

            Laco c2 = new Laco();
            c2.Acoes.Add(new Acao() { action = new Action(() => imprimeConsole("c2a")) });
            c2.Repeticoes = 5;

            Acao c3 = new Acao()
            {
                action = new Action(() =>
                {
                    imprimeConsole("c3");
                })
            };

            fluxo.comandos.Add(c1);
            fluxo.comandos.Add(c2);
            fluxo.comandos.Add(c3);

            foreach (var cmd in fluxo.comandos)
            {
                int repeticoes = cmd.Tipo == 0 ? 1 : (cmd as Laco).Repeticoes;
                for (int i = 0; i < repeticoes; i++)
                {
                    if (repeticoes > 1)
                    {
                        foreach (Acao acao in (cmd as Laco).Acoes)
                        {
                            acao.executar();
                        }
                    }
                    else
                    {
                        (cmd as Acao).executar();
                    }
                }
            }
        }
    }
}
