using System;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChessWPF
{
    public partial class MainWindow : Window
    {
        private GUI gui;
        private readonly string path;
        private SoundPlayer sp;
        public MainWindow()
        {
            InitializeComponent();
            path = Environment.CurrentDirectory;
            Fileworker.SetPaths(path);

            imgBoard.ImageSource = BitmapFrame.Create(new Uri(path + "/images/background.png"));
            Icon = BitmapFrame.Create(new Uri(path + "/images/icon.png"));
            sp = new SoundPlayer
            {
                SoundLocation = path + "\\sounds\\music.wav"
            };
            MenuOpen();
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gui.MouseDown(sender, e);
        }
        private void MenuOpen()
        {
            sp.Stop();
            sp.SoundLocation = path + "\\sounds\\music.wav";
            sp.PlayLooping();
            ChessField.Visibility = Visibility.Hidden;
            Menu.Visibility = Visibility.Visible;
        }
        private void MenuClose()
        {
            Menu.Visibility = Visibility.Hidden;
            ChessField.Visibility = Visibility.Visible;
            sp.Stop();
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            MenuClose();
            sp.SoundLocation = path + "\\sounds\\move.wav";
            canvas.Children.Clear();
            whoseMove.Content = "";
            gui = new GUI(canvas, whoseMove, path, sp);
        }

        private void Settings(object sender, RoutedEventArgs e)
        {
            var settings = new Settings(path + "\\images\\styles");
            settings.ShowDialog();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ToMenu(object sender, RoutedEventArgs e)
        {
            MenuOpen();
        }
    }
}
