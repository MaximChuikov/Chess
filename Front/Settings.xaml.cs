using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace ChessWPF
{
    public partial class Settings : Window
    {
        private readonly string pathDirStyles;
        private List<DockPanel> list = new List<DockPanel>();
        public Settings(string pathDirStyles)
        {
            InitializeComponent();
            this.pathDirStyles = pathDirStyles;

            ChoosingFigure(new Thickness(2), new RoutedEventArgs());
        }

        private void ChoosingFigure(object sender, RoutedEventArgs e)
        {
            var folders = Directory.GetDirectories(pathDirStyles);
            for (int i = 0; i < folders.Length; i++)
            {
                var array = folders[i].Split('\\');
                List.Items.Add(CreateDock(folders[i] + "\\wQ.png", array[array.Length - 1]));
            }
        }
        private DockPanel CreateDock(string pathPhoto, string labelContent)
        {
            var dock = new DockPanel()
            {
                Margin = new Thickness(0, 1, 0, 1),
                Background = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            var border = new Border
            {
                Height = 50,
                Width = 50,
                Background = Brushes.White
            };
            var img = new Image
            {
                Source = BitmapFrame.Create(new Uri(pathPhoto.ToString())),
                Width = 50,
                Height = 50
            };
            border.Child = img;
            var lab = new Label
            {
                Content = labelContent,
                Foreground = Brushes.Wheat,
                FontSize = 30,
                Margin = new Thickness(10, 0, 0, 0)
            };
            dock.Children.Add(border);
            dock.Children.Add(lab);
            list.Add(dock);
            dock.MouseLeftButtonDown += DockClick;
            return dock;
        }
        private void DockClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DockPanel dock)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (dock == list[i])
                        Fileworker.SaveData(TypeOfData.FigureStyle, i);
                }
            }
            this.Close();
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
