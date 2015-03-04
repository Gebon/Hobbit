using System.Collections.Generic;
using System.Drawing;

namespace Hobbit.Interfaces
{
    public interface IMap : IEnumerable<IMapElement>
    {
        IMapElement this[Point index] { get; set; }
    }
}