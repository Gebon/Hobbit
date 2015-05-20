using System.Collections.Generic;
using HelperLibrary;

namespace Hobbit.Interfaces
{
    public interface IMap : IEnumerable<IMapElement>
    {
        int Width { get; }
        int Height { get; }
        IMapElement this[Point index] { get; set; }
    }
}