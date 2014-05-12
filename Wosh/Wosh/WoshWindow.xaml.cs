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
using NUnit.Framework;

namespace Wosh
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WoshWindow : Window
    {
        public Canvas Canvas;

        public int MaxColumns;
        public int MaxRows;


        public WoshWindow()
        {
            Canvas = new Canvas();
            MaxColumns = 2;
            MaxRows = 10;
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
            Canvas.SetLeft(txt, (column * r.Width) + ((r.Width - txt.RenderSize.Width) / 2));
            Canvas.SetTop(txt, (row * r.Height) + (r.Height / 3));
            Canvas.Children.Add(txt);


            Content = Canvas;
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
    }

    [TestFixture]
    public class WoshTest
    {
        [Test]
        public static void play() {
            int o = 1;
            int e = 0;
            Console.WriteLine("Error: {0}", o/e);
        }
    }
}