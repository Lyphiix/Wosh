using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private readonly Canvas _canvas;

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
        /// Retains parsed Pipelines for drawing when the window is resized
        /// </summary>
        public List<Pipeline> Pipelines;

        /// <summary>
        /// Retains old parsed Projects
        /// </summary>
        public List<Project> OldProjects;

        /// <summary>
        /// Retains the current parsed Projects
        /// </summary>
        public List<Project> Projects;

        /// <summary>
        /// Instance of the SoundHandler class
        /// </summary>
        public SoundHandler SoundHandler;

        /// <summary>
        /// The instance of the Configuration (Preferences) window
        /// </summary>
        public WoshConfigurationWindow ConfigurationWindow;

        /// <summary>
        /// Error
        /// </summary>
        public enum Error
        {
            ErrorWebException,
            ErrorInvalidUrl
        }

        public WoshWindow()
        {
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;

            InitializeComponent();

            // Timer set up
            UpdateTimer = new DispatcherTimer();
            UpdateTimer.Tick += OnTimedEvent;
            UpdateTimer.Interval = new TimeSpan(0, 0, Config.Default.PollSpeed); // Sets the timer's interval to 15 seconds
            // TODO - Make the interval get retrieved from config
            UpdateTimer.Start();

            XmlParser = new XmlParser
                {
                    ShouldShowBrokenProjects = true
                };

            SoundHandler = new SoundHandler();

            // Parse for the lists
            using (var webClient = new WebClient())
            {
                try
                {
                    OldProjects = Projects = XmlParser.ParseString(webClient.DownloadString(Config.Default.URLToParse));
                    Pipelines = XmlParser.ParseToPipeline(Projects);
                }
                catch (WebException)
                {
                    DrawErrorScreen(Error.ErrorWebException);
                }
                catch (Exception)
                {
                    OldProjects = Projects = new List<Project>();
                    Pipelines = new List<Pipeline>();
                }
            }

            _canvas = new Canvas {Background = new SolidColorBrush(Colors.Black)};

            CalculateMaximums();
        }

        public void ForceRedraw()
        {
            TimerScreenDraw();
        }

        // Calculates the maximums for Columns and Rows - Columns is retrieved from config and Rows is calculated
        private void CalculateMaximums()
        {
            Columns = Config.Default.NumOfColumns;
            Rows = (int)Math.Ceiling((double)Pipelines.Count/Columns);
        }

        // Called by timer - Redraws the screen
        private void OnTimedEvent(object source, EventArgs eventArgs)
        {
            TimerScreenDraw();
        }

        private void TimerScreenDraw()
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    OldProjects = Projects;
                    Projects = XmlParser.ParseString(webClient.DownloadString(Config.Default.URLToParse));
                    DrawScreen(Pipelines = XmlParser.ParseToPipeline(Projects));
                    SoundHandler.PlaySound(OldProjects, Projects);
                }
                catch (WebException)
                {
                    DrawErrorScreen(Error.ErrorWebException);
                }
                catch (System.Xml.XmlException)
                {
                    DrawErrorScreen(Error.ErrorInvalidUrl);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error");
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawScreen(Pipelines);
        }

        // Draws the display on the window
        private void DrawScreen(List<Pipeline> pipelines)
        {
            _canvas.Children.Clear();
            var pipelineArray = pipelines.ToArray();
            var counter = 0;
            for (var i = 0; i < Columns; i++)
            {
                for (var j = 0; j < Rows; j++)
                {
                    if (pipelineArray.Count() != 0)
                    {
                        try
                        {
                            DrawPipelineSegment(i, j, (Pipeline)pipelineArray.GetValue(counter));
                            counter++;
                        }
                        catch (Exception)
                        {
                            counter--;
                        }
                    }
                }
            }
            Content = _canvas; // Add this segment to the screens content
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void DrawErrorScreen(Error error)
        {
            _canvas.Children.Clear();
            switch (error)
            {
                case Error.ErrorWebException:
                    break;
                case Error.ErrorInvalidUrl:
                    break;
            }
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

            var textBlock = new TextBlock // Contains the name of the Pipeline to display
                {
                    Text = pipeline.Name,
                    Foreground = new SolidColorBrush(Colors.Black)
                };

            var viewBox = new Viewbox // Scales the containing elements
            {
                MaxWidth = (rectangle.Width / 100) * 80,
                MinHeight = (rectangle.Height/100)*90,
                MaxHeight = (rectangle.Height / 100) * 90
            };
            Canvas.SetLeft(viewBox, (column * rectangle.Width) + ((rectangle.Width / 100) * 10));
            Canvas.SetTop(viewBox, (row * rectangle.Height));
            
            if (pipeline.IsBrokenProject)
            {
                textBlock.TextAlignment = TextAlignment.Left;
                viewBox.MinWidth = (rectangle.Width / 100) * 75;
                viewBox.MaxWidth = (rectangle.Width / 100) * 75;
                Canvas.SetLeft(viewBox, (column * rectangle.Width) + ((rectangle.Width / 100) * 25));
            }
            else
            {
                viewBox.MinWidth = (rectangle.Height / 100) * 80;
            }

            viewBox.Child = textBlock;
            _canvas.Children.Add(viewBox);
        }

        // Returns the color of the given Project
        public Color ColorForProject(Project project)
        {
            if (project.Activity.Equals("Building")) return Colors.Yellow;
            if (project.LastBuildStatus.Equals("Success")) return Colors.LimeGreen;
            return (project.LastBuildStatus.Equals("Failure")) ? Colors.Red : Colors.White;
        }

        // Returns the color of the given Pipeline
        public Color ColorForPipeline(Pipeline pipeline)
        {
            var colorReturn = Colors.LimeGreen;
            foreach (var project in pipeline.SubData)
            {
                if (project.LastBuildStatus.Equals("Failure")) return Colors.Red;
                if (project.Activity.Equals("Building")) colorReturn =  Colors.Yellow;
            }
            return colorReturn;
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
           if ((e.Key == Key.F2))
           {
               try
               {
                   ConfigurationWindow.Show();
                   ConfigurationWindow.Activate();
               }
               catch (Exception)
               {
                    ConfigurationWindow = new WoshConfigurationWindow(this);
                    ConfigurationWindow.Show();
                    ConfigurationWindow.Activate();
               }
           }
        }

        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name) ? Application.Current.Windows.OfType<T>().Any() : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            UpdateTimer.Stop();
        }
    }
}