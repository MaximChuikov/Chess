using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using ChessLogic;
using System.Media;
using System.IO;
using System.Text;

namespace ChessWPF
{
    class GUI : IGui
    {
        private List<Border> aviableMoves = new List<Border>();
        private List<Image> aviableEats = new List<Image>();
        private readonly Canvas canvas;
        private readonly Board board;
        private readonly Label whoseMove;
        private readonly string path;
        private List<Border> figures = new List<Border>();
        private double cellWidth, cellHeight;
        private ChessLogic.Point selected;
        private bool isSelected;
        private readonly SoundPlayer sp;
        public GUI(Canvas canvas, Label whoseMove, string path, SoundPlayer sp)
        {
            this.sp = sp;
            this.path = path;
            this.canvas = canvas;
            cellHeight = canvas.ActualHeight / 8;
            cellWidth = canvas.ActualWidth / 8;
            board = new Board(this);
            this.whoseMove = whoseMove;
        }

        private ChessLogic.Point GetIndex(Border label)
        {
            return new ChessLogic.Point((int)(Canvas.GetLeft(label) / cellWidth), (int)(Canvas.GetTop(label) / cellHeight));
        }
        private ChessLogic.Point GetIndex(System.Windows.Point p)
        {
            return new ChessLogic.Point((int)(p.X / cellWidth), (int)(p.Y / cellHeight));
        }

        private Border FindFigure(ChessLogic.Point point)
        {
            foreach(var img in figures)
            {
                if (GetIndex(img) == point)
                    return img;
            }
            throw new Exception("Error 404: Figure not found");
        }

        public void WhoseMove(ChessLogic.Color color)
        {
            if (color == ChessLogic.Color.Black)
                whoseMove.Content = "Ход черных";
            else if (color == ChessLogic.Color.White)
                whoseMove.Content = "Ход белых";
        }

        public void SelectFigure(ChessLogic.Point cell)
        {
            if (board.IsCanToSelect(cell))
            {
                selected = new ChessLogic.Point(cell.x, cell.y);
                var img = FindFigure(selected);
                img.Background.Opacity = 0.5d;
                isSelected = true;
            }
        }
        private void UnselectFigure()
        {
            var img = FindFigure(selected);
            img.Background.Opacity = 0d;
            isSelected = false;
            UnshowAviableMoves();
        }

        internal void MouseDown(object o, MouseEventArgs e)
        {
            if (!isSelected)
            {
                SelectFigure(GetIndex(e.GetPosition(canvas)));
            }
            else
            {
                UnselectFigure();
                MoveRequest(selected, GetIndex(e.GetPosition(canvas)));
            }
        }
        public PawnReplacedFigure PawnReplaceRequest()
        {
            string folder = GetStylePath();
            var paths = new string[4];
            if (board.WhoseMove == ChessLogic.Color.Black)
            {
                paths[0] = "bQ.png";
                paths[1] = "bN.png";
                paths[2] = "bR.png";
                paths[3] = "bB.png";
            }
            else
            {
                paths[0] = "wQ.png";
                paths[1] = "wN.png";
                paths[2] = "wR.png";
                paths[3] = "wB.png";
            }
            var toReturn = new FigureReference(PawnReplacedFigure.Queen);
            var window = new PawnReplace(folder, paths, ref toReturn);
            window.ShowDialog();

            return toReturn.replacedFigure;
        }
        public void Replace(ChessLogic.Point from, ChessLogic.Point to)
        {
            var lab = FindFigure(from);
            lab.SetValue(Canvas.LeftProperty, to.x * cellWidth);
            lab.SetValue(Canvas.TopProperty, to.y * cellHeight);
            sp.Play();
        }
        public void Set(ChessLogic.Point cell, Figures figure)
        {
            CreateFigure(cell.x, cell.y, figure, Fileworker.GetStyle());
        }
        public void DeleteCellContent(ChessLogic.Point cell)
        {
            canvas.Children.Remove(FindFigure(cell));
            figures.Remove(FindFigure(cell));
        }
        public void MoveRequest(ChessLogic.Point from, ChessLogic.Point to)
        {
            board.MoveRequest(from, to);
        }
        public void DrawAllFigures(Figures[,] board)
        {
            int style = Fileworker.GetStyle();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != Figures.Empty)
                        CreateFigure(i, j, board[i, j], style);
                }
            }
        }
        private string GetStylePath()
        {
            return Directory.GetDirectories(path + "\\images\\styles")[Fileworker.GetStyle()] + "\\";
        }
        private void CreateFigure(int x, int y, Figures figure, int style)
        {
            var pathPhoto = new StringBuilder();
            pathPhoto.Append(GetStylePath());
            switch (figure)
            {
                case Figures.BlackBishop:
                    pathPhoto.Append("bB");
                    break;
                case Figures.BlackKing:
                    pathPhoto.Append("bK");
                    break;
                case Figures.BlackKnight:
                    pathPhoto.Append("bN");
                    break;
                case Figures.BlackPawn:
                    pathPhoto.Append("bP");
                    break;
                case Figures.BlackQueen:
                    pathPhoto.Append("bQ");
                    break;
                case Figures.BlackRook:
                    pathPhoto.Append("bR");
                    break;
                case Figures.WhiteBishop:
                    pathPhoto.Append("wB");
                    break;
                case Figures.WhiteKing:
                    pathPhoto.Append("wK");
                    break;
                case Figures.WhiteKnight:
                    pathPhoto.Append("wN");
                    break;
                case Figures.WhitePawn:
                    pathPhoto.Append("wP");
                    break;
                case Figures.WhiteQueen:
                    pathPhoto.Append("wQ");
                    break;
                case Figures.WhiteRook:
                    pathPhoto.Append("wR");
                    break;
                default:
                    throw new Exception("Unkown figure");
            };
            pathPhoto.Append(".png");


            var border = new Border
            {
                Width = cellWidth,
                Height = cellHeight
            };
            border.SetValue(Canvas.LeftProperty, cellWidth * x);
            border.SetValue(Canvas.TopProperty, cellHeight * y);
            border.Background = new SolidColorBrush(Colors.Orange)
            {
                Opacity = 0d
            };

            var img = new Image
            {
                Source = BitmapFrame.Create(new Uri(pathPhoto.ToString()))
            };

            border.Child = img;

            canvas.Children.Add(border);
            figures.Add(border);
        }

        public void Win(ChessLogic.Color whoWin)
        {
            whoseMove.Content = whoWin == ChessLogic.Color.White ? "Black King is defeated, white won!" : "White King is defeated, black won!";
            sp.SoundLocation = path + "\\sounds\\win.wav";
            sp.Play();
        }
        public void ShowAviableMoves(ChessLogic.Point[] move, ChessLogic.Point[] eat)
        {
            foreach (var p in move)
            {
                var bor = new Border
                {
                    Width = cellWidth,
                    Height = cellHeight
                };
                bor.SetValue(Canvas.LeftProperty, p.x * cellWidth);
                bor.SetValue(Canvas.TopProperty, p.y * cellHeight);

                var img = new Image
                {
                    Source = BitmapFrame.Create(new Uri(path + "\\images\\move.png")),
                    Opacity = 0.4d,
                    Margin = new Thickness(cellWidth / 3.25)
                };
                bor.Child = img;
                canvas.Children.Add(bor);
                aviableMoves.Add(bor);
            }
            foreach (var p in eat)
            {
                var img = new Image
                {
                    Source = BitmapFrame.Create(new Uri(path + "\\images\\eat.png")),
                    Width = cellWidth,
                    Height = cellHeight,
                    Opacity = 0.4d
                };
                img.SetValue(Canvas.LeftProperty, p.x * cellWidth);
                img.SetValue(Canvas.TopProperty, p.y * cellHeight);
                canvas.Children.Add(img);
                aviableEats.Add(img);
            }
        }
        private void UnshowAviableMoves()
        {
            foreach (var img in aviableMoves)
            {
                canvas.Children.Remove(img);
            }
            aviableMoves.Clear();

            foreach (var img in aviableEats)
            {
                canvas.Children.Remove(img);
            }
            aviableEats.Clear();
        }
    }
}
