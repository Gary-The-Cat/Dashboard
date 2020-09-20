using DigitRecognition.ImageTools;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Events.EventArgs;
using Shared.Interfaces;
using Shared.NeuralNetworks;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace DigitRecognition.Screens
{
    public class DigitRecognitionScreen : Screen
    {
        public const int ImageWidth = 28;
        public const int ImageHeight = 28;
        public const int PixelSize = 16;
        public const int SampleCount = 60000;
        private int selectedImage = 1;
        private ImageExtraction imageExtraction;
        private MLPNeuralNetwork net;
        private bool guessNumber = false;
        private bool isGuessRequired = false;
        private RectangleShape[,] imageCanvas;
        private Text feedbackText;

        private IApplication application;

        public DigitRecognitionScreen(
            IApplication application,
            IApplicationInstance applicationInstance)
            : base(application.Configuration, applicationInstance)
        {
            this.application = application;

            var currentDirectory = Directory.GetCurrentDirectory();
            var imagePath = Path.Combine(currentDirectory, "Resources", "train-images.idx3-ubyte");
            var labelPath = Path.Combine(currentDirectory, "Resources", "train-labels.idx1-ubyte");

            if (!File.Exists(imagePath))
            {
                this.ExtractImagesFromZip();
            }

            // Extract the training data
            imageExtraction = new ImageExtraction(
                imagePath,
                labelPath,
                ImageWidth,
                ImageHeight);

            imageExtraction.LoadImages();

            imageCanvas = GetImageCanvas();

            feedbackText = new Text("Training Network...", new Font("Resources\\font.ttf"));
            feedbackText.Position = new Vector2f(application.Window.Size.X * 0.55f, application.Window.Size.Y / 2);
            feedbackText.FillColor = Color.Black;

            Task.Run(() =>
            {
                this.TrainNetwork();
                feedbackText.DisplayedString = "Ready to go";
            });

            RegisterKeyboardCallback(
                new KeyPressCallbackEventArgs(Keyboard.Key.Right),
                NextImage);

            RegisterKeyboardCallback(
                new KeyPressCallbackEventArgs(Keyboard.Key.Left),
                PreviousImage);
        }

        private void NextImage(KeyboardEventArgs _)
        {
            selectedImage++;
            if (selectedImage == imageExtraction.ImageCount)
            {
                selectedImage = 0;
            }

            isGuessRequired = true;
        }

        private void PreviousImage(KeyboardEventArgs _)
        {
            selectedImage--;
            if (selectedImage < 0)
            {
                selectedImage = imageExtraction.ImageCount - 1;
            }

            isGuessRequired = true;
        }

        private RectangleShape[,] GetImageCanvas()
        {
            var canvas = new RectangleShape[ImageWidth, ImageHeight];

            var canvasSize = PixelSize * ImageHeight;
            var remainingSpace = application.Window.Size.Y - canvasSize;
            var offset = new Vector2f(remainingSpace / 2, remainingSpace / 2);
            var pixelSize = new Vector2f(PixelSize, PixelSize);

            for (int x = 0; x < ImageWidth; x++)
            {
                for (int y = 0; y < ImageWidth; y++)
                {
                    var pixel = new RectangleShape(pixelSize);
                    pixel.Position = new Vector2f(x * PixelSize, y * PixelSize) + offset;
                    pixel.FillColor = Color.Black;
                    canvas[x, y] = pixel;
                }
            }

            return canvas;
        }

        public override void OnUpdate(float dt)
        {
            if (!isGuessRequired)
            {
                return;
            }

            this.LoadSelectedImage();

            if (guessNumber)
            {
                // Guess.
                var guess = net.FeedForward(imageExtraction.Images[selectedImage].GetImageAsInput()).ToList();

                feedbackText.DisplayedString = $"Prediction: {guess.IndexOf(guess.Max())}";
            }
        }

        private void LoadSelectedImage()
        {
            var image = imageExtraction.Images[selectedImage];

            for (int y = 0; y < ImageHeight; y++)
            {
                for (int x = 0; x < ImageWidth; x++)
                {
                    var imagePixel = image.ImageValues[x, y];
                    imageCanvas[x, y].FillColor = new Color(imagePixel, imagePixel, imagePixel);
                }
            }
        }

        public override void OnRender(RenderTarget target)
        {
            foreach (var pixel in imageCanvas)
            {
                target.Draw(pixel);
            }

            target.Draw(feedbackText);
        }

        private void ExtractImagesFromZip()
        {
            var currentDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
            var imageZipPath = Path.Combine(currentDirectory, "images.zip");

            ZipFile.ExtractToDirectory(imageZipPath, currentDirectory);
        }

        private void TrainNetwork()
        {
            // Define the layour for our NN.
            // -- Input Nodes 784 - 1 per pixel (28x28).
            // -- 2 Hidden Layer. 16 nodes each - arbitrary.
            // -- 1 Output Layer. 10 Nodes (0-10).
            var inputNodes = 784;
            var outputNodes = 10;
            var innerLayers = 2;
            var hiddenLayerDepth = 16;

            // Inner Layer + Input + Output = 3
            var layers = new int[innerLayers + 2];

            layers[0] = inputNodes;
            for (int i = 1; i < innerLayers + 1; i++)
            {
                layers[i] = hiddenLayerDepth;
            }
            layers[innerLayers + 1] = outputNodes;

            // Create the NN.
            net = new MLPNeuralNetwork(layers);

            // Grab the training data from the images loaded.
            var trainData = imageExtraction.Images.Select(i => i.GetImageAsInput()).ToList();

            // Grab the reinforcement data from the labels provided.
            var reinforceValues = imageExtraction.Images.Select(i => i.integerValues).ToList();

            // Train the network with our known good data.
            net.Train(trainData, reinforceValues, ReportProgress);

            // We now want to display our guess of the number.
            guessNumber = true;
        }

        private void ReportProgress(int arg1, int arg2)
        {
            feedbackText.DisplayedString = $"Training Network...\n{arg1}\nof\n{arg2}";
        }
    }
}