using System;
using System.Collections.Generic;
using System.Linq;
using HelperLibrary;
using Hobbit.Classes.GameObjects;

namespace HobbitAI
{
    public class AI : IAi
    {
        private List<Point> visited = new List<Point>();
        public Direction GetNextTurn(Point start, Point destination, MapInfo mapInfo)
        {
            Point preferredTurn;
            if (Math.Abs(destination.X - start.X) > Math.Abs(destination.Y - start.Y))
                preferredTurn = new Point(Normalize(destination.X - start.X), 0);
            else
                preferredTurn = new Point(0, Normalize(destination.Y - start.Y));

            var nextTurn = GetNeighbours(start, mapInfo).OrderByDescending(x => GetHeuristic(mapInfo, x, destination)).First();
            return Direction.Right;
        }

        private int Normalize(int number)
        {
            return number/Math.Abs(number);
        }
        private Direction ToDirection(Point arg)
        {
            throw new NotImplementedException();
        }

        private Point ToDeltas(Point point, Point location)
        {
            throw new NotImplementedException();
        }

        private int GetHeuristic(MapInfo mapInfo, Point point, Point destination)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Point> GetNeighbours(Point location, MapInfo mapInfo)
        {
            throw new NotImplementedException();
        }
    }
}
