using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using OpenCvSharp;


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
                             new System.Drawing.Size(width, height),
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

        public static (bool Existe, int X, int Y) ValidaImagem(string nomeImagem)
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

            (bool Existe, int X, int Y) isInCapture = IsInCaptureOpenCv(myPic, screenCapture);

            if (isInCapture.Existe)
            {
                sw.Stop();
                return (true, isInCapture.X + (myPic.Width / 2), isInCapture.Y + (myPic.Height / 2));
            }
            else
            {
                sw.Stop();
                return (false, 0, 0);
            }

        }

        public static (bool Exists, int X, int Y) IsInCaptureOpenCv(Bitmap templateBmp, Bitmap sourceBmp, double threshold = 0.80)
        {
            using (var templateMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(templateBmp))
            using (var sourceMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(sourceBmp))
            {
                // Split template and source into channels
                Mat[] templateChannels = Cv2.Split(templateMat);
                Mat[] sourceChannels = Cv2.Split(sourceMat);

                var resultChannels = new Mat[3]; // assuming 3 channels (BGR)

                for (int c = 0; c < 3; c++)
                {
                    // MatchTemplate for each channel
                    resultChannels[c] = new Mat();
                    Cv2.MatchTemplate(sourceChannels[c], templateChannels[c], resultChannels[c], TemplateMatchModes.CCoeffNormed);
                }

                // Average the results per pixel
                Mat colorResult = (resultChannels[0] + resultChannels[1] + resultChannels[2]) / 3.0;

                // Find best match in the averaged result
                Cv2.MinMaxLoc(colorResult, out double minVal, out double maxVal, out OpenCvSharp.Point minLoc, out OpenCvSharp.Point maxLoc);

                if (maxVal >= threshold)
                    return (true, maxLoc.X, maxLoc.Y);
            }

            return (false, 0, 0);
        }

    }
}
