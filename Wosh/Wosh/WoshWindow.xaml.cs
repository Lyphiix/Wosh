using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Wosh
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WoshWindow : Window
    {
        private Canvas Canvas;
        

        public int MaxColumns;
        public int MaxRows;

        public WoshWindow()
        {
            Canvas = new Canvas();
            MaxColumns = 2;
            MaxRows = 10;
        }

        private void DrawScreen(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < MaxColumns; i++)
            {
                for (int j = 0; j < MaxRows; j++)
                {
                    DrawSegment(i, j);
                }
            }
        }

        private void RedrawScreen(object sender, SizeChangedEventArgs e)
        {
            Canvas = new Canvas();
            DrawScreen(sender, e);
        }

        public void DrawSegment(int column, int row)
        {
            Rectangle r = new Rectangle();
            r.Width = (ActualWidth - 16) / MaxColumns;
            r.Height = (ActualHeight - 39) / MaxRows;
            r.Stroke = new SolidColorBrush(Colors.Black);
            r.StrokeThickness = 1;
            r.Fill = new SolidColorBrush(Colors.LimeGreen);
            Canvas.SetLeft(r, column * r.Width);
            Canvas.SetTop(r, row*r.Height);
            Canvas.Children.Add(r);

            TextBlock txt = new TextBlock();
            txt.Text = "Project";
            txt.Foreground = new SolidColorBrush(Colors.Black);
            txt.FontSize = r.Height / 3;
            txt.VerticalAlignment = VerticalAlignment.Center;
            txt.HorizontalAlignment = HorizontalAlignment.Center;
            txt.Measure(new Size(r.Width, r.Height));
            txt.Arrange(new Rect(new Size(r.Width, r.Height)));
            Canvas.SetLeft(txt, (column * r.Width) + ((r.Width - txt.ActualWidth) / 2));
            Canvas.SetTop(txt, (row * r.Height) + ((r.Height - txt.ActualHeight) / 2 ));
            Canvas.Children.Add(txt);

            Content = Canvas;
        }

        public void DrawMultiSegment(int column, int row)
        {
            
        }

        public void DrawMetaDataList(List<MetaData> meta)
        {
            
        }
    }
}