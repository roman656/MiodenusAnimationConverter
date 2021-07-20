using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace MiodenusAnimationConverter.Media
{
    public struct Screenshot
    {
        public const byte PixelChannelsAmount = 3;
        public readonly byte[] PixelsData;
        public readonly ushort Width;
        public readonly ushort Height;
        
        public Bitmap Bitmap
        {
            get
            {
                var bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.WriteOnly, bitmap.PixelFormat);

                Marshal.Copy(PixelsData, 0, bitmapData.Scan0, PixelsData.Length);
                bitmap.UnlockBits(bitmapData);

                return bitmap;
            }
        }

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
            GL.ReadPixels(0, 0, Width, Height, PixelFormat.Bgr, PixelType.Byte, PixelsData);
        }

        public void Save(in string filename, ImageFormat format)
        {
            Bitmap.Save($"{filename}.{format.ToString().ToLower()}", format);
        }
    }
}