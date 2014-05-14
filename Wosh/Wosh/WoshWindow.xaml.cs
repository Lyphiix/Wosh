using System;
using System.Collections.Generic;
using System.Globalization;
using System.Media;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Wosh.logic;

namespace Wosh
{
    public partial class WoshWindow : Window
    {
        /// <summary>
        /// Drawing canvas
        /// </summary>
        private Canvas _canvas;

        /// <summary>
        /// The number of columns to draw - Set in configuration
        /// </summary>
        public int Columns;
        /// <summary>
        /// The number of rows to draw - Calculated using Columns and the number of MetaDatas
        /// </summary>
        public int Rows;

        /// <summary>
        /// The timer that refreshes the UI
        /// </summary>
        public DispatcherTimer UpdateTimer;

        /// <summary>
        /// The instance of the XmlParser
        /// </summary>
        public XmlParser XmlParser;

        /// <summary>
        /// Retains parsed MetaDatas for drawing when the window is resized
        /// </summary>
        public List<Pipeline> GroupedMetaDatas;

        public WoshWindow()
        {
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;

            InitializeComponent();

            // Timer set up
            UpdateTimer = new DispatcherTimer();
            UpdateTimer.Tick += OnTimedEvent;
            UpdateTimer.Interval = new TimeSpan(0, 0, 2); // Sets the timer's interval to 15 seconds
            // TODO - Make the interval get retrieved from config
            UpdateTimer.Start();

            XmlParser = new XmlParser();

            // List of current GroupedMetaDatas
            using (var webClient = new WebClient())
            {
                GroupedMetaDatas = XmlParser.ParseStringForGroup(webClient.DownloadString(@"http://augo/go/cctray.xml"));
            }

            _canvas = new Canvas();
        }

        // Calculates the maximums for Columns and Rows - Columns is retrieved from config and Rows is calculated
        private void CalculateMaximums()
        {
            Columns = 3; // TODO - Retrieve from config
            Rows = GroupedMetaDatas.Count/Columns;
        }

        // Called by timer - Redraws the screen
        private void OnTimedEvent(object source, EventArgs eventArgs)
        {
            _canvas.Children.Clear();
            using (var webClient = new WebClient())
            {
                DrawScreen(GroupedMetaDatas = XmlParser.ParseStringForGroup(webClient.DownloadString(@"http://augo/go/cctray.xml")));
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawScreen(GroupedMetaDatas);
        }

        // Draws the display on the window
        private void DrawScreen(List<Pipeline> metaDatas)
        {
            CalculateMaximums();
            var metaArray = metaDatas.ToArray();
            var counter = 0;
            for (var i = 0; i < Columns; i++)
            {
                if ((Columns != 1) && (i == (Columns - 1)))
                {
                    Rows = metaDatas.Count - counter;
                }
                for (var j = 0; j < Rows; j++)
                {
                    try
                    {
                        DrawPipelineSegment(i, j, (Pipeline)metaArray.GetValue(counter));
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("There are more rows than needed");
                    }
                    counter++;
                }
            }
            Content = _canvas; // Add this segment to the screens content
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        // Draws a single segment
        private void DrawPipelineSegment(int column, int row, Pipeline pipeline)
        {
            var rectangle = new Rectangle // The background rectangle
                {
                    Name = "Rectangle" + column + "" + row,
                    Width = (ActualWidth - 16) / Columns,
                    Height = (ActualHeight - 39) / Rows,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1,
                    Fill = new SolidColorBrush(ColorForPipeline(pipeline))
                };
            Canvas.SetLeft(rectangle, column*rectangle.Width);
            Canvas.SetTop(rectangle, row*rectangle.Height);
            _canvas.Children.Add(rectangle);

            var viewBox = new Viewbox // Scales the containing elements
                {
                    MinWidth = (rectangle.Width / 100) * 80,
                    MaxWidth = (rectangle.Width / 100) * 80,
                    MinHeight = (rectangle.Height / 100) * 90,
                    MaxHeight = (rectangle.Height / 100) * 90
                };
            Canvas.SetLeft(viewBox, (column*rectangle.Width) + ((rectangle.Width/100)*10));
            Canvas.SetTop(viewBox, (row*rectangle.Height));

            var textBlock = new TextBlock // Contains the name of the Pipeline to display
                {
                    Text = pipeline.Name,
                    Foreground = new SolidColorBrush(Colors.Black),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
            textBlock.Measure(new Size(rectangle.Width, rectangle.Height));
            textBlock.Arrange(new Rect(new Size(rectangle.Width, rectangle.Height)));

            viewBox.Child = textBlock;
            _canvas.Children.Add(viewBox);
        }

        // Returns the color of 
        public Color ColorForProject(Project project)
        {
            if (project.Activity.Equals("Building")) return Colors.Yellow;
            if (project.LastBuildStatus.Equals("Success")) return Colors.LimeGreen;
            return (project.LastBuildStatus.Equals("Failure")) ? Colors.Red : Colors.White;
        }

        public Color ColorForPipeline(Pipeline pipeline)
        {
            foreach (var project in pipeline.SubData)
            {
                if (project.LastBuildStatus.Equals("Failure")) return Colors.Red;
                if (project.Activity.Equals("Building")) return Colors.Yellow;
            }
            return Colors.LimeGreen;
        }
    }
}