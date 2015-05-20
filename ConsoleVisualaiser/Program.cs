using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using HelperLibrary;
using Hobbit.Classes.GameObjects;
using Hobbit.Interfaces;
using Server;

namespace ConsoleVisualaiser
{
    class ConsoleVisualaiser
    {
        private Socket client;
        private static Dictionary<Type, char> mapElementsRepresentation = new Dictionary<Type, char>
        {
            {typeof(Wall), '*'},
            {typeof(Road), '.'},
            {typeof(Health), 'h'},
            {typeof(Trap), 't'}
        };

        private static Dictionary<MapCell, Func<IMapElement>> factory = new Dictionary<MapCell, Func<IMapElement>>
        {
            {MapCell.Wall, () => new Wall()},
            {MapCell.Road, () => new Road()},
            {MapCell.Trap, () => new Trap()},
            {MapCell.Health, () => new Health()}
        };

        private static Func<IResident, char> residentRepresentation = resident => resident.Name.First();
        static void Main(string[] args)
        {
            var visualiser = new ConsoleVisualaiser();
            visualiser.Connect().Wait();

        }

        public async Task Connect()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect("127.0.0.1", 6002);

            var helloPacket = new Hello { IsVisualizator = true };
            var serializedHelloPacket = Serializer.Serialize(helloPacket);

            client.Send(serializedHelloPacket);
            var worldInfo = Serializer.Deserialize<WorldInfo>(HelperMethods.ReceiveData(client));
            var answer = new Answer {AnswerCode = 0};
            client.Send(Serializer.Serialize(answer));
            var forest = GenerateForest(worldInfo);


            VisualizeMap(forest);

            while (true)
            {
                var lastMoveInfo = Serializer.Deserialize<LastMoveInfo>(HelperMethods.ReceiveData(client));
                foreach (var changedCell in lastMoveInfo.ChangedCells)
                {
                    forest.Map[changedCell.Item1] = factory[(MapCell) changedCell.Item2]();
                }
                foreach (var tuple in lastMoveInfo.PlayersChangedPosition)
                {
                    var resident = forest.Residents.First(x => x.Id == tuple.Item1);
                    resident.Location = tuple.Item2;
                    resident.Health = tuple.Item3;
                }
                VisualizeMap(forest);
                Thread.Sleep(1000);
                client.Send(Serializer.Serialize(new Answer {AnswerCode = 0}));
            }
        }

        private static Forest GenerateForest(WorldInfo worldInfo)
        {
            var map = new Map(worldInfo.Map.GetLength(0), worldInfo.Map.GetLength(1));
            for (var i = 0; i < map.Width; i++)
            {
                for (var j = 0; j < map.Height; j++)
                {
                    map[new Point(i, j)] = factory[(MapCell) worldInfo.Map[i, j]]();
                }
            }
            var forest = new Forest(map);
            foreach (var player in worldInfo.Players ?? new Player[0])
            {
                forest.SetResidentAt(new Resident(player.Id, player.Nick, (short) player.Hp, player.StartPosition));
            }
            return forest;
        }

        public void VisualizeMap(IForest forest)
        {
            Console.Clear();
            var endPoint = new Point(0, 0);
            VisualizeMap(forest, ref endPoint);
            VisualiseResidents(forest);

            GenerateMapLegend(forest, endPoint);
        }

        private void GenerateMapLegend(IForest forest, Point endPoint)
        {
            Console.SetCursorPosition(endPoint.X, endPoint.Y);
            Console.WriteLine("\r\n\r\n\r\n---------------------------------------");
            Console.WriteLine("*: wall\r\n.: road\r\nh: health\r\nt: trap");
            foreach (var resident in forest.Residents)
            {
                Console.WriteLine("{0}: {1}", resident.Name.First(), resident.Name);
            }
        }

        private void VisualiseResidents(IForest forest)
        {
            foreach (var resident in forest.Residents)
            {
                Console.SetCursorPosition(resident.Location.X, resident.Location.Y);
                Console.Write(residentRepresentation(resident));
            }
        }

        private void VisualizeMap(IForest forest, ref Point endPoint)
        {
            foreach (var mapElement in forest)
            {
                Console.SetCursorPosition(mapElement.Location.X, mapElement.Location.Y);
                Console.Write(mapElementsRepresentation[mapElement.GetType()]);
                endPoint = mapElement.Location;
            }
        }
    }
}
