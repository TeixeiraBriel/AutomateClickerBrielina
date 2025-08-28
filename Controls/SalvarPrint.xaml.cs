using AutomateClickerBrielina.Entidades;
using AutomateClickerBrielina.Enums;
using AutomateClickerBrielina.Servico;
using AutomateClickerBrielina.Util;
using System;
using System.CodeDom;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AutomateClickerBrielina.Controls
{
    /// <summary>
    /// Lógica interna para SalvarPrint.xaml
    /// </summary>
    public partial class SalvarPrint : Page
    {
        private Bitmap Print;
        private string _FileName;
        private CliquesControlador CliquesControlador;

        public SalvarPrint(Bitmap _print)
        {
            InitializeComponent();
            Print = _print;
            CliquesControlador = MainWindow.CliquesControlador;

            InicializaImagem();
            inputName.IsReadOnly = false;
        }

        void InicializaImagem()
        {
            try
            {
                if (Print == null)
                    return;

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

        private void Salvar(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(inputName.Text))
            {
                MessageBox.Show("Favor inserir nome do arquivo.");
                return;
            }

            Print.Save($"Prints\\{inputName.Text}.png", ImageFormat.Png);
            _FileName = $"Prints\\{inputName.Text}.png";
            MessageBox.Show($"Arquivo {inputName.Text}.png salvo.");
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private void Carregar(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

            // Defina as propriedades do OpenFileDialog conforme necessário
            openFileDialog.Title = "Selecionar Arquivo";
            openFileDialog.Filter = "Arquivos PNG|*.png|Todos os Arquivos|*.*";
            openFileDialog.DefaultExt = "png";

            // Defina o diretório inicial relativo à aplicação
            string diretorioInicial = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Prints");
            openFileDialog.InitialDirectory = diretorioInicial;

            // Abra o diálogo e verifique se o usuário selecionou um arquivo
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // O caminho do arquivo selecionado está em openFileDialog.FileName
                string caminhoDoArquivo = openFileDialog.FileName;
                imagePanel.Source = buscarImage(caminhoDoArquivo);
                inputName.Text = caminhoDoArquivo;
                inputName.IsReadOnly = true;
                _FileName = caminhoDoArquivo;

                // Faça algo com o caminho do arquivo, como exibir em uma caixa de texto
            }
        }

        private BitmapImage buscarImage(string caminho)
        {
            // Crie um objeto BitmapImage e atribua-o à propriedade Source do Image
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(caminho, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();

            return bitmapImage;
        }

        private void AdicionarClique(object sender, RoutedEventArgs e)
        {
            CliquesControlador.Add(new Clique()
            {
                Tipo = TipoCliqueEnum.Imagem,
                FileName = _FileName,
                Imagem = true,
                PosSleep = 0,
                PreSleep = 0,
                qtdCliques = 1,
                TempoIntervalo = 0,
                posX = 0,
                posY = 0
            });

            Window.GetWindow(this).Close();
            new GerenciaFluxo().Show();
        }

        private void FecharJanela(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
