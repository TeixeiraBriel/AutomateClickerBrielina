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
using AutomateClickerBrielina.Servico;

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
        private ExecucaoCliques execucaoCliques;
        private bool cancelamentoSolicitado = false;
        Task taskExecute;
        CancellationTokenSource cancellationTokenSource;

        public static CliquesControlador CliquesControlador;
        public int PosXVal = 0;
        public int PosYVal = 0;
        public bool SelecionarClique = false;

        public MainWindow()
        {
            InitializeComponent();
            janelaAtiva = AutoIt.AutoItX.WinGetTitle("[ACTIVE]");
            CliquesControlador = new CliquesControlador();
        }

        #region Botões Cliques
        private async void btnInciarClick(object sender, RoutedEventArgs e)
        {
            iniciaCliques(false);
        }

        private void btnLoopClick(object sender, RoutedEventArgs e)
        {
            iniciaCliques(true);
        }

        private void btnGerenciarClick(object sender, RoutedEventArgs e)
        {
            new GerenciaFluxo().Show();
        }
        private void btnStopClick(object sender, RoutedEventArgs e)
        {
            cancelamentoSolicitado = true;
        }

        private void btnSavesClick(object sender, RoutedEventArgs e)
        {
            ViewerSaves.Children.Clear();
            SavesModal.Visibility = Visibility.Visible;
            SavesModalFundo.Visibility = Visibility.Visible;
            QtdCliquesInputName.Content = $"{CliquesControlador.Cliques.Count} Cliques";

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
                    CliquesControlador.Cliques = save.Sequencia;
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
            if (!string.IsNullOrEmpty(NovoSaveInputName.Text) && CliquesControlador.Cliques.Count > 0)
            {
                List<Save> Saves = JsonUtil.CarregaJsons();
                Saves.Add(new Save() { Nome = NovoSaveInputName.Text, Sequencia = CliquesControlador.Cliques });
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
            (bool Existe, int X, int Y) saida = CapturaTelas.ValidaImagem(teste.LastOrDefault());
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
        #endregion

        void iniciaCliques(bool loop)
        {
            cancelamentoSolicitado = false;
            Console.Children.Clear();
            imprimeConsole($"--CTRL + ENTER para Parar--");
            execucaoCliques = new ExecucaoCliques() { MainWindow = this };
            execucaoCliques.Inicia(loop, CliquesControlador);
            Task.Run(() => PararRobo());
        }

        async Task PararRobo()
        {
            while ((!IsKeyPressed(VirtualKeyCode.CONTROL) || !IsKeyPressed(VirtualKeyCode.RETURN)) && !cancelamentoSolicitado)
            {
                //travaaq
            }

            if (execucaoCliques != null)
            {
                execucaoCliques.Finaliza();
                imprimeConsole("Comando finalizar enviado.");
            }
            cancelamentoSolicitado = false;
            statusBtnsCliques(false);
        }

        public void statusBtnsCliques(bool Executando)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (Executando)
                {
                    btnIniciar.IsEnabled = false;
                    btnLoop.IsEnabled = false;
                    btnStop.IsEnabled = true;
                }
                else
                {
                    btnIniciar.IsEnabled = true;
                    btnLoop.IsEnabled = true;
                    btnStop.IsEnabled = false;
                }
            }));
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

        #region Validação teclas para Atalhos
        static bool IsKeyPressed(VirtualKeyCode keyCode)
        {
            return (GetAsyncKeyState(keyCode) & 0x8000) != 0;
        }

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(VirtualKeyCode vKey);

        public enum VirtualKeyCode : int
        {
            CONTROL = 0x11,
            RETURN = 0x0D,
        }
        #endregion
    }
}
