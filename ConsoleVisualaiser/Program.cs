using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Hobbit.Classes.GameObjects;
using Hobbit.Interfaces;

namespace ConsoleVisualaiser
{
    class ConsoleVisualaiser : IVisualaiser
    {
        private Dictionary<IMapElement, char> mapElementsRepresentation = new Dictionary<IMapElement, char>
        {
        }; 
        static void Main(string[] args)
        {
        }

        public void VisualiseMap(IForest forest)
        {
            foreach (var mapElement in forest)
            {
                Console.SetCursorPosition(mapElement.Location.X, mapElement.Location.Y);
                //Console.Write();
            }
        }
    }
}
