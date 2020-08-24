using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DigitRecognition.ImageTools
{
    public class ImageExtraction
    {
        public List<Image> Images { get; set; }
        public int ImageCount { get; set; }

        private string imagePath;
        private string labelPath;

        private readonly int imageWidth;
        private readonly int imageHeight;

        // Extraction for use with training set provided by
        // http://yann.lecun.com/exdb/mnist/
        // File format can be found on site.
        public ImageExtraction(
            string imagePath, 
            string labelPath,
            int imageWidth, 
            int imageHeight)
        {
            this.imagePath = imagePath;
            this.labelPath = labelPath;
            this.Images = new List<Image>();

            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
        }

        /// <summary>
        /// Loads the images into our own image format and also loads in the label data for training.
        /// </summary>
        public void LoadImages()
        {
            int pixel = 0;
            int x, y;

            var image = new Image(imageWidth, imageHeight);

            // Load the image data.
            using (FileStream fs = File.Open(imagePath, FileMode.Open))
            {
                // The first 16 bytes are junk, as per the the instructions on the site above.
                byte[] junk = new byte[16];
                fs.Read(junk, 0, junk.Length);

                // While we have bytes to read, load the images.
                while (true)
                {
                    byte[] b = new byte[1];
                    var bytesRead = fs.Read(b, 0, b.Length);

                    // We have hit the end of our stream.
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    // Get the x, y position
                    x = pixel % imageWidth;
                    y = pixel / imageWidth;

                    // Save the value into our image.
                    image.ImageValues[x, y] = b[0];
                    pixel++;

                    // If we have bit the end of our image, move onto the next one.
                    if (pixel == imageWidth * imageHeight)
                    {
                        Images.Add(image);
                        ImageCount++;
                        image = new Image(imageWidth, imageHeight);
                        pixel = 0;
                    }
                }
            }

            // Load the label data.
            using (FileStream fs = File.Open(labelPath, FileMode.Open))
            {
                // The first 8 bytes are junk, as per the the instructions on the site above.
                byte[] junk = new byte[8];
                fs.Read(junk, 0, junk.Length);

                // While we have bytes left, populate 
                for (int i = 0; i < Images.Count; i++)
                {
                    // As we are adding to existing images, we need to track which one.
                    image = Images[i];
                    byte[] b = new byte[1];
                    var bytesRead = fs.Read(b, 0, b.Length);

                    image.SetIntValue(b[0]);
                }
            }
        }
    }
}
