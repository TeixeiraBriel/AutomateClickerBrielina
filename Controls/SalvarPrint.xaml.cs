using AutomateClickerBrielina.Util;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AutomateClickerBrielina.Controls
{
    /// <summary>
    /// Lógica interna para SalvarPrint.xaml
    /// </summary>
    public partial class SalvarPrint : Window
    {
        private Bitmap Print;
        public SalvarPrint(Bitmap _print)
        {
            InitializeComponent();
            Print = _print;

            InicializaImagem();
        }

        void InicializaImagem()
        {
            try
            {
                IntPtr hBitmap = Print.GetHbitmap();
                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                // Libera o recurso do HBitmap para evitar vazamento de memória
                DeleteObject(hBitmap);

                imagePanel.Source = bitmapSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar a imagem: {ex.Message}");
            }
        }

        private void FecharJanela(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Salvar(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(inputName.Text))
            {
                MessageBox.Show("Favor inserir nome do arquivo.");
                return;
            }

            Print.Save($"Prints\\{inputName.Text}.png", ImageFormat.Png);
            this.Close();
            CapturaTelas.ListaNomesPrints();
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}
