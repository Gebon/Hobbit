using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using HelperLibrary;
using Hobbit.Classes;
using Hobbit.Classes.GameObjects;
using Hobbit.Interfaces;
using Newtonsoft.Json;
using Server;

namespace Hobbit
{
    public struct PlayerInfo
    {
        public int Health { get; set; }
        public Point StartPosition { get; set; }
        public Point Destination { get; set; }
    }
    class Server
    {
        private Socket visualaiser;
        private readonly List<Tuple<Player, Socket>> players = new List<Tuple<Player, Socket>>();

        private readonly IForest forest;
        private LastMoveInfo lastMoveInfo;

        private static readonly Dictionary<Type, MapCell> ToMapCell = new Dictionary<Type, MapCell>
        {
            {typeof(Wall), MapCell.Wall},
            {typeof(Road), MapCell.Road},
            {typeof(Trap), MapCell.Trap},
            {typeof(Health), MapCell.Health}
        };

        private readonly Direction[] toDirection = new[] { Direction.Up, Direction.Right, Direction.Down, Direction.Left };
        private readonly long playersCount;
        private PlayerInfo[] playersInfo;

        public Server()
        {
            dynamic data = JsonConvert
                .DeserializeObject(File.ReadAllText("config.json"));

            string mapPath = data.map_path;
            var map = new MapLoader(mapPath).LoadMap();
            playersCount = data.players_count;
            playersInfo = new PlayerInfo[playersCount];
            for (int i = 0; i < playersCount; i++)
            {
                Point start = new Point((int)data[i.ToString()].start_position.X, (int)data[i.ToString()].start_position.Y);
                Point destination = new Point((int)data[i.ToString()].destination.X, (int)data[i.ToString()].destination.Y);
                int health = data[i.ToString()].health;
                playersInfo[i] = new PlayerInfo { Destination = destination, StartPosition = start, Health = health };
            }
            forest = new Forest(map);
        }
        public static void Main(string[] args)
        {
            var server = new Server();
            server.Listen();

            while (true)
            {
                server.lastMoveInfo = new LastMoveInfo
                {
                    ChangedCells = new Tuple<Point, int>[0],
                    GameOver = false,
                    PlayersChangedPosition = new Tuple<int, Point, int>[0]
                };

                var answer = Serializer.Deserialize<Answer>(HelperMethods.ReceiveData(server.visualaiser));
                if (answer.AnswerCode != 0)
                    throw new Exception(String.Format("Bad answer code {0}", answer.AnswerCode));
                server.MakeNextMove();
                server.visualaiser.Send(Serializer.Serialize(server.lastMoveInfo));
            }
        }

        private void MakeNextMove()
        {
            foreach (var player in players)
            {
                var changedCells = new List<Tuple<Point, int>>();
                var changedPlayers = new List<Tuple<int, Point, int>>();
                var move = HelperMethods.ReceiveData(player.Item2)[0];
                var moveDirection = toDirection[move];

                var resident = forest.Residents.First(x => x.Id == player.Item1.Id);
                var health = resident.Health;
                if (forest.MoveResidentTo(moveDirection, resident))
                {
                    health = resident.Health - health;
                    player.Item2.Send(Serializer.Serialize(new MoveResultInfo
                    {
                        Result = 0,
                        VisibleMap = new[,]
                        {
                            {(int) ToMapCell[forest.Map[resident.Location].GetType()]}
                        }
                    }));
                    changedPlayers.Add(new Tuple<int, Point, int>(player.Item1.Id, resident.Location, resident.Health));
                }
                if (health > 0)
                    changedCells.Add(new Tuple<Point, int>(resident.Location, (int)MapCell.Road));
                changedCells.AddRange(lastMoveInfo.ChangedCells);
                changedPlayers.AddRange(lastMoveInfo.PlayersChangedPosition);

                changedCells.AddRange(lastMoveInfo.ChangedCells);
                lastMoveInfo.ChangedCells = changedCells.ToArray();

                changedPlayers.AddRange(lastMoveInfo.PlayersChangedPosition);
                lastMoveInfo.PlayersChangedPosition = changedPlayers.ToArray();

            }
        }

        private void Listen()
        {
            var listener = new TcpListener(new IPAddress(0x0100007f), 6002);
            listener.Start();
            listener.Server.ReceiveTimeout = 1000;

            while (!Ready())
            {
                var client = listener.AcceptSocket();
                client.ReceiveTimeout = 0;
                var buffer = HelperMethods.ReceiveData(client);

                var helloPacket = Serializer.Deserialize<Hello>(buffer);
                if (helloPacket.IsVisualizator)
                {
                    var worldInfo = GenerateWorldInfo();
                    client.Send(Serializer.Serialize(worldInfo));

                    visualaiser = client;
                }
                else
                    lock (players)
                    {
                        var player = new Player(new Guid().GetHashCode(), helloPacket.Name,
                            playersInfo[players.Count].StartPosition,
                            playersInfo[players.Count].Destination,
                            playersInfo[players.Count].Health);
                        players.Add(new Tuple<Player, Socket>(player, client));
                        forest.SetResidentAt(new Resident(player.Id, player.Nick, (short)player.Hp,
                            player.StartPosition));
                        var clientInfo = new ClientInfo
                        {
                            Hp = player.Hp,
                            MapSize = new Point(forest.Width, forest.Height),
                            StartPosition = player.StartPosition,
                            Target = player.Target,
                            VisibleMap = new[,]
                            {
                                {(int) MapCell.Road}
                            }
                        };
                        client.Send(Serializer.Serialize(clientInfo));
                    }
            }
        }

        private bool Ready()
        {
            return visualaiser != null && players.Count == playersCount;
        }

        private WorldInfo GenerateWorldInfo()
        {
            var map = new int[forest.Width, forest.Height];
            foreach (var mapElem in forest)
            {
                map[mapElem.Location.X, mapElem.Location.Y] = (int)ToMapCell[mapElem.GetType()];
            }

            var worldInfo = new WorldInfo { Map = map, Players = players.Select(x => x.Item1).ToArray() };

            return worldInfo;
        }
    }
}
