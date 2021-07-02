using System;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter
{
    public struct Screenshot
    {
        public const byte PixelChannelsAmount = 3;
        public readonly byte[] PixelsData;
        public readonly ushort Width;
        public readonly ushort Height;

        public Screenshot(ushort width, ushort height)
        {
            if ((width <= 0) || (height <= 0))
            {
                throw new ArgumentException("Screenshot`s width and height must be greater than 0.");
            }

            Width = width;
            Height = height;
            PixelsData = new byte[width * height * PixelChannelsAmount];
            
            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            GL.ReadBuffer(ReadBufferMode.Front);
            GL.ReadPixels(0, 0, Width, Height, PixelFormat.Rgb, PixelType.Byte, PixelsData);
        }

        public void SaveToPng(in string filename)
        {
            Bitmap bmp = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), 
                    System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
            
            Marshal.Copy(PixelsData, 0, bitmapData.Scan0, PixelsData.Length);
            bmp.UnlockBits(bitmapData);
            bmp.Save($"{filename}.png");
        }
    }
}