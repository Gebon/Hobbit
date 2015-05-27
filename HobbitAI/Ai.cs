using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HelperLibrary;

namespace HobbitAI
{
    // ReSharper disable once InconsistentNaming
    public class AI : IAi
    {
        private readonly Dictionary<MapCell, int> cellCost = new Dictionary<MapCell, int>
        {
            {MapCell.Health, -3},
            {MapCell.Trap, 10},
            {MapCell.Player, 2},
            {MapCell.Road, 0},
            {MapCell.Wall, 20},
            {MapCell.Undefined, 0}
        };

        public Direction GetNextTurn(Point start, Point destination, MapInfo mapInfo, int hp)
        {
            var comparer = new PointEqualityComparer();
            var isGoalForPath = new Func<Point, bool>(point => comparer.Equals(point, destination));
            var isGoalForHealth = new Func<Point, bool>(point => mapInfo[point] == MapCell.Health);

            var getHeuristicForPath = new Func<Point, int>(point => GetManhattanDistance(destination, point) + cellCost[mapInfo[point]]);
            var getHeuristicForHealth = new Func<Point, int>(point => mapInfo[point] == MapCell.Trap ? 1 : (mapInfo[point] == MapCell.Health ? -1 : 0));

            var getNeighbours = new Func<PathNode, Func<Point, int>, IEnumerable<PathNode>>((node, getHeuristic) => GetNeighbours(node, getHeuristic, mapInfo));

            var aStar = new Func<Func<Point, bool>, Func<Point, int>, Point>((isGoal, getHeuristic) => AStar(start, isGoal, getNeighbours, getHeuristic));

            Point nextTurn;
            if (hp > 1)
            {
                nextTurn = aStar(isGoalForPath, getHeuristicForPath) ?? aStar(isGoalForHealth, getHeuristicForHealth);
            }
            else
                nextTurn = aStar(isGoalForHealth, getHeuristicForHealth);
            if (nextTurn == null)
            {
                var neighbour = GetNeighbours(start).OrderBy(x => mapInfo[x] == MapCell.Trap ? 10 : 0).First(x => mapInfo[x] != MapCell.Wall);
                return DirectionResolver.ToDirection(new Point(neighbour.X - start.X, neighbour.Y - start.Y));
            }
            nextTurn = new Point(nextTurn.X - start.X, nextTurn.Y - start.Y);
            return DirectionResolver.ToDirection(nextTurn);
        }

        private static int GetManhattanDistance(Point destination, Point point)
        {
            return Math.Abs(point.X - destination.X) + Math.Abs(point.Y - destination.Y);
        }

        private static Point AStar(Point start, Func<Point, bool> isGoal, Func<PathNode, Func<Point, int>, IEnumerable<PathNode>> getNeighbours, Func<Point, int> getHeuristic )
        {
            var comparer = new PointEqualityComparer();

            var closedSet = new Collection<PathNode>();
            var openSet = new Collection<PathNode>();

            PathNode startNode = new PathNode()
            {
                Position = start,
                CameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = getHeuristic(start)
            };
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                var currentNode = openSet.OrderBy(node =>
                  node.EstimateFullPathLength).First();
                if (isGoal(currentNode.Position))
                    return GetPathForNode(currentNode).ElementAt(1);

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (var neighbourNode in getNeighbours(currentNode, getHeuristic).Where(x => closedSet.All(y => !comparer.Equals(x.Position, y.Position))))
                {
                    var openNode = openSet.FirstOrDefault(node =>
                      comparer.Equals(node.Position, neighbourNode.Position));

                    if (openNode == null)
                        openSet.Add(neighbourNode);
                    else
                        if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                        {
                            openNode.CameFrom = currentNode;
                            openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                        }
                }
            }
            return null;
        }


        private static IEnumerable<PathNode> GetNeighbours(PathNode pathNode, Func<Point, int> getHeuristic, MapInfo mapInfo)
        {
            var dx = new[] { 0, 1, 0, -1 };
            var dy = new[] { 1, 0, -1, 0 };
            for (int i = 0; i < 4; i++)
            {
                var point = new Point(pathNode.Position.X + dx[i], pathNode.Position.Y + dy[i]);
                if (!mapInfo.Bounds(point) || mapInfo[point] == MapCell.Wall)
                    continue;
                var neighbour =
                    new PathNode
                    {
                        Position = point,
                        CameFrom = pathNode,
                        PathLengthFromStart = pathNode.PathLengthFromStart + 1,
                        HeuristicEstimatePathLength = getHeuristic(point)
                    };
                yield return neighbour;
            }
        }

        private IEnumerable<Point> GetNeighbours(Point location)
        {
            var dx = new[] { 0, 1, 0, -1 };
            var dy = new[] { 1, 0, -1, 0 };
            for (int i = 0; i < 4; i++)
            {
                yield return new Point(location.X + dx[i], location.Y + dy[i]);
            }
        }
        private static List<Point> GetPathForNode(PathNode pathNode)
        {
            var result = new List<Point>();
            var currentNode = pathNode;
            while (currentNode != null)
            {
                result.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }
            result.Reverse();
            return result;
        }
    }
}
