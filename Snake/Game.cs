using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Snake
{
    class Game
    {
        private bool debug;
        private bool gameOver;

        private int score;
        private int width, height;
        private int playerX, playerY;

        private int[] tailX, tailY;
        private int tailLength;

        Random rand;

        private int fruitX, fruitY;

        enum Direction { STOP = 0, LEFT, RIGHT, UP, DOWN };
        Direction dir;

        Thread inputThread;
        Thread drawThread;
        Thread logicThread;

        public Game()
        {
            debug = false;
            gameOver = false;

            score = 0;

            width = 30;
            height = 20;

            playerX = width / 2;
            playerY = height / 2;

            tailX = new int[100];
            tailY = new int[100];
            tailLength = 0;

            rand = new Random();

            fruitX = rand.Next(1, 29);
            fruitY = rand.Next(0, 20);

            dir = Direction.STOP;

            inputThread = new Thread(new ThreadStart(Input));
            drawThread = new Thread(new ThreadStart(Draw));
            logicThread = new Thread(new ThreadStart(Logic));
        }

        public void Start()
        {
            drawThread.Start();
            logicThread.Start();
            inputThread.Start();

            while (!gameOver)
            {
                Thread.Sleep(1000);
            }

            GameOver();
        }

        private void Input()
        {
            while(true)
            {
                char key = Console.ReadKey().KeyChar;

                switch (key)
                {
                    case 'w':
                        dir = Direction.UP;
                        break;
                    case 'a':
                        dir = Direction.LEFT;
                        break;
                    case 's':
                        dir = Direction.DOWN;
                        break;
                    case 'd':
                        dir = Direction.RIGHT;
                        break;
                    case 'x':
                        gameOver = true;
                        break;
                    case 'f':
                        if(!debug)
                        {
                            debug = true;
                        }
                        else
                        {
                            debug = false;
                        }
                        break;
                    default:
                        dir = Direction.STOP;
                        break;
                }

                if(gameOver)
                {
                    inputThread.Abort();
                }

                Thread.Sleep(10);
            }
        }

        private void Logic()
        {
            while(true)
            {
                int prevX = tailX[0];
                int prevY = tailY[0];
                int prev2X, prev2Y;
                tailX[0] = playerX;
                tailY[0] = playerY;


                for(int i = 1; i < tailLength; i++)
                {
                    prev2X = tailX[i];
                    prev2Y = tailY[i];
                    tailX[i] = prevX;
                    tailY[i] = prevY;
                    prevX = prev2X;
                    prevY = prev2Y;
                }

                switch(dir)
                {
                    case Direction.UP:
                        playerY--;
                        break;
                    case Direction.LEFT:
                        playerX--;
                        break;
                    case Direction.DOWN:
                        playerY++;
                        break;
                    case Direction.RIGHT:
                        playerX++;
                        break;
                    default:
                        break;
                }

                if (playerX > width-2 || playerX < 1 || playerY > height-1 || playerY < 0)
                {
                    gameOver = true;
                }

                for(int i = 0; i < tailLength; i++)
                {
                    if(tailX[i] == playerX  && tailY[i] == playerY)
                    {
                        gameOver = true;
                    }
                }

                if(playerX == fruitX && playerY == fruitY)
                {
                    score++;
                    fruitX = rand.Next(1, 29);
                    fruitY = rand.Next(0, 20);
                    tailLength++;
                }

                if (gameOver)
                {
                    logicThread.Abort();
                }

                Thread.Sleep(250);
            }
        }

        private void Draw()
        {
            while(true)
            {
                Console.Clear();

                for (int i = 0; i < width; i++)
                {
                    Console.Write("#");
                }
                Console.WriteLine();

                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        if (col == 0 || col == width - 1)
                        {
                            Console.Write("#");
                        }
                        else if (row == playerY && col == playerX)
                        {
                            Console.Write("O");
                        }
                        else if (row == fruitY && col == fruitX)
                        {
                            Console.Write("@");
                        }
                        else
                        {
                            bool print = false;
                            for (int i = 0; i < tailLength; i++)
                            {
                                if(tailX[i] == col && tailY[i] == row)
                                {
                                    Console.Write("o");
                                    print = true;
                                }
                            }
                            if (!print)
                            {
                                Console.Write(" ");
                            }
                        }
                    }
                    Console.WriteLine();
                }

                for (int i = 0; i < width; i++)
                {
                    Console.Write("#");
                }
                Console.WriteLine();

                Console.WriteLine($"Score: {score}");
                Console.WriteLine("Press 'x' to quit");
                Console.WriteLine("Press 'f' to debug");

                if (debug)
                {
                    Console.WriteLine($"Direction: {dir}");
                    Console.WriteLine($"Player: x={playerX}, y={playerY}");
                    Console.WriteLine($"Fruit: x={fruitX}, y={fruitY}");
                }

                if (gameOver)
                {
                    drawThread.Abort();
                }

                Thread.Sleep(100);
            }
        }

        private void GameOver()
        {

            Console.Clear();

            Console.WriteLine("*****GAME OVER*****");
            Console.WriteLine();
            Console.WriteLine($"Score: {score}");
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
