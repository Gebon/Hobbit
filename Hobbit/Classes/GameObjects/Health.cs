using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hobbit.Interfaces;

namespace Hobbit.Classes
{
    public class Health : IMapElement
    {
        public Point Location { get; set; }

        public bool GoToMe(IResident resident)
        {
            resident.Location = Location;
            resident.Health++;
            return true;
        }
    }
}
