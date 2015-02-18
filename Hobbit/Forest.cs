using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Hobbit
{
    public enum Direction
    {
        Up,
        Down, 
        Left, 
        Right
    }


    public interface IMap
    {
        MapElement this[Point index] { get; set; }
    }

    public enum MapElement
    {
        Wall,
        Road,
        Resident,
        Health,
        Trap
    }

    public interface IForest
    {
        bool SetResidentAt(Point location, string name, int health);
        bool MoveResidentTo(Point location, IResident resident);
    }

    public class Forest : IForest
    {
        private IMap map;
        public bool SetResidentAt(Point location, string name, int health)
        {
            throw new NotImplementedException();
        }

        public bool MoveResidentTo(Point location, IResident resident)
        {
            var newLocation = resident.Location + (Size) location;
            var elem = map[newLocation];
            return true;
        }
    }

    public abstract class AbstractMapElement
    {
        public Point Location { get; set; }
        public abstract void GoToMe(IResident resident);
    }

    public class Wall : AbstractMapElement
    {
        public override void GoToMe(IResident resident)
        {
            
        }
    }

    public class Road : AbstractMapElement
    {
        public override void GoToMe(IResident resident)
        {
            resident.Location = Location;
        }
    }

    public interface IVisualiser
    {
        
    }

    public interface IResident
    {
        Point Location { get; set; }
    }

    public interface ILoader : IDisposable
    {
        IMap LoadMap();
    }

    public interface ITrap
    {
        
    }

    public interface IHealth
    {
         
    }
}
