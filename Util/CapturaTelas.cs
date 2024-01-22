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
using AutoIt;

namespace AutomateClickerBrielina.Util
{
    public static class CapturaTelas
    {
        public static Bitmap CapturaSelecao(int left, int top, int width, int height)
        {
            left -= 6; top -= 6; width += 1; height += 1;
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
                             new Size(width, height),
                             CopyPixelOperation.SourceCopy);

            sw.Stop();

            return screenCapture;
        }

        public static Bitmap CapturaSelecaoFromImage(Bitmap image, int left, int top, int width, int height)
        {
            left -= 6; top -= 6; width += 1; height += 1;
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

            Bitmap croppedBitmap = new Bitmap(width, height);

            Rectangle cropRectangle = new Rectangle(left, top, width, height);

            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                g.DrawImage(image, new Rectangle(0, 0, croppedBitmap.Width, croppedBitmap.Height),
                            cropRectangle,
                            GraphicsUnit.Pixel);
            }

            return croppedBitmap;
        }

        public static List<string> ListaNomesPrints()
        {
            string caminhoDaPasta = @"Prints/";

            // Verifica se a pasta existe
            if (Directory.Exists(caminhoDaPasta))
            {
                // Obtém os nomes dos arquivos na pasta
                List<string> nomesDosArquivos = Directory.GetFiles(caminhoDaPasta).ToList();

                // Exibe os nomes dos arquivos
                string texto = "Nomes dos Arquivos na Pasta:";
                foreach (string nomeDoArquivo in nomesDosArquivos)
                {
                    texto += $"\n{nomeDoArquivo}";
                }

                MessageBox.Show(texto);
                return nomesDosArquivos;
            }
            else
            {
                MessageBox.Show("A pasta não existe.");
                return null;
            }
        }

        public static (bool Existe, int X, int Y) ValidaMoveImagem(string nomeImagem)
        {
            Bitmap myPic = new Bitmap(nomeImagem);
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Bitmap screenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            Graphics g = Graphics.FromImage(screenCapture);
            g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                             Screen.PrimaryScreen.Bounds.Y,
                             0, 0,
                             screenCapture.Size,
                             CopyPixelOperation.SourceCopy);
            screenCapture.Save($"Prints\\ScreenSearchIn.png", ImageFormat.Png);
            myPic.Save($"Prints\\ScreenSearchFor.png", ImageFormat.Png);
            
            (bool Existe, int X, int Y) isInCapture = IsInCapture(myPic, screenCapture);

            if (isInCapture.Existe)
            {
                return (true, isInCapture.X + (myPic.Width / 2), isInCapture.Y + (myPic.Height / 2));
            }
            else
            {
                return (false, 0, 0);
            }

            sw.Stop();
        }


        private static (bool Existe, int X, int Y) IsInCapture(Bitmap searchFor, Bitmap searchIn)
        {
            int width = 0;
            int height = 0;
            for (int x = 0; x < searchIn.Width; x++)
            {
                for (int y = 0; y < searchIn.Height; y++)
                {
                    bool invalid = false;
                    int k = x, l = y;
                    for (int a = 0; a < searchFor.Width; a++)
                    {
                        l = y;
                        for (int b = 0; b < searchFor.Height; b++)
                        {
                            if (searchFor.GetPixel(a, b) != searchIn.GetPixel(k, l))
                            {
                                invalid = true;
                                break;
                            }
                            else
                                l++;
                        }
                        if (invalid)
                            break;
                        else
                            k++;
                    }
                    if (!invalid)
                    {
                        width = x;
                        height = y;
                        return (true, width, height);
                    }
                }
            }
            return (false, width, height);
        }
    }
}
