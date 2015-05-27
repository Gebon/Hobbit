using System;
using HelperLibrary;

namespace HobbitAI
{
    public static class DirectionResolver
    {
        public static Direction ToDirection(Point arg)
        {
            if (arg.X == 0)
                switch (arg.Y)
                {
                    case 1:
                        return Direction.Down;
                    case -1:
                        return Direction.Up;
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
            switch (direction)
            {
                case Direction.Up:
                    return new Point(0, -1);
                case Direction.Down:
                    return new Point(0, 1);
                case Direction.Left:
                    return new Point(-1, 0);
                case Direction.Right:
                    return new Point(1, 0);
                default:
                    throw new Exception("Wrong argument");
            }
        }
    }
}