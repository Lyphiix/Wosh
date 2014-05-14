using System;
using System.Windows;
using Wosh.Properties;

namespace Wosh
{
    /// <summary>
    /// Interaction logic for WoshConfigurationWindow.xaml
    /// </summary>
    public partial class WoshConfigurationWindow : Window
    {
        public WoshConfigurationWindow()
        {
            InitializeComponent();

            UrlTextBox.Text = Settings.Default.URLToParse;
            PollSpeedTextBox.Text = Settings.Default.PollSpeed.ToString();
        }
    }
}
