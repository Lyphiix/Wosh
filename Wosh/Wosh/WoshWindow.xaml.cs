using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly Canvas _canvas;

        // Properties for determining what the UI looks like
        public int MaxColumns;
        public int MaxRows;

        /// <summary>
        ///     The timer that refreshes the UI
        /// </summary>
        public DispatcherTimer UpdateTimer;

        public List<GroupedMetaData> GroupedMetaDatas;

        public WoshWindow()
        {
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;

            InitializeComponent();
            _canvas = new Canvas();

            // Timer
            UpdateTimer = new DispatcherTimer();
            UpdateTimer.Tick += OnTimedEvent;
            UpdateTimer.Interval = new TimeSpan(0, 0, 15);
            UpdateTimer.Start();

            // List of current GroupedMetaDatas
            GroupedMetaDatas = XmlParser.ParseStringForGroup(new WebClient().DownloadString(@"http://augo/go/cctray.xml"));

            CalculateMaximums();
        }

        private void CalculateMaximums()
        {
            MaxColumns = 2;
            MaxRows = GroupedMetaDatas.Count/MaxColumns;
        }

        private void OnTimedEvent(object source, EventArgs eventArgs)
        {
            DrawScreen(GroupedMetaDatas = XmlParser.ParseStringForGroup(new WebClient().DownloadString(@"http://augo/go/cctray.xml")));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawScreen(GroupedMetaDatas);
        }

        private void DrawScreen(List<GroupedMetaData> metaDatas)
        {
            var metaArray = metaDatas.ToArray();
            var counter = 0;
            for (var i = 0; i < MaxColumns; i++)
            {
                if (i == (MaxColumns - 1))
                {
                    MaxRows = metaDatas.Count - counter;
                }
                for (var j = 0; j < MaxRows; j++)
                {
                    DrawSegment(i, j, (GroupedMetaData) metaArray.GetValue(counter));
                    counter++;
                }
            }
            CalculateMaximums();
            Console.WriteLine("Finished Drawing");
        }

        private void DrawSegment(int column, int row, GroupedMetaData meta)
        {
            var rectangle = new Rectangle
                {
                    Name = "Rectangle" + column + "" + row,
                    Width = (ActualWidth - 16) / MaxColumns,
                    Height = (ActualHeight - 39) / MaxRows,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1,
                    Fill = new SolidColorBrush(ColorForGroupedMetaData(meta))
                };
            Canvas.SetLeft(rectangle, column*rectangle.Width);
            Canvas.SetTop(rectangle, row*rectangle.Height);
            _canvas.Children.Add(rectangle);

            var viewBox = new Viewbox
                {
                    MinWidth = (rectangle.Width / 100) * 80,
                    MaxWidth = (rectangle.Width / 100) * 80,
                    MinHeight = (rectangle.Height / 100) * 90,
                    MaxHeight = (rectangle.Height / 100) * 90
                };
            Canvas.SetLeft(viewBox, (column*rectangle.Width) + ((rectangle.Width/100)*10));
            Canvas.SetTop(viewBox, (row*rectangle.Height));

            var textBlock = new TextBlock
                {
                    Text = meta.Name,
                    Foreground = new SolidColorBrush(Colors.Black),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
            textBlock.Measure(new Size(rectangle.Width, rectangle.Height));
            textBlock.Arrange(new Rect(new Size(rectangle.Width, rectangle.Height)));

            viewBox.Child = textBlock;
            _canvas.Children.Add(viewBox);

            Content = _canvas;
        }

        private void DrawMultiSegment(int column, int row, MetaData meta)
        {
            // Placeholder
        }

        // Measures text size of textblock
        private static Size MeasureText(TextBlock tb)
        {
            var formattedText = new FormattedText(tb.Text, CultureInfo.CurrentUICulture,
                                                  FlowDirection.LeftToRight,
                                                  new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight,
                                                               tb.FontStretch),
                                                  tb.FontSize, Brushes.Black);
            return new Size(formattedText.Width, formattedText.Height);
        }

        public Color ColorForMetaData(MetaData meta)
        {
            if (meta.Activity.Equals("Building")) return Colors.Yellow;
            if (meta.LastBuildStatus.Equals("Success")) return Colors.LimeGreen;
            if (meta.LastBuildStatus.Equals("Failure")) return Colors.Red;
            return Colors.White;
        }

        public Color ColorForGroupedMetaData(GroupedMetaData groupMeta)
        {
            foreach (var meta in groupMeta.SubData)
            {
                if (meta.LastBuildStatus.Equals("Failure")) return Colors.Red;
                if (meta.Activity.Equals("Building")) return Colors.Yellow;
            }
            return Colors.LimeGreen;
        }
    }
}