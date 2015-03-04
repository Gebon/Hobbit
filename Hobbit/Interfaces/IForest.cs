using System.Collections.Generic;
using System.Drawing;

namespace Hobbit.Interfaces
{
    public interface IForest : IEnumerable<IMapElement>
    {
        List<IResident> Residents { get; } 
        bool SetResidentAt(Point location, string name, short health);
        bool MoveResidentTo(Point location, IResident resident);
    }
}
