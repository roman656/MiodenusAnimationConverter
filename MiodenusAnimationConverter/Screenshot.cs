using System;
using System.IO;
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

        public void SaveToFile(in string filename)
        {
            var width = BitConverter.GetBytes(Width);
            var height = BitConverter.GetBytes(Height);
            
            byte[] header = {0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, width[0], width[1], height[0], height[1], 24, 0b00001000};

            using (var fileStream = File.OpenWrite(filename))
            {
                fileStream.Write(header, 0, header.Length);
                fileStream.Write(PixelsData, 0, PixelsData.Length);
            }
        }
    }
}