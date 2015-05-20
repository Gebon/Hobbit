

using HelperLibrary;

namespace Hobbit.Interfaces
{
    public interface IResident
    {
        int Id { get; }
        Point Location { get; set; }
        string Name { get; set; }
        int Health { get; set; }
    }
}