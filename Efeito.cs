using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afinamento
{

    class Ponto
    {
        int x, y;

        public Ponto(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
    }

    class Efeito
    {
        public static void convert_to_grayDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int pixelSize = 3;
            Int32 gs;

            //lock dados bitmap origem
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //lock dados bitmap destino
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();

                int r, g, b;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        b = *(src++); //está armazenado dessa forma: b g r 
                        g = *(src++);
                        r = *(src++);
                        gs = (Int32)(r * 0.2990 + g * 0.5870 + b * 0.1140);
                        *(dst++) = (byte)gs;
                        *(dst++) = (byte)gs;
                        *(dst++) = (byte)gs;
                    }
                    src += padding;
                    dst += padding;
                }
            }
            //unlock imagem origem
            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            //unlock imagem destino
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }

        public static void afinamento(Bitmap imgSrc, Bitmap imgDest)
        {
            int width = imgSrc.Width, height = imgSrc.Height, info, conect = 0, c = 0;
            Color cor;
            List<Ponto> lista = new List<Ponto>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //obtendo a cor do pixel
                    cor = imgSrc.GetPixel(x, y);

                    if (cor.R > 170)
                        info = 255;
                    else
                        info = 0;

                    //nova cor
                    Color newcolor = Color.FromArgb(info, info, info);

                    imgDest.SetPixel(x, y, newcolor);
                }
            }

            Color p1, p2, p3, p4, p5, p6, p7, p8, p9;

            while (c != 2)
            {
                c = 0;
                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        conect = 0;

                        p1 = imgDest.GetPixel(x, y);
                        if(p1.R == 0)
                        {
                            p2 = imgDest.GetPixel(x, y - 1);
                            p3 = imgDest.GetPixel(x + 1, y - 1);
                            p4 = imgDest.GetPixel(x + 1, y);
                            p5 = imgDest.GetPixel(x + 1, y + 1);
                            p6 = imgDest.GetPixel(x, y + 1);
                            p7 = imgDest.GetPixel(x - 1, y + 1);
                            p8 = imgDest.GetPixel(x - 1, y);
                            p9 = imgDest.GetPixel(x - 1, y - 1);

                            //Para cada pixel
                            if (p2.R == 255 && p3.R == 0)
                                conect++;
                            if (p3.R == 255 && p4.R == 0)
                                conect++;
                            if (p4.R == 255 && p5.R == 0)
                                conect++;
                            if (p5.R == 255 && p6.R == 0)
                                conect++;
                            if (p6.R == 255 && p7.R == 0)
                                conect++;
                            if (p7.R == 255 && p8.R == 0)
                                conect++;
                            if (p8.R == 255 && p9.R == 0)
                                conect++;

                            if (conect == 1)
                            {
                                conect = 0;
                                if (p2.R == 0)
                                    conect++;
                                if (p3.R == 0)
                                    conect++;
                                if (p4.R == 0)
                                    conect++;
                                if (p5.R == 0)
                                    conect++;
                                if (p6.R == 0)
                                    conect++;
                                if (p7.R == 0)
                                    conect++;
                                if (p8.R == 0)
                                    conect++;
                                if (p9.R == 0)
                                    conect++;

                                if (conect >= 2 && conect <= 6)
                                {
                                    if (p2.R == 255 || p4.R == 255 || p8.R == 255)
                                    {
                                        if (p2.R == 255 || p6.R == 255 || p8.R == 255)
                                        {
                                            lista.Add(new Ponto(x, y));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (lista.Count == 0)
                    c++;
                else
                {
                    foreach (Ponto p in lista)
                    {
                        imgDest.SetPixel(p.X, p.Y, Color.FromArgb(255, 255, 255));
                    }
                    lista = new List<Ponto>();
                }

                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        conect = 0;

                        p1 = imgDest.GetPixel(x, y);
                        if (p1.R == 0)
                        {
                            p2 = imgDest.GetPixel(x, y - 1);
                            p3 = imgDest.GetPixel(x + 1, y - 1);
                            p4 = imgDest.GetPixel(x + 1, y);
                            p5 = imgDest.GetPixel(x + 1, y + 1);
                            p6 = imgDest.GetPixel(x, y + 1);
                            p7 = imgDest.GetPixel(x - 1, y + 1);
                            p8 = imgDest.GetPixel(x - 1, y);
                            p9 = imgDest.GetPixel(x - 1, y - 1);

                            //Para cada pixel
                            if (p2.R == 255 && p3.R == 0)
                                conect++;
                            if (p3.R == 255 && p4.R == 0)
                                conect++;
                            if (p4.R == 255 && p5.R == 0)
                                conect++;
                            if (p5.R == 255 && p6.R == 0)
                                conect++;
                            if (p6.R == 255 && p7.R == 0)
                                conect++;
                            if (p7.R == 255 && p8.R == 0)
                                conect++;
                            if (p8.R == 255 && p9.R == 0)
                                conect++;

                            if (conect == 1)
                            {
                                conect = 0;
                                if (p2.R == 0)
                                    conect++;
                                if (p3.R == 0)
                                    conect++;
                                if (p4.R == 0)
                                    conect++;
                                if (p5.R == 0)
                                    conect++;
                                if (p6.R == 0)
                                    conect++;
                                if (p7.R == 0)
                                    conect++;
                                if (p8.R == 0)
                                    conect++;
                                if (p9.R == 0)
                                    conect++;

                                if (conect >= 2 && conect <= 6)
                                {
                                    if (p2.R == 255 || p4.R == 255 || p6.R == 255)
                                    {
                                        if (p4.R == 255 || p6.R == 255 || p8.R == 255)
                                        {
                                            lista.Add(new Ponto(x, y));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (lista.Count == 0)
                    c++;
                else
                {
                    foreach (Ponto p in lista)
                    {
                        imgDest.SetPixel(p.X, p.Y, Color.FromArgb(255, 255, 255));
                    }
                    lista = new List<Ponto>();
                }
            }
        }
    }
}
