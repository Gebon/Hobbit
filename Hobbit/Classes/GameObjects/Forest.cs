using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Hobbit.Interfaces;

namespace Hobbit.Classes.GameObjects
{
    public class Forest : IForest
    {
        private IMap map;
        private readonly List<IResident> residents = new List<IResident>();
        public List<IResident> Residents { get { return residents; } }

        public Forest(IMap map)
        {
            this.map = map;
        }
        public bool SetResidentAt(Point location, string name, short health)
        {
            var resident = new Resident { Location = location, Name = name, Health = health };
            while (map[location].GoToMe(resident))
            {
                Residents.Add(resident);
                return true;
            }
            return false;
        }

        public bool MoveResidentTo(Point location, IResident resident)
        {
            var newLocation = resident.Location + (Size)location;
            try
            {
                var elem = map[newLocation];
                return elem.GoToMe(resident);
            }
            catch (Exception)
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
