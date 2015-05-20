
using HelperLibrary;

namespace Hobbit.Interfaces
{
    public interface IMapElement
    {
        Point Location { get; set; }
        bool GoToMe(IResident resident);
        bool Equals(object obj);
        int GetHashCode();
    }
}