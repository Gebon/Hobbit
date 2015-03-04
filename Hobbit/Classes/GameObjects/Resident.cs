using System.Drawing;
using Hobbit.Interfaces;

namespace Hobbit.Classes.GameObjects
{
    public class Resident : IResident
    {
        public Point Location { get; set; }
        public string Name { get; set; }
        public short Health { get; set; }
    }
}
