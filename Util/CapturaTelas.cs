using OpenCvSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;


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
            using (var myPic = new Bitmap(nomeImagem))
            using (var screenCapture = new Bitmap(
                Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(screenCapture))
                {
                    g.CopyFromScreen(
                        Screen.PrimaryScreen.Bounds.X,
                        Screen.PrimaryScreen.Bounds.Y,
                        0,
                        0,
                        screenCapture.Size,
                        CopyPixelOperation.SourceCopy);
                }

                // opcional: salvar para debug
                screenCapture.Save($"Prints\\ScreenSearchIn.png", ImageFormat.Png);
                myPic.Save($"Prints\\ScreenSearchFor.png", ImageFormat.Png);

                var isInCapture = IsInCaptureOpenCv(myPic, screenCapture);

                if (isInCapture.Existe)
                {
                    return (
                        true,
                        isInCapture.X + (myPic.Width / 2),
                        isInCapture.Y + (myPic.Height / 2)
                    );
                }

                return (false, 0, 0);
            }
        }


        public static (bool Existe, int X, int Y) IsInCaptureOpenCv(
            Bitmap templateBmp, Bitmap sourceBmp, double threshold = 0.80)
        {
            try
            {
                // Normalizar Bitmap para formato fixo
                using (var tplNorm = templateBmp.Clone(
                    new Rectangle(0, 0, templateBmp.Width, templateBmp.Height),
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                using (var srcNorm = sourceBmp.Clone(
                    new Rectangle(0, 0, sourceBmp.Width, sourceBmp.Height),
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                using (var templateMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(tplNorm))
                using (var sourceMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(srcNorm))
                using (var templateGray = new Mat())
                using (var sourceGray = new Mat())
                using (var result = new Mat())
                {
                    Cv2.CvtColor(templateMat, templateGray, ColorConversionCodes.BGR2GRAY);
                    Cv2.CvtColor(sourceMat, sourceGray, ColorConversionCodes.BGR2GRAY);

                    if (templateGray.Width > sourceGray.Width ||
                        templateGray.Height > sourceGray.Height)
                    {
                        return (false, 0, 0);
                    }

                    Cv2.MatchTemplate(sourceGray, templateGray, result, TemplateMatchModes.CCoeffNormed);
                    Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point maxLoc);

                    if (maxVal >= threshold)
                        return (true, maxLoc.X, maxLoc.Y);

                    return (false, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return (false, 0, 0);
            }
        }
    }
}
