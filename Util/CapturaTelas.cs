using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System.IO;

namespace AutomateClickerBrielina.Util
{
    public static class CapturaTelas
    {
        public static Bitmap CapturaSelecao(int left, int top, int width, int height)
        {
            if (width < 0)
            {
                left += width;
                width *= -1;
            }
            if (height < 0)
            {
                top += height;
                height *= -1;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Bitmap screenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            screenCapture = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(screenCapture);
            g.CopyFromScreen(left,
                             top,
                             0, 0,
                             new Size(width,height),
                             CopyPixelOperation.SourceCopy);

            string filename = "print" + DateTime.Now.ToString("_dd-MM_hh-mm");
            sw.Stop();

            return screenCapture;
        }

        public static void ListaNomesPrints()
        {
            string caminhoDaPasta = @"Prints/";

            // Verifica se a pasta existe
            if (Directory.Exists(caminhoDaPasta))
            {
                // Obtém os nomes dos arquivos na pasta
                string[] nomesDosArquivos = Directory.GetFiles(caminhoDaPasta);

                // Exibe os nomes dos arquivos
                string texto = "Nomes dos Arquivos na Pasta:";
                foreach (string nomeDoArquivo in nomesDosArquivos)
                {
                    texto += $"\n{nomeDoArquivo}";
                }

                MessageBox.Show(texto);
            }
            else
            {
                MessageBox.Show("A pasta não existe.");
            }
        }
    }
}
