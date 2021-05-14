using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChessLogic
{
    public enum Color
    {
        Black,
        White
    }
    internal abstract class Figure
    {
        protected Board board;
        internal Figure(Point position, Board board)
        {
            MyPosition = position;
            this.board = board;
        }
        internal abstract Color Color { get; }
        internal Point MyPosition { get; set; }
        internal virtual bool BaseChecking(Point move)
        {
            if (board.CheckIncludingBoard(move))
              return CheckEat(move) && CheckMoveCorrect(move) && CheckObstacles(move) && MyPosition != move;
            return false;
        }
        internal virtual bool MakeMove(Point move, Color whoseMove)
        {
            if (BaseChecking(move) && Color == whoseMove)
            {
                ZeroAllFlagsBeforeMove();
                ReplaceMeTo(move);
                ZeroAllFlagsAfterMove();
                return true;
            }
            else
            {
                return false;
            }
        }
        protected virtual void ZeroAllFlagsBeforeMove()
        {
            foreach (var obj in board.ChessBoard)
            {
                if (obj is Pawn pawn)
                    pawn.isDoubleJumped = false;
            }
        }
        protected virtual void ZeroAllFlagsAfterMove()
        {
            //пока нету
        }
        protected abstract bool CheckMoveCorrect(Point where);
        protected virtual void ReplaceMeTo(Point where)
        {
            MyPosition = board[MyPosition, where];
        }
        protected virtual bool CheckObstacles(Point where)
        {
            int xDir = where.x - MyPosition.x < 0 ? -1 : 1;
            int yDir = where.y - MyPosition.y < 0 ? -1 : 1;
            var currentPosition = new Point(MyPosition.x, MyPosition.y);
            do
            {
                if (currentPosition.x != where.x)
                    currentPosition.x += xDir;
                if (currentPosition.y != where.y)
                    currentPosition.y += yDir;
                if ((currentPosition != where) && board[currentPosition] != null)
                    return false;
            } while (currentPosition != where);
            return true;
        }
        protected bool CheckEat(Point where)
        {
            if (board[where] != null)
                if (board[where] is Figure figure)
                    return figure.Color != this.Color;
                else
                    return false;
            else
                return true;
        }
        protected bool CheckColor(Color color)
        {
            return color == this.Color;
        }
        internal abstract void GetAviableMoves(ref List<Point> move, ref List<Point> eat);
    }

    internal abstract class Bishop : Figure
    {
        internal Bishop(Point position, Board board) : base(position, board) { }
        protected override bool CheckMoveCorrect(Point where)
        {
            return Math.Abs(where.x - MyPosition.x) == Math.Abs(where.y - MyPosition.y);
        }
        internal override void GetAviableMoves(ref List<Point> move, ref List<Point> eat)
        {
            AviableMovesCycleTo(new Point(1, 1), ref move, ref eat);
            AviableMovesCycleTo(new Point(1, -1), ref move, ref eat);
            AviableMovesCycleTo(new Point(-1, 1), ref move, ref eat);
            AviableMovesCycleTo(new Point(-1, -1), ref move, ref eat);
        }
        protected void AviableMovesCycleTo(Point there, ref List<Point> move, ref List<Point> eat)
        {
            Point p;
            int i = 1;
            while(board.CheckIncludingBoard(MyPosition + i * there))
            {
                p = MyPosition + i * there;
                if (BaseChecking(p))
                    if (board[p] is Figure)
                        eat.Add(p);
                    else
                        move.Add(p);
                i++;
            }
        }
    }
    internal class WhiteBishop : Bishop
    {
        internal WhiteBishop(Point position, Board board) : base(position, board) { }
        internal override Color Color => Color.White;
    }
    internal class BlackBishop : Bishop
    {
        internal BlackBishop(Point position, Board board) : base(position, board) { }
        internal override Color Color => Color.Black;
    }
    internal abstract class Rook : Figure
    {
        internal Rook(Point position, Board board) : base(position, board) { }
        internal bool isMoved = false;
        protected override bool CheckMoveCorrect(Point where)
        {
            return MyPosition.x == where.x || MyPosition.y == where.y;
        }
        internal override bool MakeMove(Point move, Color whooseMove)
        {
            if (base.MakeMove(move, whooseMove))
            {
                if (!isMoved)
                    isMoved = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        internal override void GetAviableMoves(ref List<Point> move, ref List<Point> eat)
        {
            for (int i = 0; i < 8; i++)
            {
                var p = new Point(i, MyPosition.y);

                if (BaseChecking(p))
                    if (board[p] is Figure)
                        eat.Add(p);
                    else
                        move.Add(p);

                p = new Point(MyPosition.x, i);
                if (BaseChecking(p))
                    if (board[p] is Figure)
                        eat.Add(p);
                    else
                        move.Add(p);
            }
        }
    }
    internal class BlackRook : Rook
    {
        internal BlackRook(Point position, Board board) : base(position, board) { }
        internal override Color Color => Color.Black;
    }
    internal class WhiteRook : Rook
    {
        internal WhiteRook(Point position, Board board) : base(position, board) { }
        internal override Color Color => Color.White;
    }
    internal abstract class Knight : Figure
    {
        internal Knight(Point position, Board board) : base(position, board) { }
        protected override bool CheckMoveCorrect(Point where)
        {
            return Math.Pow(MyPosition.x - where.x, 2) + Math.Pow(MyPosition.y - where.y, 2) == 5;
        }
        protected override bool CheckObstacles(Point where)
        {
            return true;
        }
        internal override void GetAviableMoves(ref List<Point> move, ref List<Point> eat)
        {
            Point[] allMoves = new Point[8] { MyPosition + new Point(-1, -2), MyPosition + new Point(1, -2), MyPosition + new Point(-1, 2), MyPosition + new Point(1, 2), MyPosition + new Point(2, 1), MyPosition + new Point(2, -1), MyPosition + new Point(-2, 1), MyPosition + new Point(-2, -1), };
            foreach (var p in allMoves)
            {
                if (BaseChecking(p))
                    if (board[p] is Figure)
                        eat.Add(p);
                    else
                        move.Add(p);
            }
        }
    }
    internal class BlackKnight : Knight
    {
        internal BlackKnight(Point position, Board board) : base(position, board) { }
        internal override Color Color => Color.Black;
    }
    internal class WhiteKnight : Knight
    {
        internal WhiteKnight(Point position, Board board) : base(position, board) { }
        internal override Color Color => Color.White;
    }
    internal abstract class Queen : Figure
    {
        internal Queen(Point position, Board board) : base(position, board) { }
        protected override bool CheckMoveCorrect(Point where)
        {
            return (MyPosition.x == where.x || MyPosition.y == where.y) ||
                   (Math.Abs(where.x - MyPosition.x) == Math.Abs(where.y - MyPosition.y));
        }
        internal override void GetAviableMoves(ref List<Point> move, ref List<Point> eat)
        {
            AviableMovesCycleTo(new Point(1, 1), ref move, ref eat);
            AviableMovesCycleTo(new Point(1, -1), ref move, ref eat);
            AviableMovesCycleTo(new Point(-1, 1), ref move, ref eat);
            AviableMovesCycleTo(new Point(-1, -1), ref move, ref eat);

            Point p;
            for (int i = 0; i < 8; i++)
            {
                p = new Point(i, MyPosition.y);

                if (BaseChecking(p))
                    if (board[p] is Figure)
                        eat.Add(p);
                    else
                        move.Add(p);

                p = new Point(MyPosition.x, i);
                if (BaseChecking(p))
                    if (board[p] is Figure)
                        eat.Add(p);
                    else
                        move.Add(p);
            }
        }
        protected void AviableMovesCycleTo(Point there, ref List<Point> move, ref List<Point> eat)
        {
            Point p;
            int i = 1;
            while (board.CheckIncludingBoard(MyPosition + i * there))
            {
                p = MyPosition + i * there;
                if (BaseChecking(p))
                    if (board[p] is Figure)
                        eat.Add(p);
                    else
                        move.Add(p);
                i++;
            }
        }
    }
    internal class WhiteQueen : Queen
    {
        internal WhiteQueen(Point position, Board board) : base(position, board) { }
        internal override Color Color => Color.White;
    }
    internal class BlackQueen : Queen
    {
        internal BlackQueen(Point position, Board board) : base(position, board) { }
        internal override Color Color => Color.Black;
    }
    internal abstract class Pawn : Figure
    {
        internal Pawn(Point position, Board board, Point incStep) : base(position, board)
        {
            this.incStep = incStep;
        }
        internal bool isDoubleJumped = false;
        protected bool isMoved = false;
        protected Point willAte;
        protected readonly Point incStep;

        protected override bool CheckMoveCorrect(Point where)
        {
            if ((where - MyPosition == incStep) || (where - MyPosition == 2 * incStep && !isMoved))
            {
                willAte = new Point(where.x, where.y);
                return true;
            }
            else if (MyPosition + incStep + !incStep == where || MyPosition + incStep - !incStep == where)
            {
                var pawnFinder = where - incStep;
                if (board[pawnFinder] is Pawn pawn)
                {
                    if (pawn.isDoubleJumped && CheckEat(pawnFinder))
                    {
                        willAte = pawnFinder;
                        return true;
                    }
                }
                if (board[where] is Figure)
                {
                    willAte = where;
                    return true;
                }
            }
            return false;
        }
        protected override bool CheckObstacles(Point where)
        {
            if (where - MyPosition == incStep)
                return board.ChessBoard[where.x, where.y] == null;
            if (where - MyPosition == 2 * incStep)
                return board.ChessBoard[where.x, where.y] == null &&
                       board.ChessBoard[(MyPosition + incStep).x, (MyPosition + incStep).y] == null;
            return true; //иначе пешка ест, там препятствий нет
        }
        protected override void ReplaceMeTo(Point where)
        {
            if (where - MyPosition == 2 * incStep)
                isDoubleJumped = true;

            MyPosition = board[MyPosition, where];
            if (willAte != where)
                board[willAte] = null;

            if (!isMoved)
                isMoved = true;
            
            if (!board.CheckIncludingBoard(MyPosition + incStep))
                board[MyPosition] = PawnIsNewFigure(board.gui.PawnReplaceRequest());
        }
        protected object PawnIsNewFigure(PawnReplacedFigure figure)
        {
            if (Color == Color.White)
                switch (figure)
                {
                    case PawnReplacedFigure.Bishop:
                        return new WhiteBishop(MyPosition, board);
                    case PawnReplacedFigure.Queen:
                        return new WhiteQueen(MyPosition, board);
                    case PawnReplacedFigure.Knight:
                        return new WhiteKnight(MyPosition, board);
                    case PawnReplacedFigure.Rook:
                        return new WhiteRook(MyPosition, board);
                    default:
                        return new WhiteQueen(MyPosition, board);
                }
            else
                switch (figure)
                {
                    case PawnReplacedFigure.Bishop:
                        return new BlackBishop(MyPosition, board);
                    case PawnReplacedFigure.Queen:
                        return new BlackQueen(MyPosition, board);
                    case PawnReplacedFigure.Knight:
                        return new BlackKnight(MyPosition, board);
                    case PawnReplacedFigure.Rook:
                        return new BlackRook(MyPosition, board);
                    default:
                        return new BlackQueen(MyPosition, board);
                }
        }
        internal override void GetAviableMoves(ref List<Point> move, ref List<Point> eat)
        {
            Point[] allMoves = new Point[4] { MyPosition + incStep, MyPosition + 2 * incStep, MyPosition + incStep + !incStep, MyPosition + incStep - !incStep };
            foreach (var p in allMoves)
            {
                if (BaseChecking(p))
                    if (p != willAte || board[p] is Figure)
                        eat.Add(p);
                    else
                        move.Add(p);
            }
        }
    }
    internal class BlackPawn : Pawn
    {
        internal BlackPawn(Point position, Board board, Point incStep) : base(position, board, incStep) { }
        internal override Color Color => Color.Black;
    }
    internal class WhitePawn : Pawn
    {
        internal WhitePawn(Point position, Board board, Point incStep) : base(position, board, incStep) { }
        internal override Color Color => Color.White;
    }
    internal abstract class King : Figure
    {
        protected bool isMoved = false;

        protected Point leftRook;
        protected Point rightRook;
        protected Point leftCastle;
        protected Point rightCastle;

        internal Point moveMeTo;
        internal Point moveRookFrom;
        internal Point moveRookTo;

        public King(Point position, Board board, Point leftRook, Point rightRook, Point leftCastle, Point rightCastle) : base(position, board)
        {
            this.leftRook = leftRook;
            this.rightRook = rightRook;
            this.leftCastle = leftCastle;
            this.rightCastle = rightCastle;
        }
        protected override bool CheckMoveCorrect(Point where)
        {
            if (Math.Abs((MyPosition - where).x) <= 1 && Math.Abs((MyPosition - where).y) <= 1)
            {
                return true;
            }
            else if (!isMoved)
            {
                if (where == leftCastle && board.ChessBoard[leftRook.x, leftRook.y] is Rook lRook && !lRook.isMoved)
                {
                    var dir = MyPosition - leftCastle;
                    if (dir.x != 0)
                        dir.x /= Math.Abs(dir.x);
                    if (dir.y != 0)
                        dir.y /= Math.Abs(dir.y);

                    moveMeTo = leftCastle;
                    moveRookFrom = lRook.MyPosition;
                    moveRookTo = leftCastle + dir;
                    return true;
                }
                else if (where == rightCastle && board.ChessBoard[rightRook.x, rightRook.y] is Rook rRook && !rRook.isMoved)
                {
                    var dir = MyPosition - rightCastle;
                    if (dir.x != 0)
                        dir.x /= Math.Abs(dir.x);
                    if (dir.y != 0)
                        dir.y /= Math.Abs(dir.y);

                    moveMeTo = rightCastle;
                    moveRookFrom = rRook.MyPosition;
                    moveRookTo = rightCastle + dir;
                    return true;
                }
            }
            return false;
        }
        protected override bool CheckObstacles(Point where)
        {
            if (where == leftCastle)
                return base.CheckObstacles(leftRook);
            else if (where == rightCastle)
                return base.CheckObstacles(rightRook);
            else
                return true;
        }
        protected override void ReplaceMeTo(Point where)
        {
            if (where == leftCastle || where == rightCastle) //рокировка
            {
                MyPosition = board[MyPosition, moveMeTo];

                if (board[moveRookFrom] is Rook rook)
                {
                    rook.MyPosition = board[moveRookFrom, moveRookTo];
                    rook.isMoved = true;
                }
            }
            else
            {
                base.ReplaceMeTo(where);
            }
            if (!isMoved)
                isMoved = true;
        }
        internal override void GetAviableMoves(ref List<Point> move, ref List<Point> eat)
        {
            Point p;
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    p = MyPosition + new Point(x, y);
                    if (BaseChecking(p))
                        if (board[p] is Figure)
                            eat.Add(p);
                        else
                            move.Add(p);
                }
            }
            p = MyPosition + new Point(-2, 0);
            if (BaseChecking(p))
                move.Add(p);
            p = MyPosition + new Point(2, 0);
            if (BaseChecking(p))
                move.Add(p);
        }
    }
    internal class WhiteKing : King
    {
        internal WhiteKing(Point position, Board board, Point leftRook, Point rightRook, Point leftCastle, Point rightCastle) :
               base(position, board, leftRook, rightRook, leftCastle, rightCastle) { }
        internal override Color Color => Color.White;
    }
    internal class BlackKing : King
    {
        internal BlackKing(Point position, Board board, Point leftRook, Point rightRook, Point leftCastle, Point rightCastle) :
               base(position, board, leftRook, rightRook, leftCastle, rightCastle) { }
        internal override Color Color => Color.Black;
    }
    
}
