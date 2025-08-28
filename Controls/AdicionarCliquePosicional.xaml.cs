using AutomateClickerBrielina.Entidades;
using AutomateClickerBrielina.Enums;
using AutomateClickerBrielina.Servico;
using Microsoft.SqlServer.Server;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutomateClickerBrielina.Controls
{
    /// <summary>
    /// Lógica interna para AdicionarCliquePosicional.xaml
    /// </summary>
    public partial class AdicionarCliquePosicional : Page
    {
        public int PosXVal = 0;
        public int PosYVal = 0;
        public bool SelecionarClique = false;
        public Clique _clique = null;

        private Clique oldClique = null;
        private bool CliqueSelecionado = false;
        private CliquesControlador CliquesControlador;
        private bool Navegando;
        private FuncaoCrudCliqueEnum _funcaoCrudCliqueEnum;


        public AdicionarCliquePosicional(FuncaoCrudCliqueEnum funcaoCrudCliqueEnum, Clique clique)
        {
            InitializeComponent();
            CliquesControlador = MainWindow.CliquesControlador;
            _funcaoCrudCliqueEnum = funcaoCrudCliqueEnum;
            oldClique = clique;

            if (funcaoCrudCliqueEnum == FuncaoCrudCliqueEnum.Editar)
            {
                _clique = clique;
                preencheCamposDados();
                btnConcluir.Content = "Editar";
                btnConcluir.Click += (s, e) => EditarCliqueClick(s, e);
            }
            else if(funcaoCrudCliqueEnum == FuncaoCrudCliqueEnum.Adicionar 
                || funcaoCrudCliqueEnum == FuncaoCrudCliqueEnum.Up
                || funcaoCrudCliqueEnum == FuncaoCrudCliqueEnum.Down)
            {
                btnConcluir.Click += (s, e) => AdicionarCliqueClick(s, e);
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
                AdicionarClick();

                CliqueQtdInput.Text = string.Empty;
                CliquesintervaloInput.Text = string.Empty;
                PreIntervaloInput.Text = string.Empty;
                PosIntervaloInput.Text = string.Empty;
                CliqueSelecionado = false;
            }

            (Window.GetWindow(this) as CliquesAdionador).Fechar();
        }

        private void EditarCliqueClick(object sender, RoutedEventArgs e)
        {
            Clique newCliqueLocal = new Clique()
            {
                Id = _clique.Id,
                Tipo = TipoCliqueEnum.Posicional,
                posX = PosXVal,
                posY = PosYVal,
                qtdCliques = int.Parse(CliqueQtdInput.Text),
                TempoIntervalo = int.Parse(CliquesintervaloInput.Text),
                PreSleep = int.Parse(PreIntervaloInput.Text),
                PosSleep = int.Parse(PosIntervaloInput.Text)
            };
            CliquesControlador.Edit(newCliqueLocal);

            (Window.GetWindow(this) as CliquesAdionador).Fechar();
        }

        private void SelecionarCliqueClick(object sender, RoutedEventArgs e)
        {
            SelecionarClique = true;
            Transparente novaJanelaTransparente = new Transparente(FuncaoCrudCliqueEnum.Adicionar ,TipoCliqueEnum.Posicional, this, _clique);
            novaJanelaTransparente.Show();
            CliqueSelecionado = true;

            AdicionarCliqueBtnsPanel.IsEnabled = false;
        }

        private void AdicionarClick()
        {
            Clique newCliqueLocal = new Clique()
            {
                Tipo = TipoCliqueEnum.Posicional,
                posX = PosXVal,
                posY = PosYVal,
                qtdCliques = int.Parse(CliqueQtdInput.Text),
                TempoIntervalo = int.Parse(CliquesintervaloInput.Text),
                PreSleep = int.Parse(PreIntervaloInput.Text),
                PosSleep = int.Parse(PosIntervaloInput.Text)
            };

            switch (_funcaoCrudCliqueEnum)
            {
                case FuncaoCrudCliqueEnum.Adicionar:
                    CliquesControlador.Add(newCliqueLocal);
                    break;
                case FuncaoCrudCliqueEnum.Up:
                    CliquesControlador.AddUp(oldClique, newCliqueLocal);
                    break;
                case FuncaoCrudCliqueEnum.Down:
                    CliquesControlador.AddDown(oldClique, newCliqueLocal);
                    break;
            }
        }

        public void preencheCamposDados()
        {
            PosXVal = _clique.posX;
            PosYVal = _clique.posY;
            PosXlbl.Content = $"PosX: {_clique.posX}";
            PosYlbl.Content = $"PosY: {_clique.posY}";
            CliqueQtdInput.Text = _clique.qtdCliques.ToString();
            CliquesintervaloInput.Text = _clique.TempoIntervalo.ToString();
            PreIntervaloInput.Text = _clique.PreSleep.ToString();
            PosIntervaloInput.Text = _clique.PosSleep.ToString();
        }
    }
}
