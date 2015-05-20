using System.Collections.Generic;
using Hobbit.Classes.GameObjects;

namespace Hobbit.Interfaces
{
    public interface IForest : IEnumerable<IMapElement>
    {
        int Width { get; }
        int Height { get; }
        IMap Map { get; }
        List<IResident> Residents { get; } 
        bool SetResidentAt(IResident resident);
        bool MoveResidentTo(Direction direction, IResident resident);
    }
}
