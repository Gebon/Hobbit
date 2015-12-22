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
            var name = "BigBear";
            if (args.Length == 3)
                name = args[2];

            socket.Send(Serializer.Serialize(new Hello { IsVisualizator = false, Name = name }));

            var clientInfo = Serializer.Deserialize<ClientInfo>(socket);

            var position = clientInfo.StartPosition;
            var target = clientInfo.Target;
            var heigth = clientInfo.MapSize.X;
            var width = clientInfo.MapSize.Y;
            var hp = clientInfo.Hp;

            var mapInfo = GenerateMapInfo(width, heigth, position, clientInfo.VisibleMap);

            var ai = new AI(mapInfo);
            while (true)
            {
                var direction = ai.GetNextTurn(position, target, mapInfo, hp);
                socket.Send(Serializer.Serialize(new Move { Direction = (int)direction }));
                var moveResultInfo = Serializer.Deserialize<MoveResultInfo>(socket);
                switch (moveResultInfo.Result)
                {
                    case 2:
                        return;
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
            for (var i = 0; i < mapInfo.Heigth; i++)
            {
                for (var j = 0; j < mapInfo.Width; j++)
                {
                    if (mapInfo[new Point(i, j)] == MapCell.Player)
                        mapInfo[new Point(i, j)] = MapCell.Undefined;
                }
            }
            var delta = -visibleMap.GetLength(0) / 2;
            for (var x = 0; x < visibleMap.GetLength(0); x++)
            {
                for (var y = 0; y < visibleMap.GetLength(1); y++)
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
