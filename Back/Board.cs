using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ChessLogic
{
    public class Board
    {
        public IGui gui;
        private bool gameIsEnd = false;
        internal object[,] ChessBoard;
        public Color WhoseMove { get; private set; } = Color.White;
        public Board(IGui gui)
        {
            this.gui = gui;
            ArrangeFigures();
            gui.DrawAllFigures(GetArray());
        }
        internal object this [Point p]
        {
            get
            {
                return ChessBoard[p.x, p.y];
            }
            set
            {
                if (value == null)
                    if (ChessBoard[p.x, p.y] is King king)
                    {
                        gui.Win(king.Color == Color.White ? Color.Black : Color.White);
                        gameIsEnd = true;
                        gui.DeleteCellContent(p);
                    }    
                    else
                        gui.DeleteCellContent(p);
                else if (ChessBoard[p.x, p.y] != null)
                {
                    gui.DeleteCellContent(p);
                    gui.Set(p, FigureTranslator(value));
                }
                else
                    gui.Set(p, FigureTranslator(value));
                ChessBoard[p.x, p.y] = value;
            }
        }
        internal Point this[Point from, Point to]
        {
            get
            {
                if (ChessBoard[to.x, to.y] != null)
                    if (ChessBoard[to.x, to.y] is King king)
                    {
                        gui.Win(king.Color == Color.White ? Color.Black : Color.White);
                        gameIsEnd = true;
                        gui.DeleteCellContent(to);
                    }
                    else
                        gui.DeleteCellContent(to);
                gui.Replace(from, to);
                ChessBoard[to.x, to.y] = ChessBoard[from.x, from.y];
                ChessBoard[from.x, from.y] = null;
                return new Point(to.x, to.y);
            }
        }
        public bool IsCanToSelect(Point cell)
        {
            if (ChessBoard[cell.x, cell.y] != null && !gameIsEnd)
                if (ChessBoard[cell.x, cell.y] is Figure figure && figure.Color == WhoseMove)
                {
                    var moves = new List<Point>();
                    var eats = new List<Point>();

                    figure.GetAviableMoves(ref moves, ref eats);
                    if (moves.Count + eats.Count == 0)
                        return false;
                    else
                    {
                        gui.ShowAviableMoves(moves.ToArray(), eats.ToArray());
                        return true;
                    }
                }
            return false;
        }
        
        public void MoveRequest(Point from, Point where)
        {
            if (CheckIncludingBoard(from) && CheckIncludingBoard(where) && (from != where) && !gameIsEnd)
                if (ChessBoard[from.x, from.y] != null)
                    if (ChessBoard[from.x, from.y] is Figure figure)
                        if (figure.MakeMove(where, WhoseMove) && !gameIsEnd)
                        {
                            WhoseMove = WhoseMove == Color.White ? Color.Black : Color.White;
                            gui.WhoseMove(WhoseMove);
                        }     
        }
        internal bool CheckIncludingBoard(Point cell)
        {
            return (cell.x >= 0 && cell.x <= 7) && (cell.y >= 0 && cell.y <= 7);
        }
        internal Figures FigureTranslator(object fig)
        {
            if (fig is Figure figure)
                if (figure.Color == Color.Black)
                {
                    if (figure is Pawn)
                        return Figures.BlackPawn;
                    else if (figure is Bishop)
                        return Figures.BlackBishop;
                    else if (figure is Knight)
                        return Figures.BlackKnight;
                    else if (figure is Rook)
                        return Figures.BlackRook;
                    else if (figure is Queen)
                        return Figures.BlackQueen;
                    else if (figure is King)
                        return Figures.BlackKing;
                }
                else if (figure.Color == Color.White)
                {
                    if (figure is Pawn)
                        return Figures.WhitePawn;
                    else if (figure is Bishop)
                        return Figures.WhiteBishop;
                    else if (figure is Knight)
                        return Figures.WhiteKnight;
                    else if (figure is Rook)
                        return Figures.WhiteRook;
                    else if (figure is Queen)
                        return Figures.WhiteQueen;
                    else if (figure is King)
                        return Figures.WhiteKing;
                }
            return Figures.Empty;
        }
        protected Figures[,] GetArray()
        {
            var array = new Figures[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (ChessBoard[i, j] != null)
                        array[i, j] = FigureTranslator(ChessBoard[i, j]);
                    else
                        array[i, j] = Figures.Empty;
                }
            }
            return array;
        }
        private void ArrangeFigures()
        {
            ChessBoard = new object[8, 8];
            ChessBoard[0, 0] = new BlackRook(new Point(0, 0), this);
            ChessBoard[1, 0] = new BlackKnight(new Point(1, 0), this);
            ChessBoard[2, 0] = new BlackBishop(new Point(2, 0), this);
            ChessBoard[3, 0] = new BlackQueen(new Point(3, 0), this);
            ChessBoard[4, 0] = new BlackKing(new Point(4, 0), this, new Point(0, 0), new Point(7, 0), new Point(2, 0), new Point(6, 0));
            ChessBoard[5, 0] = new BlackBishop(new Point(5, 0), this);
            ChessBoard[6, 0] = new BlackKnight(new Point(6, 0), this);
            ChessBoard[7, 0] = new BlackRook(new Point(7, 0), this);
            for (int i = 0; i < 8; i++)
            {
                ChessBoard[i, 1] = new BlackPawn(new Point(i, 1), this, new Point(0, 1));
            }

            for (int i = 0; i < 8; i++)
            {
                ChessBoard[i, 6] = new WhitePawn(new Point(i, 6), this, new Point(0, -1));
            }
            ChessBoard[0, 7] = new WhiteRook(new Point(0, 7), this);
            ChessBoard[1, 7] = new WhiteKnight(new Point(1, 7), this);
            ChessBoard[2, 7] = new WhiteBishop(new Point(2, 7), this);
            ChessBoard[3, 7] = new WhiteQueen(new Point(3, 7), this);
            ChessBoard[4, 7] = new WhiteKing(new Point(4, 7), this, new Point(0, 7), new Point(7, 7), new Point(2, 7), new Point(6, 7));
            ChessBoard[5, 7] = new WhiteBishop(new Point(5, 7), this);
            ChessBoard[6, 7] = new WhiteKnight(new Point(6, 7), this);
            ChessBoard[7, 7] = new WhiteRook(new Point(7, 7), this);
        }
    }
}
