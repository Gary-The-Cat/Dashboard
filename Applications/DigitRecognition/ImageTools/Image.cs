using System;
using System.Collections.Generic;
using System.Text;

namespace DigitRecognition.ImageTools
{
    public class Image
    {
        public byte[,] ImageValues;
        public float[] integerValues;
        private readonly int imageWidth;
        private readonly int imageHeight;

        public Image(int imageWidth, int imageHeight)
        {
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;

            ImageValues = new byte[imageWidth, imageHeight];
        }

        public float[] GetImageAsInput()
        {
            var imageInput = new float[imageWidth * imageHeight];
            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    imageInput[x + y * imageHeight] = (ImageValues[x, y]) / 255f;
                }
            }
            return imageInput;
        }

        public void SetIntValue(byte value)
        {
            var byteInt = (int)value;
            integerValues = new float[10];
            integerValues[byteInt] = 1;
        }
    }
}
