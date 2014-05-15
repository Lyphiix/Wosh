using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Wosh
{
    /// <summary>
    /// Interaction logic for WoshConfigurationWindow.xaml
    /// </summary>
    public partial class WoshConfigurationWindow : Window
    {
        /// <summary>
        /// This windows parent
        /// </summary>
        public WoshWindow ParentWoshWindow;

        /// <summary>
        /// Determines wether or not to display warning
        /// </summary>
        public bool ShouldDisplayWarning;

        public WoshConfigurationWindow(WoshWindow parent)
        {
            Name = "Preferences";

            ParentWoshWindow = parent;

            InitializeComponent();

            UrlTextBox.Text = Config.Default.URLToParse;

            PollSpeedTextBox.Text = Config.Default.PollSpeed.ToString(CultureInfo.InvariantCulture);

            NumOfColumnsTextBox.Text = Config.Default.NumOfColumns.ToString(CultureInfo.InvariantCulture);

            ExcludedPipelinesCBox.IsChecked = Config.Default.IsExcludedPipelinesCBoxChecked;
            if (!ExcludedPipelinesCBox.IsChecked.Value)
            {
                ExcludedPipelinesTextBox.IsEnabled = false;
            }
            ExcludedPipelinesTextBox.Text = Config.Default.ExcludedPipelines;
            
            ExcludedProjectsCBox.IsChecked = Config.Default.IsExcludedProjectsCBoxChecked;
            if (!ExcludedProjectsCBox.IsChecked.Value)
            {
                ExcludedProjectsTextBox.IsEnabled = false;
            }
            ExcludedProjectsTextBox.Text = Config.Default.ExcludedProjects;

            ShouldDisplayWarning = true;
        }

        private void SaveConfig(object sender, RoutedEventArgs e)
        {
            ParentWoshWindow.UpdateTimer.Stop();
            Config.Default.URLToParse = UrlTextBox.Text;
            try
            {
                Config.Default.PollSpeed = int.Parse(PollSpeedTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Please put a whole number in Poll Speed:", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                Config.Default.NumOfColumns = int.Parse(NumOfColumnsTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Please put a whole number in Number of Columns:", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Config.Default.IsExcludedPipelinesCBoxChecked = ExcludedPipelinesCBox.IsChecked.Value;
            Config.Default.ExcludedPipelines = ExcludedPipelinesTextBox.Text;

            Config.Default.IsExcludedProjectsCBoxChecked = ExcludedProjectsCBox.IsChecked.Value;
            Config.Default.ExcludedProjects = ExcludedProjectsTextBox.Text;

            Config.Default.Save();

            ShouldDisplayWarning = false;
            Close();

            ParentWoshWindow.UpdateTimer.Interval = new TimeSpan(0, 0, Config.Default.PollSpeed);
            ParentWoshWindow.UpdateTimer.Start();

            ParentWoshWindow.ForceRedraw();
        }

        private void ExcludedPipelinesCBoxChecked(object sender, RoutedEventArgs e)
        {
            var box = (CheckBox)sender;
            ExcludedPipelinesTextBox.IsEnabled = box.IsChecked.Value;
        }

        private void ExcludedProjectsCBoxChecked(object sender, RoutedEventArgs e)
        {
            var box = (CheckBox)sender;
            ExcludedProjectsTextBox.IsEnabled = box.IsChecked.Value;
        }

        public void ShowWarning(CancelEventArgs e)
        {
            var result = MessageBox.Show("Changed settings will not be saved. Close anyway?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ShouldDisplayWarning)
            {
                ShowWarning(e);
            }
            ShouldDisplayWarning = true;
        }
    }
}