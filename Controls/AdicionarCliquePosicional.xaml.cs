using AutomateClickerBrielina.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace AutomateClickerBrielina.Controls
{
    /// <summary>
    /// Lógica interna para AdicionarCliquePosicional.xaml
    /// </summary>
    public partial class AdicionarCliquePosicional : Window
    {
        public int PosXVal = 0;
        public int PosYVal = 0;
        public bool SelecionarClique = false;

        private bool CliqueSelecionado = false;
        private List<Clique> Cliques;
        private bool Navegando;

        public AdicionarCliquePosicional(List<Clique> cliques)
        {
            InitializeComponent();
            Cliques = cliques;
            Closing += AoFechar;
        }

        private void AoFechar(object sender, CancelEventArgs e)
        {
            new GerenciaFluxo(Cliques).Show();
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
    }
}
