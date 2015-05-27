using System;
using System.Net.Sockets;
using HelperLibrary;
using Server;

namespace HobbitAI
{
    static class Program
    {

        public static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { ReceiveTimeout = 0 };
            socket.Connect(args[0], int.Parse(args[1]));

            socket.Send(Serializer.Serialize(new Hello { IsVisualizator = false, Name = "BigBear" }));

            var clientInfo = Serializer.Deserialize<ClientInfo>(socket);

            var position = clientInfo.StartPosition;
            var target = clientInfo.Target;
            var heigth = clientInfo.MapSize.X;
            var width = clientInfo.MapSize.Y;
            var hp = clientInfo.Hp;

            var mapInfo = GenerateMapInfo(width, heigth, position, clientInfo.VisibleMap);

            var ai = new AI();
            while (true)
            {
                var direction = ai.GetNextTurn(position, target, mapInfo, hp);
                socket.Send(Serializer.Serialize(new Move {Direction = (int)direction}));
                var moveResultInfo = Serializer.Deserialize<MoveResultInfo>(socket);
                switch (moveResultInfo.Result)
                {
                    case 2:
                        Environment.Exit(0);
                        break;
                    case 0:
                        position = new Point(position.X + DirectionResolver.ToPoint(direction).X,
                            position.Y + DirectionResolver.ToPoint(direction).Y);
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
                    mapInfo[new Point(position.X + y + delta, position.Y + x + delta)] = (MapCell)visibleMap[x, y];
                }
            }
        }

        private static MapInfo GenerateMapInfo(int width, int heigth, Point playerPosition, int[,] visibleMap)
        {
            var mapInfo = new MapInfo(width, heigth);
            UpdateMapInfo(mapInfo, playerPosition, visibleMap);
            return mapInfo;
        }
    }
}
