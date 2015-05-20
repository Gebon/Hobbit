using HelperLibrary;
using Hobbit.Classes.GameObjects;
using Hobbit.Interfaces;

namespace HobbitAI
{
    public interface IAi
    {
        Direction GetNextTurn(Point start, Point destination, MapInfo mapInfo);
    }
}