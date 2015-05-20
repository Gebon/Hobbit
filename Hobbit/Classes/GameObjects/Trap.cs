﻿using HelperLibrary;
using Hobbit.Interfaces;

namespace Hobbit.Classes.GameObjects
{
    public class Trap : IMapElement
    {
        public Point Location { get; set; }

        public bool GoToMe(IResident resident)
        {
            resident.Location = Location;
            resident.Health--;
            return true;
        }
    }
}
