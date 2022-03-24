using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace MiodenusAnimationConverter.Media
{
    public readonly struct Screenshot
    {
        public const byte PixelChannelsAmount = 3;
        public readonly byte[] PixelsData;
        public readonly int Width;
        public readonly int Height;
        
        public Bitmap Bitmap
        {
            get
            {
                var bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.WriteOnly, bitmap.PixelFormat);

                Marshal.Copy(PixelsData, 0, bitmapData.Scan0, PixelsData.Length);
                bitmap.UnlockBits(bitmapData);
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                
                return bitmap;
            }
        }

        public Screenshot(in MainWindow window)
        {
            Width = window.Size.X;
            Height = window.Size.Y;
            PixelsData = new byte[Width * Height * PixelChannelsAmount];
            
            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            GL.ReadBuffer(ReadBufferMode.Front);
            GL.ReadPixels(0, 0, Width, Height, PixelFormat.Bgr, PixelType.UnsignedByte, PixelsData);
        }

        public void Save(in string filename, ImageFormat format)
        {
            Bitmap.Save($"{filename}.{format.ToString().ToLower()}", format);
        }
    }
}