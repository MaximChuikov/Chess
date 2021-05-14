using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLogic
{
    public class Point
    {
        public int x, y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public static bool operator ==(Point one, Point two)
        {
            return one.x == two.x && one.y == two.y;
        }
        public static bool operator !=(Point one, Point two)
        {
            return one.x != two.x || one.y != two.y;
        }
        public static Point operator +(Point one, Point two)
        {
            return new Point(one.x + two.x, one.y + two.y);
        }
        public static Point operator -(Point one, Point two)
        {
            return new Point(one.x - two.x, one.y - two.y);
        }
        public static Point operator *(int num, Point p)
        {
            return new Point(num * p.x, num * p.y);
        }
        public static Point operator !(Point p) => new Point(p.y, p.x);
    }
}
