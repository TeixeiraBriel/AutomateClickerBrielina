using AutomateClickerBrielina.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace AutomateClickerBrielina.Controls
{
    /// <summary>
    /// Lógica interna para Transparente.xaml
    /// </summary>
    public partial class Transparente : Window
    {
        private AdicionarCliquePosicional JanelaPaiPosicional;
        private GerenciaFluxo JanelaPaiPrint;
        private bool isDragging = false;
        private System.Windows.Point startPoint;
        private Bitmap PrintTela;

        public Transparente(Window _janelaPai, string Funcionalidade)
        {
            InitializeComponent();

            inicializaTela();
            switch (Funcionalidade)
            {
                case "Clique":
                    this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
                    Opacity = 0.1;
                    imagePanel.Source = null;
                    JanelaPaiPosicional = _janelaPai as AdicionarCliquePosicional;
                    break;
                case "Print":
                    _janelaPai.WindowState = WindowState.Minimized;
                    this.MouseLeftButtonDown += OnMouseLeftButtonDown;
                    this.MouseLeftButtonUp += OnMouseLeftButtonUp;
                    inicializaTelaPrint();
                    JanelaPaiPrint = _janelaPai as GerenciaFluxo;
                    break;
            }
        }

        void inicializaTela()
        {
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            Topmost = true;
        }

        void inicializaTelaPrint()
        {
            Bitmap bitmap = CapturaTelas.CapturaSelecao(6, 6, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            PrintTela = bitmap;
            BitmapImage bitmapImage = ConvertBitmapToBitmapImage(bitmap);
            imagePanel.Source = bitmapImage;
            Opacity = 1;
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Verifica se o botão esquerdo do mouse foi clicado
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var valor = AutoIt.AutoItX.MouseGetPos();
                JanelaPaiPosicional.PosXVal = valor.X;
                JanelaPaiPosicional.PosYVal = valor.Y;
                if (!JanelaPaiPosicional.SelecionarClique)
                {
                    System.Windows.MessageBox.Show($"Mouse pos: {valor}");
                }
                else
                {
                    JanelaPaiPosicional.PosXlbl.Content = $"PosX: {valor.X}";
                    JanelaPaiPosicional.PosYlbl.Content = $"PosY: {valor.Y}";
                    JanelaPaiPosicional.CliqueQtdInput.Text = "1";
                    JanelaPaiPosicional.CliquesintervaloInput.Text = "0";
                    JanelaPaiPosicional.PreIntervaloInput.Text = "0";
                    JanelaPaiPosicional.PosIntervaloInput.Text = "0";
                    JanelaPaiPosicional.AdicionarCliqueBtnsPanel.IsEnabled = true;
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

                    this.Close();
                    System.Threading.Thread.Sleep(1000);
                    var print = CapturaTelas.CapturaSelecaoFromImage(PrintTela, left, top, width, height);
                    new SalvarPrint(print).Show();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Erro ao capturar print; " + ex.Message);
                this.Close();
            }
        }

        static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Save the bitmap to a memory stream in a format that BitmapImage can read
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                memoryStream.Position = 0;

                // Create a BitmapImage and set its source to the memory stream
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}
