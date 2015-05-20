using System;
using ForestSolverPackages;
using Hobbit.Classes.GameObjects;
using Hobbit.Interfaces;

namespace HobbitAI
{
    public class AiController
    {
        private readonly IResident resident;
        private readonly Point destination;
        private readonly MapInfo mapInfo = new MapInfo();
        public IAi Ai { get; set; }

        public AiController(IAi ai, IResident resident, Point destination)
        {
            this.resident = resident;
            this.destination = destination;
            Ai = ai;
        }

        public void MakeNextMove(Func<Direction, IResident, bool> move)
        {
            var direction = Ai.GetNextTurn(resident, destination, mapInfo);
            var oldHealth = resident.Health;

            var moveResult = move(direction, resident);
            if (moveResult)
            {
                if (oldHealth > resident.Health)
                    mapInfo[resident.Location] = new Trap();
                else mapInfo[resident.Location] = new Road();
            }
            else mapInfo[resident.Location] = new Wall();
        }
    }
}
