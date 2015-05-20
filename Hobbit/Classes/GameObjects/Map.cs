using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HelperLibrary;
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
            Width = width;
            Height = height;
            map = new IMapElement[height][];
            for (var i = 0; i < height; i++)
            {
                map[i] = new IMapElement[width];
            }
        }

        public override int GetHashCode()
        {
            return this.Aggregate(0, (current, item) => current ^ item.GetHashCode());
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

        public int Width { get; private set; }

        public int Height { get; private set; }

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