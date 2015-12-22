using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using HelperLibrary;

namespace HobbitAI
{
    public static class ExtensionMethods
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
        {
            var result = enumerable.ToList();
            var n = result.Count;
            while (n > 1)
            {
                n--;
                var k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                var value = result[k];
                result[k] = result[n];
                result[n] = value;
            }
            return result;
        }
    }
    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom => Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId)));
    }

    // ReSharper disable once InconsistentNaming
    public class AI : IAi
    {
        private readonly HashSet<Point> visited = new HashSet<Point>(new PointEqualityComparer());
        private MapInfo previousMap;

        public AI(MapInfo previousMap)
        {
            this.previousMap = previousMap;
        }

        private static int GetCellCost(MapCell cell, int hp)
        {
            switch (hp)
            {
                case 1:
                case 2:
                    switch (cell)
                    {
                        case MapCell.Trap:
                            return 10;
                        case MapCell.Health:
                            return -10;
                        default:
                            return 0;
                    }

                case 3:
                    switch (cell)
                    {
                        case MapCell.Trap:
                            return 5;
                        case MapCell.Health:
                            return -5;
                        default:
                            return 0;
                    }
                default:
                    switch (cell)
                    {
                        default:
                            return 0;
                    }
            }
        }

        private delegate IEnumerable<PathNode> GetNeighboursFunc(PathNode node, GetHeuristicFunc getHeuristic);

        private delegate int GetHeuristicFunc(Point point);

        private delegate bool IsGoalFunc(Point point);
        public Direction GetNextTurn(Point start, Point destination, MapInfo mapInfo, int hp)
        {
            var comparer = new PointEqualityComparer();
            var isGoalForPathFinding = new IsGoalFunc(point => comparer.Equals(point, destination));
            var isGoalForHealthFinding = new IsGoalFunc(point => mapInfo[point] == MapCell.Health);

            var getHeuristicForPath = new GetHeuristicFunc(point => GetManhattanDistance(destination, point) + GetCellCost(mapInfo[point], hp));
            var getHeuristicForHealth = new GetHeuristicFunc(point => GetCellCost(mapInfo[point], hp));

            var getNeighboursForPathFinding = new GetNeighboursFunc((node, getHeuristic) => GetNeighbours(node, getHeuristic, mapInfo));
            var getNeighboursForHealthFinding =
                new GetNeighboursFunc((node, getHeuristic) =>
                    GetNeighbours(node, getHeuristic, mapInfo).Where(x => mapInfo[x.Position] != MapCell.Trap));

            var aStar = new Func<IsGoalFunc,
                                 GetHeuristicFunc,
                                 GetNeighboursFunc,
                                 IEnumerable<Point>>((isGoal, getHeuristic, getNeighbours) => AStar(start, isGoal, getNeighbours, getHeuristic));

            if (GetNeighbours(start).Any(x => mapInfo[x] == MapCell.Health) && previousMap[start] != MapCell.Trap && hp < 4)
            {
                var point = GetNeighbours(start).First(x => mapInfo[x] == MapCell.Health);
                previousMap = (MapInfo) mapInfo.Clone();
                return
                    DirectionResolver.ToDirection(new Point(point.X - start.X, point.Y - start.Y));
            }
            var path = aStar(isGoalForPathFinding, getHeuristicForPath, getNeighboursForPathFinding) ??
                           aStar(isGoalForHealthFinding, getHeuristicForHealth, getNeighboursForHealthFinding);

            var nextTurn = (path ?? new List<Point> { null, null }).ElementAt(1) ??
                             GetNeighbours(start).OrderBy(x => GetCellCost(mapInfo[x], hp)).First();
            nextTurn = new Point(nextTurn.X - start.X, nextTurn.Y - start.Y);
            previousMap = (MapInfo) mapInfo.Clone();
            return DirectionResolver.ToDirection(nextTurn);
        }

        private static int GetManhattanDistance(Point destination, Point point)
        {
            return Math.Abs(point.X - destination.X) + Math.Abs(point.Y - destination.Y);
        }

        private IEnumerable<Point> AStar(Point start,
                                         IsGoalFunc isGoal,
                                         GetNeighboursFunc getNeighbours,
                                         GetHeuristicFunc getHeuristic)
        {
            var comparer = new PointEqualityComparer();

            var closedSet = new Collection<PathNode>();
            var openSet = new Collection<PathNode>();

            var startNode = new PathNode(start)
            {
                CameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = getHeuristic(start)
            };
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                var currentNode = GetBestNode(openSet, startNode);

                if (isGoal(currentNode.Position))
                {
                    var path = GetPathForNode(currentNode);
                    foreach (var point in path)
                    {
                        visited.Add(point);
                    }
                    return path;
                }

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

        private PathNode GetBestNode(IEnumerable<PathNode> openSet, PathNode startNode)
        {
            return openSet.OrderBy(node =>
                node.EstimateFullPathLength + (visited.Contains(node.Position) ? 2 : 0))
                .ThenByDescending(x => Math.Max(Math.Abs(x.Position.X - startNode.Position.X), Math.Abs(x.Position.Y - startNode.Position.Y)))
                .First();
        }


        private static IEnumerable<PathNode> GetNeighbours(PathNode pathNode, GetHeuristicFunc getHeuristic, MapInfo mapInfo)
        {
            return from point in GetNeighbours(pathNode.Position).Shuffle()
                   where mapInfo.Bounds(point) && mapInfo[point] != MapCell.Wall && mapInfo[point] != MapCell.Player
                   select new PathNode(point)
                       {
                           CameFrom = pathNode,
                           PathLengthFromStart = pathNode.PathLengthFromStart + 1,
                           HeuristicEstimatePathLength = getHeuristic(point)
                       };
        }

        private static IEnumerable<Point> GetNeighbours(Point location)
        {
            var dx = new[] { 0, 1, 0, -1 };
            var dy = new[] { 1, 0, -1, 0 };
            for (var i = 0; i < 4; i++)
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
