using System;
using HelperLibrary;

namespace HobbitAI
{
    public class MapInfo : ICloneable
    {
        private MapCell[][] map;
        public int Width { get; private set; }
        public int Heigth { get; private set; }

        public MapInfo(int width, int heigth)
        {
            map = new MapCell[heigth][];
            for (var i = 0; i < heigth; i++)
            {
                map[i] = new MapCell[width];
                for (var j = 0; j < width; j++)
                {
                    map[i][j] = MapCell.Undefined;
                }
            }
            Width = width;
            Heigth = heigth;
        }

        private MapInfo(MapCell[][] map, int width, int heigth)
        {
            Width = width;
            Heigth = heigth;
            this.map = map;
        }
        public MapCell this[Point location]
        {
            get
            {
                return Bounds(location) ? map[location.Y][location.X] : MapCell.None;
            }
            set
            {
                if (Bounds(location))
                    map[location.Y][location.X] = value;
            }
        }

        public bool Bounds(Point location)
        {
            return location.X >= 0 && location.X < Width && location.Y >= 0 && location.Y < Heigth;
        }

        public object Clone()
        {
            var result = new MapCell[Heigth][];
            for (var i = 0; i < Heigth; i++)
            {
                result[i] = new MapCell[Width];
                map[i].CopyTo(result[i], 0);
            }
            return new MapInfo(result, Width, Heigth);
        }
    }
}