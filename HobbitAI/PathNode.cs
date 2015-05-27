using HelperLibrary;

namespace HobbitAI
{
    internal class PathNode
    {
        public Point Position { get; set; }
        public int PathLengthFromStart { get; set; }
        public PathNode CameFrom { get; set; }
        public int HeuristicEstimatePathLength { get; set; }
        public int EstimateFullPathLength
        {
            get
            {
                return PathLengthFromStart + HeuristicEstimatePathLength;
            }
        }
    }
}