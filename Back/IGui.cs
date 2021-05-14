using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLogic
{
    public enum PawnReplacedFigure
    {
        Knight,
        Bishop,
        Queen,
        Rook
    }
    public enum Figures
    {
        Empty,
        BlackPawn,
        BlackKing,
        BlackKnight,
        BlackBishop,
        BlackQueen,
        BlackRook,
        WhitePawn,
        WhiteKing,
        WhiteKnight,
        WhiteBishop,
        WhiteQueen,
        WhiteRook
    }

    public interface IGui
    {
        void Win(Color whoWin);
        void DrawAllFigures(Figures[,] board);
        void MoveRequest(Point from, Point to);
        void WhoseMove(Color color);
        void Replace(Point from, Point to);
        void Set(Point cell, Figures figure);
        void DeleteCellContent(Point cell);
        void SelectFigure(Point cell);
        PawnReplacedFigure PawnReplaceRequest();
        void ShowAviableMoves(Point[] move, Point[] eat);
    }
}
