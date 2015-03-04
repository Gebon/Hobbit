using System.Drawing;

namespace Hobbit.Interfaces
{
    public interface IForest
    {
        bool SetResidentAt(Point location, string name, short health);
        bool MoveResidentTo(Point location, IResident resident);
    }
}
