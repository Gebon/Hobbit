using System.Drawing;
using Hobbit.Interfaces;

namespace Hobbit.Classes.GameObjects
{
    public class Road : IMapElement
    {
        public Point Location { get; set; }

        public bool GoToMe(IResident resident)
        {
            resident.Location = Location;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null) return false;
            var other = obj as Road;
            return other != null && Location == other.Location;
        }

        public override int GetHashCode()
        {
            return Location.GetHashCode();
        }
    }
}