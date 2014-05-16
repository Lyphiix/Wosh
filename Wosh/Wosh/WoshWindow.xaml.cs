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
        /// Determines wether or not the application plays sounds
        /// </summary>
        public bool ShouldPlaySounds;

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
                    ShouldShowBrokenProjects = Config.Default.ShouldShowBrokenStages,
                    ShouldRemoveAfterExpirary = Config.Default.ShouldAutoExcludeOldProjects,
                    DaysToExpiry = Config.Default.ExcludeProjectsAfterDays
                };

            SoundHandler = new SoundHandler();

            ShouldPlaySounds = Config.Default.ShouldPlaySounds;

            _canvas = new Canvas { Background = new SolidColorBrush(Colors.Black) };

            // Parse for the lists
            try
            {
                using (var webClient = new WebClient())
                {
                    OldProjects = Projects = XmlParser.ParseString(webClient.DownloadString(Config.Default.URLToParse));
                    Pipelines = XmlParser.ParseToPipeline(Projects);
                }
            }
            catch (Exception)
            {
                OldProjects = Projects = new List<Project>();
                Pipelines = new List<Pipeline>();
            }

            CalculateMaximums();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TimerScreenDraw();
        }

        public void ForceRedraw()
        {
            TimerScreenDraw();
        }

        // Calculates the maximums for Columns and Rows - Columns is retrieved from config and Rows is calculated
        private void CalculateMaximums()
        {
            Columns = Config.Default.NumOfColumns;
            Rows = (int)Math.Ceiling((double)Pipelines.Count / Columns);
        }

        // Called by timer - Redraws the screen
        private void OnTimedEvent(object source, EventArgs eventArgs)
        {
            TimerScreenDraw();
        }

        private void TimerScreenDraw()
        {
            Console.WriteLine("Timer went off");
            try
            {
                using (var webClient = new WebClient())
                {
                    OldProjects = Projects;
                    var x = webClient.DownloadString(Config.Default.URLToParse);
                    Projects = XmlParser.ParseString(x);
                    Pipelines = XmlParser.ParseToPipeline(Projects);
                    DrawScreen(Pipelines);
                    if (ShouldPlaySounds) SoundHandler.PlaySound(OldProjects, Projects);
                }
            }
            catch (WebException)
            {
                DrawErrorScreen(Error.ErrorWebException);
            }
            catch (System.Xml.XmlException)
            {
                DrawErrorScreen(Error.ErrorInvalidUrl);
            }
            catch (ArgumentException)
            {
                DrawErrorScreen(Error.ErrorInvalidUrl);
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
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
            if (Pipelines.Any())
            {
                CalculateMaximums();
                var pipelineArray = pipelines.ToArray();
                var counter = 0;
                for (var i = 0; i < Columns; i++)
                {
                    for (var j = 0; j < Rows; j++)
                    {
                        try
                        {
                            DrawPipelineSegment(i, j, (Pipeline)pipelineArray.GetValue(counter));
                            counter++;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Do nothing");
                        }
                    }
                }
            }
            else
            {
                var viewBox = new Viewbox
                {
                    MaxWidth = (ActualWidth / 100) * 80,
                    MaxHeight = ActualHeight / 2
                };
                Canvas.SetLeft(viewBox, (ActualWidth / 100) * 10);
                Canvas.SetTop(viewBox, (ActualHeight / 3));

                var textBlock = new TextBlock // Contains the name of the Pipeline to display
                {
                    Text = "There is nothing to parse at the given URL\nDouble check URL setting",
                    Foreground = new SolidColorBrush(Colors.White),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };

                viewBox.Child = textBlock;
                _canvas.Children.Add(viewBox);
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
                    var errorViewBox = new Viewbox
                        {
                            MaxWidth = (ActualWidth / 100) * 80,
                            MaxHeight = ActualHeight / 2
                        };
                    Canvas.SetLeft(errorViewBox, (ActualWidth / 100) * 10);

                    var errorTextBlock = new TextBlock // Contains the name of the Pipeline to display
                    {
                        Text = "Network Error",
                        Foreground = new SolidColorBrush(Colors.White),
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };

                    var timerViewBox = new Viewbox
                        {
                            MaxWidth = (ActualWidth / 100) * 50,
                            MaxHeight = ActualHeight / 2
                        };
                    Canvas.SetTop(timerViewBox, (ActualHeight / 2) + 10);
                    Canvas.SetLeft(timerViewBox, (ActualWidth / 100) * 25);

                    var timerTextBlock = new TextBlock
                        {
                            Foreground = new SolidColorBrush(Colors.White),
                            HorizontalAlignment = HorizontalAlignment.Center
                        };

                    var time = TimeSpan.FromSeconds(Config.Default.PollSpeed - 1);
                    var timer = new DispatcherTimer();
                    timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                        {
                            timerTextBlock.Text = time.ToString("c");
                            if (time == TimeSpan.Zero) timer.Stop();
                            time = time.Add(TimeSpan.FromSeconds(-1));
                        }, Application.Current.Dispatcher);

                    timer.Start();

                    timerViewBox.Child = timerTextBlock;
                    errorViewBox.Child = errorTextBlock;
                    _canvas.Children.Add(timerViewBox);
                    _canvas.Children.Add(errorViewBox);
                    break;
                case Error.ErrorInvalidUrl:
                    var viewBox = new Viewbox
                        {
                            MaxWidth = (ActualWidth / 100) * 80,
                            MaxHeight = ActualHeight / 2
                        };
                    Canvas.SetLeft(viewBox, (ActualWidth / 100) * 10);

                    var textBlock = new TextBlock // Contains the name of the Pipeline to display
                    {
                        Text = "Network Error",
                        Foreground = new SolidColorBrush(Colors.White),
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };

                    var box = new Viewbox
                        {
                            MaxWidth = (ActualWidth / 100) * 50,
                            MaxHeight = ActualHeight / 2
                        };
                    Canvas.SetTop(box, (ActualHeight / 2) + 10);
                    Canvas.SetLeft(box, (ActualWidth / 100) * 25);

                    var block = new TextBlock
                        {
                            Foreground = new SolidColorBrush(Colors.White),
                            HorizontalAlignment = HorizontalAlignment.Center
                        };

                    var span = TimeSpan.FromSeconds(Config.Default.PollSpeed - 1);
                    var dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                        {
                            block.Text = span.ToString("c");
                            if (span == TimeSpan.Zero) dispatcherTimer.Stop();
                            span = span.Add(TimeSpan.FromSeconds(-1));
                        }, Application.Current.Dispatcher);

                    dispatcherTimer.Start();

                    box.Child = block;
                    viewBox.Child = textBlock;
                    _canvas.Children.Add(box);
                    _canvas.Children.Add(viewBox);
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
            Canvas.SetLeft(rectangle, column * rectangle.Width);
            Canvas.SetTop(rectangle, row * rectangle.Height);
            _canvas.Children.Add(rectangle);

            var textBlock = new TextBlock // Contains the name of the Pipeline to display
                {
                    Text = pipeline.Name,
                    Foreground = new SolidColorBrush(Colors.Black)
                };

            var viewBox = new Viewbox // Scales the containing elements
            {
                MaxWidth = (rectangle.Width / 100) * 80,
                MinHeight = (rectangle.Height / 100) * 90,
                MaxHeight = (rectangle.Height / 100) * 90
            };
            Canvas.SetLeft(viewBox, (column * rectangle.Width) + ((rectangle.Width / 100) * 10));
            Canvas.SetTop(viewBox, (row * rectangle.Height));

            if (pipeline.IsBrokenProject)
            {
                textBlock.TextAlignment = TextAlignment.Left;
                textBlock.Text = "    " + textBlock.Text;
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
                if (project.Activity.Equals("Building")) colorReturn = Colors.Yellow;
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