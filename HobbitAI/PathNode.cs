using HelperLibrary;

namespace HobbitAI
{
    internal class PathNode
    {
        public PathNode(Point position)
        {
            Position = position;
        }

        public Point Position { get; }
        public int PathLengthFromStart { get; set; }
        public PathNode CameFrom { get; set; }
        public int HeuristicEstimatePathLength { get; set; }
        public int EstimateFullPathLength => PathLengthFromStart + HeuristicEstimatePathLength;

        public override bool Equals(object obj)
        {
            var other = obj as PathNode;
            if (other == null)
                return false;
            return ReferenceEquals(this, other) || (Position.X == other.Position.X && Position.Y == other.Position.Y);
        }

        public override int GetHashCode()
        {
            return Position.X << 16 + Position.Y;
        }
    }
}