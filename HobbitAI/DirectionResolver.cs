using System;
using HelperLibrary;

namespace HobbitAI
{
    public static class DirectionResolver
    {
        public static Direction ToDirection(Point arg)
        {
            if (arg.X == 0)
            {
                switch (arg.Y)
                {
                    case 1:
                        return Direction.Down;
                    case -1:
                        return Direction.Up;
                }
            }
            else if (arg.Y == 0)
                switch (arg.X)
                {
                    case 1:
                        return Direction.Right;
                    case -1:
                        return Direction.Left;
                }
            throw new Exception("Wrong argument");
        }

        public static Point ToPoint(Direction direction)
        {
            var dx = new[] {-1, 0, 1, 0};
            var dy = new[] { 0, -1, 0, 1 };
            for (int i = 0; i < dx.Length; i++)
            {
                var point = new Point(dx[i], dy[i]);
                if (ToDirection(point) == direction)
                    return point;
            }
            throw new ArgumentException("Wrong argument");
        }
    }
}