using AutomateClickerBrielina.Entidades;
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
            Closing += AoFechar;
            Navegando = false;
        }

        private void AoFechar(object sender, CancelEventArgs e)
        {
            if (!Navegando)
            {
                new MainWindow().Show();
            }
        }

        void inicializaDetalhesFluxo()
        {
            PainelFluxo.Children.Clear();
            foreach (var clique in Cliques)
            {
                StackPanel painel = new StackPanel() { Orientation = Orientation.Horizontal };
                painel.Children.Add(new Label() { Content = $"PosX:{clique.posX} PosY:{clique.posY} Qtd:{clique.qtdCliques}" });

                Button btnMove = new Button();
                btnMove.Content = "Move";
                btnMove.Click += (s, e) =>
                {
                    AutoIt.AutoItX.MouseMove(clique.posX, clique.posY);
                };
                painel.Children.Add(btnMove);

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
            Navegando = true;
            this.Close();
        }
    }
}
