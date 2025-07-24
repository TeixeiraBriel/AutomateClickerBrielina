using AutomateClickerBrielina.Entidades;
using AutomateClickerBrielina.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutomateClickerBrielina.Controls
{
    /// <summary>
    /// Lógica interna para GerenciaFluxo.xaml
    /// </summary>
    public partial class GerenciaFluxo : Window
    {
        List<Clique> Cliques;
        bool Navegando;

        public GerenciaFluxo(List<Clique> cliques)
        {
            InitializeComponent();

            Cliques = cliques;
            inicializaDetalhesFluxo();
            Navegando = false;
        }

        void inicializaDetalhesFluxo()
        {
            PainelFluxo.Children.Clear();
            foreach (var clique in Cliques)
            {
                StackPanel painel = new StackPanel() { Orientation = Orientation.Horizontal };
                string texto = clique.Imagem ? clique.FileName.Split('\\').Last() : $"PosX:{clique.posX} PosY:{clique.posY} Qtd:{clique.qtdCliques}";
                painel.Children.Add(new Label() { Content =  texto});

                Button btnMove = new Button();
                btnMove.Content = "Move";
                btnMove.Click += (s, e) =>
                {
                    if (clique.Imagem)
                    {
                        (bool Existe, int X, int Y) saida = CapturaTelas.ValidaImagem(clique.FileName);
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
                        (bool Existe, int X, int Y) saida = CapturaTelas.ValidaImagem(clique.FileName);
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

                Button btnExcluir = new Button();
                btnExcluir.Content = "Excluir";
                btnExcluir.Click += (s, e) =>
                {
                    Cliques.Remove(clique);
                    inicializaDetalhesFluxo();
                };
                painel.Children.Add(btnExcluir);

                PainelFluxo.Children.Add(painel);
            }
        }

        private void AdicionarPosicionalClick(object sender, RoutedEventArgs e)
        {
            new AdicionarCliquePosicional(Cliques).Show();
            this.Close();
        }

        private void AdicionarImagemClick(object sender, RoutedEventArgs e)
        {
            new Transparente(this, "Print").Show();
            this.Close();
        }
    }
}
