using HelperLibrary;

namespace HobbitAI
{
    public class MapInfo
    {
        private MapCell[][] map;
        public int Width { get; private set; }
        public int Heigth { get; private set; }

        public MapInfo(int width, int heigth)
        {
            map = new MapCell[heigth][];
            for (int i = 0; i < heigth; i++)
            {
                map[i] = new MapCell[width];
                for (int j = 0; j < width; j++)
                {
                    map[i][j] = MapCell.Undefined;
                }
            }
            Width = width;
            Heigth = heigth;
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
    }
}