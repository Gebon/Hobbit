using System.Drawing;

namespace Hobbit.Interfaces
{
    public interface IResident
    {
        Point Location { get; set; }
        string Name { get; set; }
        short Health { get; set; }
    }
}