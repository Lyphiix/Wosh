using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Wosh.logic;

namespace Wosh
{
    public partial class WoshWindow : Window
    {
        private readonly Canvas _canvas;
        
        public int MaxColumns;
        public int MaxRows;

        //public IXmlParser TheParser;

        public WoshWindow()
        {
            InitializeComponent();
            _canvas = new Canvas();
            MaxColumns = 2;
            MaxRows = 10;
            // Content Manger.
            // mager(this);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawScreen();
        }

        //private void DrawScreen(List<MetaData> metaDatas)
        private void DrawScreen()
        {
            var counter = 0;
            for (var i = 0; i < MaxColumns; i++)
            {
                for (var j = 0; j < MaxRows; j++)
                {
                    var meta = new MetaData
                    {
                        Name = "Project",
                        Activity = "Sleeping",
                        LastBuildLabel = "(null)",
                        LastBuildStatus = "Success",
                        LastBuildTime = "(null)",
                        WebUrl = "http://www.this-is-stupid.com"
                    };
                    if (j == 1)
                    {
                        meta.Activity = "Building";
                    } else if (j == 5)
                    {
                        meta.LastBuildStatus = "Failure";
                    }
                    DrawSegment(i, j, meta);
                }
            }
        }

        private void DrawSegment(int column, int row, MetaData meta)
        {
            var rectangle = new Rectangle
                {
                    Width = (ActualWidth - 16)/MaxColumns,
                    Height = (ActualHeight - 39)/MaxRows,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1,
                    Fill = new SolidColorBrush(ColorForMetaData(meta))
                };
            Canvas.SetLeft(rectangle, column * rectangle.Width);
            Canvas.SetTop(rectangle, row*rectangle.Height);
            _canvas.Children.Add(rectangle);

            var textBlock = new TextBlock
                {
                    Text = meta.Name,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = rectangle.Height/2,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
            textBlock.Measure(new Size(rectangle.Width, rectangle.Height));
            textBlock.Arrange(new Rect(new Size(rectangle.Width, rectangle.Height)));
            Canvas.SetLeft(textBlock, (column * rectangle.Width) + ((rectangle.Width - textBlock.ActualWidth) / 2));
            Canvas.SetTop(textBlock, (row * rectangle.Height) + ((rectangle.Height - textBlock.ActualHeight) / 2));
            _canvas.Children.Add(textBlock);

            Content = _canvas;
        }

        private void DrawMultiSegment(int column, int row, MetaData meta)
        {
            // Placeholder
        }

        private static Color ColorForMetaData(MetaData meta)
        {
            if (!meta.Activity.Equals("Sleeping"))
            {
                return Colors.Yellow;
            }
            else
            {
                return (meta.LastBuildStatus.Equals("Failure")) ? Colors.Red : Colors.LimeGreen;
            }
        }
    }
}