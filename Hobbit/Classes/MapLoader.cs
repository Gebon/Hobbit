using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HelperLibrary;
using Hobbit.Classes.GameObjects;
using Hobbit.Interfaces;

namespace Hobbit.Classes
{
    public class MapLoader : ILoader
    {
        private string filePath;
        private Dictionary<char, Func<IMapElement>> mapElementsFactory = new Dictionary<char, Func<IMapElement>>
        {
            {'0', () => new Road()},
            {'1', () => new Wall()}
        }; 

        public MapLoader(string path)
        {
            filePath = path;
        }

        public IMap LoadMap()
        {
            var lines = File.ReadAllLines(filePath);
            var size = lines[0]
                .Split(' ')
                .Select(int.Parse)
                .ToList();
            var result = new Map(size[0], size[1]);
            var x = 0;
            var y = 0;
            foreach (var line in lines.Skip(1))
            {
                foreach (var ch in line)
                {
                    result[new Point(x, y)] = mapElementsFactory[ch]();
                    x++;
                }
                x = 0;
                y++;
            }
            
            return result;
        }
    }
}