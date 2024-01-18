using AutomateClickerBrielina.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutomateClickerBrielina.Controls
{
    /// <summary>
    /// Lógica interna para Transparente.xaml
    /// </summary>
    public partial class Transparente : Window
    {
        private MainWindow JanelaPai;
        private bool isDragging = false;
        private System.Windows.Point startPoint;

        public Transparente(MainWindow _janelaPai, string Funcionalidade)
        {
            InitializeComponent();
            JanelaPai = _janelaPai;

            inicializaTela();
            switch (Funcionalidade)
            {
                case "Clique":
                    this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
                    break;
                case "Print":
                    this.MouseLeftButtonDown += OnMouseLeftButtonDown;
                    this.MouseLeftButtonUp += OnMouseLeftButtonUp;
                    break;
            }
        }

        void inicializaTela()
        {
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            Topmost = true;
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Verifica se o botão esquerdo do mouse foi clicado
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var valor = AutoIt.AutoItX.MouseGetPos();
                JanelaPai.PosXVal = valor.X;
                JanelaPai.PosYVal = valor.Y;
                if (!JanelaPai.SelecionarClique)
                {
                    JanelaPai.imprimeConsole($"Mouse pos: {valor}");
                }
                else
                {
                    JanelaPai.PosXlbl.Content = $"PosX: {valor.X}";
                    JanelaPai.PosYlbl.Content = $"PosY: {valor.Y}";
                    JanelaPai.CliqueQtdInput.Text = "1";
                    JanelaPai.CliquesintervaloInput.Text = "0";
                    JanelaPai.PreIntervaloInput.Text = "0";
                    JanelaPai.PosIntervaloInput.Text = "0";
                    JanelaPai.AdicionarCliqueBtnsPanel.IsEnabled = true;
                }
                this.Close();
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                isDragging = true;
                startPoint = e.GetPosition(this);
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {

                if (isDragging)
                {
                    isDragging = false;

                    // Exemplo: Obtendo coordenadas e tamanho após o arrasto
                    int left = int.Parse(startPoint.X.ToString());
                    int top = int.Parse(startPoint.Y.ToString());
                    System.Windows.Point finalPoint = e.GetPosition(this);
                    int width = int.Parse((finalPoint.X - startPoint.X).ToString());
                    int height = int.Parse((finalPoint.Y - startPoint.Y).ToString());

                    var print = CapturaTelas.CapturaSelecao(left, top, width, height);
                    this.Close();
                    new SalvarPrint(print).Show();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Erro ao capturar print; " + ex.Message);
                this.Close();
            }
        }
    }
}
