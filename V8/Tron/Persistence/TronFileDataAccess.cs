using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Tron.Persistence
{
    public class TronFileDataAccess : ITronDataAccess
    {
        public async Task<TronTable> LoadAsync(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string line = await reader.ReadLineAsync() ?? string.Empty;
                    string[] datas = line.Split(' ');
                    int gridsize = int.Parse(datas[0]);
                    int bluex = int.Parse(datas[1]);
                    int bluey = int.Parse(datas[2]);
                    Direction bluedirection = new Direction();
                    switch (datas[3])
                    {
                        case "Left":
                            bluedirection = Direction.Left;
                            break;
                        case "Right":
                            bluedirection = Direction.Right;
                            break;
                        case "Up":
                            bluedirection = Direction.Up;
                            break;
                        case "Down":
                            bluedirection = Direction.Down;
                            break;
                        default: throw new Exception("Wrong data!");
                    }

                    TronPlayer blue = new TronPlayer(bluex, bluey, bluedirection);

                    int redx = int.Parse(datas[4]);
                    int redy = int.Parse(datas[5]);
                    Direction reddirection = new Direction();
                    switch (datas[6])
                    {
                        case "Left":
                            bluedirection = Direction.Left;
                            break;
                        case "Right":
                            bluedirection = Direction.Right;
                            break;
                        case "Up":
                            bluedirection = Direction.Up;
                            break;
                        case "Down":
                            bluedirection = Direction.Down;
                            break;
                        default: throw new Exception("Wrong data!");
                    }
                    TronPlayer red = new TronPlayer(redx, redy, reddirection);
                    
                    
                    int[,] grid = new int[gridsize, gridsize];

                    for (int i = 0; i < gridsize; i++)
                    {
                        line = await reader.ReadLineAsync() ?? String.Empty;
                        datas = line.Split(' ');
                        for (int j = 0; j < gridsize; j++)
                        {
                            grid[i,j] = int.Parse(datas[j]);
                        }
                    }
                    TronTable _table = new TronTable(gridsize, grid, blue, red);
                    return _table;
                }
            }
            catch
            {
                throw new Exception();
            }
        }


        public async Task SaveAsync(string path, TronTable table)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    await writer.WriteLineAsync(table.GridSize + " " + table.Blue.X + " " + table.Blue.Y + " " + table.Blue.Direction + " " + table.Red.X + " " + table.Red.Y + " " + table.Red.Direction);
                    for (int i = 0; i < table.GridSize; i++)
                    {
                        for (int j = 0; j < table.GridSize; j++)
                        {
                            await writer.WriteAsync(table.Grid[i, j] + " ");
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new Exception("Something went wrong in saving");
            }
        }


    }
}
