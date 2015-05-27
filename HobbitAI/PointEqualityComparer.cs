using System.Collections.Generic;
using HelperLibrary;

namespace HobbitAI
{
    public class PointEqualityComparer : IEqualityComparer<Point>
    {
        public bool Equals(Point x, Point y)
        {
            return x.X == y.X && x.Y == y.Y;
        }

        public int GetHashCode(Point obj)
        {
            return obj.X << 16 + obj.Y;
        }
    }
}