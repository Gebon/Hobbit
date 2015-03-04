using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Hobbit.Interfaces;

namespace Hobbit.Classes.GameObjects
{
    public class Map : IMap
    {
        private IMapElement[][] map;
        public IEnumerator<IMapElement> GetEnumerator()
        {
            foreach (var abstractMapElements in map)
            {
                foreach (var abstractMapElement in abstractMapElements)
                {
                    yield return abstractMapElement;
                }
            }
        }

        public Map(int width, int height)
        {
            map = new IMapElement[height][];
            for (int i = 0; i < height; i++)
            {
                map[i] = new IMapElement[width];
            }
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var item in this)
            {
                hash = hash ^ item.GetHashCode();
            }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            var other = obj as Map;
            return other != null && !(from item in other from abstractMapElement in this where !item.Equals(abstractMapElement) select item).Any();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IMapElement this[Point index]
        {
            get { return map[index.Y][index.X]; }
            set
            {
                map[index.Y][index.X] = value;
                value.Location = index;
            }
        }
    }
}