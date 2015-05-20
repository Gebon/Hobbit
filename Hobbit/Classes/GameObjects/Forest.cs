using System;
using System.Collections;
using System.Collections.Generic;
using HelperLibrary;
using Hobbit.Interfaces;

namespace Hobbit.Classes.GameObjects
{
    public enum Direction
    {
        Up = 0,
        Down = 2,
        Left = 3,
        Right = 1
    }

    [Serializable]
    public class Forest : IForest
    {
        private IMap map;
        private readonly List<IResident> residents = new List<IResident>();

        public int Width
        {
            get { return map.Width; }
        }

        public int Height
        {
            get { return map.Height; }
        }

        public IMap Map
        {
            get { return map; }
        }

        public List<IResident> Residents { get { return residents; } }

        public Forest(IMap map)
        {
            this.map = map;
        }
        public bool SetResidentAt(IResident resident)
        {
            while (map[resident.Location].GoToMe(resident))
            {
                Residents.Add(resident);
                return true;
            }
            return false;
        }


        private readonly Dictionary<Direction, Point> directionResolver = new Dictionary<Direction, Point>
        {
            {Direction.Up, new Point(0, -1)},
            {Direction.Down, new Point(0, 1)},
            {Direction.Left, new Point(-1, 0)},
            {Direction.Right, new Point(1, 0)}
        };
        public bool MoveResidentTo(Direction direction, IResident resident)
        {
            var locationDelta = directionResolver[direction];
            var newLocation = new Point(resident.Location.X + locationDelta.X, resident.Location.Y + locationDelta.Y);
            try
            {
                var health = resident.Health;
                var elem = map[newLocation];
                var result = elem.GoToMe(resident);

                if (resident.Health - health > 0)
                    map[newLocation] = new Road();

                return result;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerator<IMapElement> GetEnumerator()
        {
            return map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
