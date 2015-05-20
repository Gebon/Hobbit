using System;
using System.Collections.Generic;
using System.Net.Sockets;
using HelperLibrary;
using Hobbit.Classes.GameObjects;
using Server;

namespace HobbitAI
{
    class Program
    {
        private static Dictionary<Direction, Point> directionResolver = new Dictionary<Direction, Point>
        {
            {Direction.Up, new Point(0, -1)},
            {Direction.Down, new Point(0, 1)},
            {Direction.Left, new Point(-1, 0)},
            {Direction.Right, new Point(1, 0)}
        };
        public static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { ReceiveTimeout = 0 };
            socket.Connect("127.0.0.1", 20000);

            socket.Send(Serializer.Serialize(new Hello { IsVisualizator = false, Name = "Archi" }));

            var clientInfo = Serializer.Deserialize<ClientInfo>(HelperMethods.ReceiveData(socket));

            var position = clientInfo.StartPosition;
            var target = clientInfo.Target;
            var width = clientInfo.MapSize.X;
            var heigth = clientInfo.MapSize.Y;
            var hp = clientInfo.Hp;

            var mapInfo = GenerateMapInfo(width, heigth, position, clientInfo.VisibleMap);

            var ai = new AI();
            while (true)
            {
                var direction = ai.GetNextTurn(position, target, mapInfo);
                socket.Send(new[] { (byte)direction });
                var moveResultInfo = Serializer.Deserialize<MoveResultInfo>(HelperMethods.ReceiveData(socket));
                switch (moveResultInfo.Result)
                {
                    case 2:
                        Environment.Exit(0);
                        break;
                    case 1:
                        position = new Point(position.X + directionResolver[direction].X,
                            position.Y + directionResolver[direction].Y);
                        switch (mapInfo[position])
                        {
                            case MapCell.Health:
                                hp++;
                                break;
                            case MapCell.Trap:
                                hp--;
                                break;
                        }

                        break;
                }
                UpdateMapInfo(mapInfo, position, moveResultInfo.VisibleMap);
            }
        }

        private static void UpdateMapInfo(MapInfo mapInfo, Point position, int[,] visibleMap)
        {
            var delta = -visibleMap.GetLength(0) / 2;
            for (int x = 0; x < visibleMap.GetLength(0); x++)
            {
                for (int y = 0; y < visibleMap.GetLength(1); y++)
                {
                    mapInfo[new Point(position.X + x + delta, position.Y + y + delta)] = (MapCell)visibleMap[x, y];
                }
            }
        }

        private static MapInfo GenerateMapInfo(int width, int heigth, Point playerPosition, int[,] visibleMap)
        {
            var mapInfo = new MapInfo();

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < heigth; y++)
                {
                    mapInfo[new Point(x, y)] = MapCell.Undefined;
                }
            }
            UpdateMapInfo(mapInfo, playerPosition, visibleMap);
            return mapInfo;
        }
    }
}
