using System;
using HelperLibrary;
using Hobbit.Interfaces;

namespace Hobbit.Classes.GameObjects
{
    [Serializable]
    public class Resident : IResident
    {
        public Resident(int id, string name, short health, Point location)
        {
            Name = name;
            Health = health;
            Location = location;
            Id = id;
        }

        public int Id { get; private set; }

        public Point Location { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }

        public override string ToString()
        {
            return String.Format("{0}: {1}, {2}", Name, Health, Location);
        }
    }
}
