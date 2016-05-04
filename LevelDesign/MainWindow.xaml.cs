using System;
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
                        map[column, row] = 1;
                    }
                    else if (row == 0)
                    {
                        map[column, row] = 1;
                    }
                    else if (column == width - 1)
                    {
                        map[column, row] = 1;
                    }
                    else if (row == height - 1)
                    {
                        map[column, row] = 1;
                    }
                    else
                    {
                        if (rand.NextDouble() < chance)
                        {
                            map[column, row] = 1;
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
                    map[column, row] = 1;
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

        public MainWindow()
        {
            InitializeComponent();
            level = new Level(70, 70, 0.35F);
            Point p = new Point(138, 20);
            Size s = new Size(300, 300);
            Size ts = new Size(20, 20);
            Rect tile = new Rect(p, s);
            Brush t = CreateGridBrush(tile, ts);
            Rectangle canvas = new Rectangle();
            canvas.Width = 300;
            canvas.Height = 300;
            canvas.Fill = t;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            levelDisplay.Text = level.SerialiseMap();
        }

        private void newLevel_OnClick(object sender, RoutedEventArgs e)
        {
            level = new Level(70, 70, 0.35F);
            UpdateDisplay();
        }

        private void clearMap_OnClick(object sender, RoutedEventArgs e)
        {
            level.ClearMap();
            UpdateDisplay();
        }

        private void stepSimulation_OnClick(object sender, RoutedEventArgs e)
        {
             int[,] tempMap = new int[level.width, level.height];
            for (int x = 0, y = 0; y < level.height; ++y)
            {
                for (x = 0; x < level.width; ++x)
                {
                    tempMap[x, y] = 1;
                }
            }
            for (int column = 0, row = 0; row <= level.height - 1; row++)
            {
                for (column = 0; column <= level.width - 1; column++)
                {
                    int numWalls = GetAdjacentWalls(column, row);

                    if (level.map[column, row] == 1)
                    {
                        if(numWalls < deathLimit)
                        {
                            tempMap[column, row] = 0;
                        }
                        else
                        {
                            tempMap[column, row] = 1;
                        }
                    }
                    else
                    {
                        if (numWalls > birthLimit)
                        {
                            tempMap[column, row] = 1;
                        }
                        else
                        {
                            tempMap[column, row] = 0;
                        }
                    }
                }
            }
            level.map = tempMap;
            UpdateDisplay();
        }

        private int GetAdjacentWalls(int x, int y)
        {
            int count = 0;
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    int neighbour_x = x + i;
                    int neighbour_y = y + j;
                    if (i == 0 && j == 0)
                    {
                        
                    }
                    //In case the index we're looking at it off the edge of the map
                    else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= level.width || neighbour_y >= level.height)
                    {
                        count = count + 1;
                    }
                    //Otherwise, a normal check of the neighbour
                    else if (level.map[neighbour_x,neighbour_y] == 1)
                    {
                        count = count + 1;
                    }
                }
            }
            return count;
        }

        private void birthLimitText_OnPreview(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextInt(e.Text);
        }

        private static bool IsTextInt(string text)
        {
            return Regex.IsMatch(text, "^[0-9]$");
        }

        static Brush CreateGridBrush(Rect bounds, Size tileSize)
        {
            var gridColor = Brushes.Black;
            var gridThickness = 1.0;
            var tileRect = new Rect(tileSize);

            var gridTile = new DrawingBrush
            {
                Stretch = Stretch.None,
                TileMode = TileMode.Tile,
                Viewport = tileRect,
                ViewportUnits = BrushMappingMode.Absolute,
                Drawing = new GeometryDrawing
                {
                    Pen = new Pen(gridColor, gridThickness),
                    Geometry = new GeometryGroup
                    {
                        Children = new GeometryCollection
                {
                    new LineGeometry(tileRect.TopLeft, tileRect.BottomRight),
                    new LineGeometry(tileRect.BottomLeft, tileRect.TopRight)
                }
                    }
                }
            };

            var offsetGrid = new DrawingBrush
            {
                Stretch = Stretch.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                Transform = new TranslateTransform(bounds.Left, bounds.Top),
                Drawing = new GeometryDrawing
                {
                    Geometry = new RectangleGeometry(new Rect(bounds.Size)),
                    Brush = gridTile
                }
            };

            return offsetGrid;
        }
    }
}
