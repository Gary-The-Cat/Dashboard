using Dashboard.Core;
using SFML.Graphics;
using SFML.Window;
using Shared.ScreenConfig;

namespace Dashboard
{
    public class Program
    {
        static void Main(string[] _)
        {
            var settings = new ContextSettings()
            {
                AntialiasingLevel = 8,
            };

            var configuration = new ScreenConfiguration();

            configuration.Width = 1920;
            configuration.Height = 1080;

            var window = new RenderWindow(
                new VideoMode(configuration.Width, configuration.Height), 
                "Dashboard", 
                Styles.Default, 
                settings);

            Image im = new Image("..\\..\\..\\Icon.png");
            window.SetIcon(64,64, im.Pixels);
            window.SetVerticalSyncEnabled(true);

            var application = new Application(window, configuration);

            application.Start();
        }
    }
}