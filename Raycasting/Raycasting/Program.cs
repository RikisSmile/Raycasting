using System;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO.Pipes;
using System.Net;
using System.Net.NetworkInformation;

namespace Raycast
{
    class Program
    {
        private const int ScreenWidth = 100;
        private const int ScreenHeight = 50;

        private const int MapWidth = 32;
        private const int MapHeight = 32;
        private const int Depth = 20;

        private const double fov = Math.PI / 3;

        private static double _playerX = 4;
        private static double _playerY = 4;
        private static double _playerA = 0;

        private static string _map = "";

        private static readonly char[] Screen = new char[ScreenWidth * ScreenHeight];

        static void Main()
        {
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
            Console.SetBufferSize(ScreenWidth, ScreenHeight);
            Console.CursorVisible = false;

            _map += "################################";
            _map += "#.....................##########";
            _map += "#.....................#........#";
            _map += "#.................... #........#";
            _map += "#.....................#........#";
            _map += "#.....................#........#";
            _map += "#.....................######...#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#########################....###";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#....###.......................#";
            _map += "#......#.......................#";
            _map += "#......#.......................#";
            _map += ".......#.......................#";
            _map += "################################";

            DateTime dateTimeFrom = DateTime.Now;

            while (true)
            {
                DateTime dateTimeTo = DateTime.Now;
                double elapsedTime =(dateTimeTo - dateTimeFrom).TotalSeconds;
                dateTimeFrom = DateTime.Now;

                if (Console.KeyAvailable)
                {
                    ConsoleKey input = Console.ReadKey(true).Key;
                    switch (input)
                    {
                        case ConsoleKey.A:
                            _playerA += 3 * elapsedTime;
                            break;
                        case ConsoleKey.D:
                            _playerA -= 3 * elapsedTime;
                            break;
                        case ConsoleKey.W:
                            _playerX += Math.Sin(_playerA) * 10 * elapsedTime;
                            _playerY += Math.Cos(_playerA) * 10 * elapsedTime;
                            break;
                    }
                }
                
                for (int x = 0; x < ScreenWidth; x++)
                {
                    // Calculating raycast angle by formula;
                    double rayAngle = _playerA + (fov / 2 - x * fov / ScreenWidth);

                    // Calculating raycast X & Y;
                    double rayX = Math.Sin(rayAngle);
                    double rayY = Math.Cos(rayAngle);

                    double distanceToWall = 0;
                    bool hitWall = false;

                    while (!hitWall && distanceToWall < Depth)
                    {
                        distanceToWall += 0.1;

                        int testX = (int) (_playerX + rayX * distanceToWall);
                        int testY = (int) (_playerY + rayY * distanceToWall);

                        if (testX < 0 || testX >= Depth + _playerX || testY < 0 || testY >= Depth + _playerY)
                        {
                            hitWall = true;
                            distanceToWall = Depth;
                        }
                        else
                        {
                            char testCell = _map[testY * MapWidth + testX];

                            if (testCell == '#')
                            {
                                hitWall = true;
                            }
                        }
                    }

                    // Calculating Ceiling and floor numbers;
                    int ceiling = (int) (ScreenHeight / 2d - ScreenHeight / distanceToWall);
                    int floor = ScreenHeight -ceiling;


                    // Like color shades
                    char wallShade;

                    // Calculting wall shade color 
                    if (distanceToWall <= Depth / 4d)
                        wallShade = '\u2588';
                    else if (distanceToWall < Depth / 3d)
                        wallShade = '\u2593';
                    else if (distanceToWall < Depth / 2d)
                        wallShade = '\u2592';
                    else if (distanceToWall < Depth)
                        wallShade = '\u2591';
                    else
                        wallShade = '#';

                    // Adding every ASCII symbol to every pixel in Screen
                    for (int y = 0; y < ScreenHeight; y++)
                    {
                        if (y <= ceiling)
                        {
                            Screen[y * ScreenWidth + x] = ' ';
                        }
                        else if (y > ceiling && y <= floor)
                        {
                            Screen[y * ScreenWidth + x] = wallShade;
                        }
                        else
                        {
                            Screen[y * ScreenWidth + x] = '.';
                        }
                    }
                }

                Console.SetCursorPosition(0, 0);
                Console.Write(Screen);
            }
        }
    }
}