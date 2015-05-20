using System.Collections.Generic;
using HelperLibrary;
using Hobbit.Interfaces;

namespace HobbitAI
{
    public class MapInfo
    {
        private Dictionary<Point, MapCell> map = new Dictionary<Point, MapCell>();

        public MapCell this[Point location]
        {
            get
            {
                return map.ContainsKey(location) ? map[location] : MapCell.None;
            }
            set {
                if (!map.ContainsKey(location))
                {
                    map.Add(location, value);
                }
                map[location] = value;
            }
        }
    }
}