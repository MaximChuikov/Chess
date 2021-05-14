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
using ChessLogic;

namespace ChessWPF
{
    public partial class PawnReplace : Window
    {
        private FigureReference toReturn;
        public PawnReplace(string pathFolder, string[] imgs, ref FigureReference toReturn)
        {
            InitializeComponent();
            img1.Source = BitmapFrame.Create(new Uri(pathFolder + imgs[0]));
            img2.Source = BitmapFrame.Create(new Uri(pathFolder + imgs[1]));
            img3.Source = BitmapFrame.Create(new Uri(pathFolder + imgs[2]));
            img4.Source = BitmapFrame.Create(new Uri(pathFolder + imgs[3]));
            this.toReturn = toReturn;
        }

        private void Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is DockPanel dockP)
                switch (dockP.Name)
                {
                    case "Queen":
                        toReturn.replacedFigure = PawnReplacedFigure.Queen;
                        break;
                    case "Knight":
                        toReturn.replacedFigure = PawnReplacedFigure.Knight;
                        break;
                    case "Rook":
                        toReturn.replacedFigure = PawnReplacedFigure.Rook;
                        break;
                    case "Bishop":
                        toReturn.replacedFigure = PawnReplacedFigure.Bishop;
                        break;
                    default:
                        toReturn.replacedFigure = PawnReplacedFigure.Queen;
                        break;
                }
            else toReturn.replacedFigure = PawnReplacedFigure.Queen;

            this.Close();
        }
    }
}
