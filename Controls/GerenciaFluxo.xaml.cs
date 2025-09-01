using AutomateClickerBrielina.Enums;
using AutomateClickerBrielina.Servico;
using AutomateClickerBrielina.Util;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AutomateClickerBrielina.Controls
{
    /// <summary>
    /// Lógica interna para GerenciaFluxo.xaml
    /// </summary>
    public partial class GerenciaFluxo : Window
    {
        CliquesControlador CliquesControlador;
        bool Navegando;

        public GerenciaFluxo()
        {
            InitializeComponent();

            CliquesControlador = MainWindow.CliquesControlador;
            inicializaDetalhesFluxo();
            Navegando = false;
        }

        void inicializaDetalhesFluxo()
        {
            PainelFluxo.Children.Clear();
            foreach (var clique in CliquesControlador.Cliques)
            {
                StackPanel painel = new StackPanel() { Orientation = Orientation.Horizontal };
                switch (clique.Tipo)
                {
                    case TipoCliqueEnum.Posicional:
                        painel.Children.Add(new Label() { Content = $"{clique.Tipo}: PosX:{clique.posX} PosY:{clique.posY} Qtd:{clique.qtdCliques}" });
                        break;
                    case TipoCliqueEnum.Imagem:
                        painel.Children.Add(new Label() { Content = $"{clique.Tipo}: {clique.FileName.Split('\\').LastOrDefault()}" });
                        break;
                }

                Button btnMove = new Button();
                btnMove.Content = "Move";
                btnMove.Click += (s, e) =>
                {
                    if (clique.Imagem)
                    {
                        (bool Existe, int X, int Y) saida = CapturaTelas.ValidaMoveImagem(clique.FileName);
                        if (saida.Existe)
                        {
                            clique.posX = saida.X;
                            clique.posY = saida.Y;
                        }
                        else
                        {
                            MessageBox.Show("Imagem não encontrada!");
                            return;
                        }
                    }

                    AutoIt.AutoItX.MouseMove(clique.posX, clique.posY);
                };

                painel.Children.Add(btnMove);

                Button btnRun = new Button();
                btnRun.Content = "Executar";
                btnRun.Click += (s, e) =>
                {
                    if (clique.Imagem)
                    {
                        (bool Existe, int X, int Y) saida = CapturaTelas.ValidaMoveImagem(clique.FileName);
                        if (saida.Existe)
                        {
                            clique.posX = saida.X;
                            clique.posY = saida.Y;
                        }
                        else
                        {
                            MessageBox.Show("Imagem não encontrada!");
                            return;
                        }
                    }

                    AutoIt.AutoItX.MouseMove(clique.posX, clique.posY);
                    AutoIt.AutoItX.MouseClick();
                };
                painel.Children.Add(btnRun);

                Button btnEditar = new Button();
                btnEditar.Content = "Editar";
                btnEditar.Click += (s, e) =>
                {
                    switch (clique.Tipo)
                    {
                        case TipoCliqueEnum.Posicional:
                            NavegarCliquesAdionador(new AdicionarCliquePosicional(FuncaoCrudCliqueEnum.Editar, clique));
                            break;
                        case TipoCliqueEnum.Imagem:
                            NavegarCliquesAdionador(new SalvarPrint(FuncaoCrudCliqueEnum.Editar, null, clique));
                            break;
                        default:
                            clique.validaTipo();
                            MessageBox.Show("Tipo ainda não definido, tentar novamente!");
                            break;
                    }
                };
                painel.Children.Add(btnEditar);

                Button btnExcluir = new Button();
                btnExcluir.Content = "Excluir";
                btnExcluir.Click += (s, e) =>
                {
                    CliquesControlador.Remove(clique);
                    inicializaDetalhesFluxo();
                };
                painel.Children.Add(btnExcluir);

                Button btnAdicionarCliqueAcima = new Button();
                btnAdicionarCliqueAcima.Content = "Adicionar Clique Acima";
                btnAdicionarCliqueAcima.Click += (s, e) =>
                {
                    NavegarCliquesAdionador(new AdicionarCliqueOpcoes(FuncaoCrudCliqueEnum.Up, clique));
                };
                painel.Children.Add(btnAdicionarCliqueAcima);

                Button btnAdicionarCliqueAbaixo = new Button();
                btnAdicionarCliqueAbaixo.Content = "Adicionar Clique Abaixo";
                btnAdicionarCliqueAbaixo.Click += (s, e) =>
                {
                    NavegarCliquesAdionador(new AdicionarCliqueOpcoes(FuncaoCrudCliqueEnum.Down, clique));
                };
                painel.Children.Add(btnAdicionarCliqueAbaixo);

                PainelFluxo.Children.Add(painel);
            }
        }

        private void AdicionarCliqueClick(object sender, RoutedEventArgs e)
        {
            NavegarCliquesAdionador(new AdicionarCliqueOpcoes(FuncaoCrudCliqueEnum.Adicionar));
        }

        void NavegarCliquesAdionador(Page page)
        {
            CliquesAdionador cliquesAdionador = new CliquesAdionador();
            cliquesAdionador.JanelaCliquesAdionador.Navigate(page);
            cliquesAdionador.Show();
            this.Close();
        }

        private void AdicionarCliquePosicionalClick(object sender, RoutedEventArgs e)
        {
            var janelaBase = new AdicionarCliquePosicional(FuncaoCrudCliqueEnum.Adicionar, null);
            Transparente novaJanelaTransparente = new Transparente(FuncaoCrudCliqueEnum.Adicionar ,TipoCliqueEnum.Posicional, janelaBase, null);
            novaJanelaTransparente.Show();
            this.Close();
        }

        private void AdicionarCliqueImagemClick(object sender, RoutedEventArgs e)
        {
            new Transparente(FuncaoCrudCliqueEnum.Adicionar, TipoCliqueEnum.Imagem, null).Show();
            this.Close();
        }
    }
}
