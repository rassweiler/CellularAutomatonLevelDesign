using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace LevelDeign
{
    public class Level
    {
        public Random rand = new Random();
        public int[,] map;
        public int width { get; set; }
        public int height { get; set; }
        public float chance { get; set; }

        public Level(int w, int h, float c)
        {
            width = w;
            height = h;
            chance = c;
            InitMap();
        }

        public void InitMap()
        {
            map = new int[width, height];
            for (int column = 0, row = 0; row < height; ++row)
            {
                for (column = 0; column < width; ++column)
                {
                    if (column == 0)
                    {
                        map[column, row] = 2;
                    }
                    else if (row == 0)
                    {
                        map[column, row] = 2;
                    }
                    else if (column == width - 1)
                    {
                        map[column, row] = 2;
                    }
                    else if (row == height - 1)
                    {
                        map[column, row] = 2;
                    }
                    else
                    {
                        if (rand.NextDouble() < chance)
                        {
                            map[column, row] = 2;
                        }
                        else
                        {
                            map[column, row] = 0;
                        }

                    }
                }
            }
        }

        public string SerialiseMap()
        {
            string mapString = "";
            for (int column = 0, row = 0; row < height; ++row)
            {
                for (column = 0; column < width; ++column)
                {
                    if(map[column, row] == 1)
                    {
                        mapString += "#";
                    }
                    else
                    {
                        mapString += ".";
                    }
                }
                mapString += "\n";
            }
                    return mapString;
        }

        public void ClearMap()
        {
            for (int column = 0, row = 0; row < height; ++row)
            {
                for (column = 0; column < width; ++column)
                {
                    map[column, row] = 0;
                }
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        static public Level level;
        static public int birthLimit = 4;
        static public int deathLimit = 3;
        static public int steps = 1;
        static public float chance = 0.45F;
        static int tileSize = 4;

        public MainWindow()
        {
            InitializeComponent();
            NewLevel();
        }

        private void NewLevel()
        {
            level = new Level(180, 180, chance);
            for(int i = 0; i < steps; ++i)
            {
                StepSimulation();
            }
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            levelDisplay.Children.Clear();
            for(int y = 0; y < level.height; ++y)
            {
                for(int x = 0; x < level.width; ++x)
                {
                    if(level.map[x,y] == 1)
                    {
                        Rectangle r = new Rectangle();
                        r.Width = tileSize;
                        r.Height = tileSize;
                        r.Fill = new SolidColorBrush(Colors.Green);
                        levelDisplay.Children.Add(r);
                        Canvas.SetTop(r, y*tileSize);
                        Canvas.SetLeft(r, x*tileSize);
                    }
                    else if (level.map[x, y] == 2)
                    {
                        Rectangle r = new Rectangle();
                        r.Width = tileSize;
                        r.Height = tileSize;
                        r.Fill = new SolidColorBrush(Colors.Gray);
                        levelDisplay.Children.Add(r);
                        Canvas.SetTop(r, y * tileSize);
                        Canvas.SetLeft(r, x * tileSize);
                    }
                    else if (level.map[x, y] == 3)
                    {
                        Rectangle r = new Rectangle();
                        r.Width = tileSize;
                        r.Height = tileSize;
                        r.Fill = new SolidColorBrush(Colors.Black);
                        levelDisplay.Children.Add(r);
                        Canvas.SetTop(r, y * tileSize);
                        Canvas.SetLeft(r, x * tileSize);
                    }
                }
            }
        }

        private void newLevel_OnClick(object sender, RoutedEventArgs e)
        {
            NewLevel();
        }

        private void clearMap_OnClick(object sender, RoutedEventArgs e)
        {
            level.ClearMap();
            levelDisplay.Children.Clear();
        }

        private void stepSimulation_OnClick(object sender, RoutedEventArgs e)
        {
            StepSimulation();
            UpdateDisplay();
        }

        private void StepSimulation()
        {
            int[,] tempMap = new int[level.width, level.height];
            for (int x = 0, y = 0; y < level.height; ++y)
            {
                for (x = 0; x < level.width; ++x)
                {
                    tempMap[x, y] = 0;
                }
            }

            Thread thread1 = new Thread(() => StepSimulationThread(0,level.height/2,tempMap));
            Thread thread2 = new Thread(() => StepSimulationThread(level.height / 2, level.height, tempMap));
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            //for (int column = 0, row = 0; row <= level.height - 1; row++)
            //{
            //    for (column = 0; column <= level.width - 1; column++)
            //    {
            //        int numWalls = GetAdjacentWalls(column, row);

            //        if (level.map[column, row] > 0)
            //        {
            //            if (numWalls < deathLimit)
            //            {
            //                tempMap[column, row] = 0;
            //            }
            //            else
            //            {
            //                tempMap[column, row] = 2;
            //            }
            //        }
            //        else
            //        {
            //            if (numWalls > birthLimit)
            //            {
            //                tempMap[column, row] = 2;
            //            }
            //            else
            //            {
            //                tempMap[column, row] = 0;
            //            }
            //        }
            //    }
            //}

            //Check for hidden and grass
            for (int column = 0, row = 0; row <= level.height - 1; row++)
            {
                for (column = 0; column <= level.width - 1; column++)
                {
                    int numWalls = GetAdjacentWalls(column, row, tempMap);

                    if (numWalls == 8)
                    {
                        tempMap[column, row] = 3;
                    }
                    else
                    {
                        if (GetGrass(column, row, tempMap))
                        {
                            tempMap[column, row] = 1;
                        }
                    }
                }
            }
            level.map = tempMap;
        }

        public void StepSimulationThread(int startRow, int endRow, int[,] tMap)
        {
            for (int column = 0, row = startRow; row < endRow; row++)
            {
                for (column = 0; column < level.width; column++)
                {
                    int numWalls = GetAdjacentWalls(column, row);

                    if (level.map[column, row] > 0)
                    {
                        if (numWalls < deathLimit)
                        {
                            tMap[column, row] = 0;
                        }
                        else
                        {
                            tMap[column, row] = 2;
                        }
                    }
                    else
                    {
                        if (numWalls > birthLimit)
                        {
                            tMap[column, row] = 2;
                        }
                        else
                        {
                            tMap[column, row] = 0;
                        }
                    }
                }
            }
        }

        private int GetAdjacentWalls(int x, int y, int[,] tmap = null)
        {
            int count = 0;
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    if (tmap == null)
                    {
                        int neighbour_x = x + i;
                        int neighbour_y = y + j;
                        if (i == 0 && j == 0)
                        {

                        }
                        
                        else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= level.width || neighbour_y >= level.height)
                        {
                            count = count + 1;
                        }
                        
                        else if (level.map[neighbour_x, neighbour_y] > 0)
                        {
                            count = count + 1;
                        }
                    }
                    else
                    {
                        int neighbour_x = x + i;
                        int neighbour_y = y + j;
                        if (i == 0 && j == 0)
                        {

                        }
                        
                        else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= level.width || neighbour_y >= level.height)
                        {
                            count = count + 1;
                        }
                        
                        else if (tmap[neighbour_x, neighbour_y] > 0)
                        {
                            count = count + 1;
                        }
                    }
                }
            }
            return count;
        }

        private bool GetGrass(int x, int y, int[,] map)
        {
            int neighbour_top = y - 1;
            int neighbour_bottom = y + 1;

            if (neighbour_top < 0 || neighbour_bottom < 0 || neighbour_top >= level.width || neighbour_bottom >= level.height)
            {
                return false;
            }
            
            else if (map[x, neighbour_top] == 0 && map[x, neighbour_bottom] > 1)
            {
                return true;
            }
            else
            {
                return false; 
            }
        }

        private void birthLimitText_OnPreview(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextInt(e.Text);
        }

        private static bool IsTextInt(string text)
        {
            return Regex.IsMatch(text, "^[0-9]$");
        }

        private void stepNumber_OnChange(object sender, TextChangedEventArgs e)
        {
            if (stepNumber.Text != "")
            {
                steps = Int32.Parse(stepNumber.Text);
            }
        }

        private void startChance_OnChange(object sender, TextChangedEventArgs e)
        {
            if (startChance.Text != "")
            {
                chance = float.Parse(startChance.Text) / 100;
            }
        }

        private void deathLimit_OnChange(object sender, TextChangedEventArgs e)
        {
            if (deathLimitText.Text != "")
            {
                deathLimit = Int32.Parse(deathLimitText.Text);
            }
        }

        private void birthLimit_OnChange(object sender, TextChangedEventArgs e)
        {
            if (birthLimitText.Text != "")
            {
                birthLimit = Int32.Parse(birthLimitText.Text);
            }
        }
    }
}
