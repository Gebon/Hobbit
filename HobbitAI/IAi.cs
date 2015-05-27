using HelperLibrary;

namespace HobbitAI
{
    public interface IAi
    {
        Direction GetNextTurn(Point start, Point destination, MapInfo mapInfo, int hp);
    }
}