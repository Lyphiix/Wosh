using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

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

            ExcludedPipelinesCBox.IsChecked = Config.Default.ShouldExcludePipelines;
            if (!ExcludedPipelinesCBox.IsChecked.Value)
            {
                ExcludedPipelinesTextBox.IsEnabled = false;
            }
            ExcludedPipelinesTextBox.Text = Config.Default.ExcludedPipelines;

            ShouldShowBrokenProjectsCBox.IsChecked = Config.Default.ShouldShowBrokenStages;

            if (!ShouldShowBrokenProjectsCBox.IsChecked.Value)
            {
                BrokenProjectKeyTextBox.IsEnabled = false;
            }
            BrokenProjectKeyTextBox.Text = Config.Default.BrokenProjectKey;

            ShouldAutoExcludeOldProjectsCBox.IsChecked = Config.Default.ShouldAutoExcludeOldProjects;
            if (!ShouldAutoExcludeOldProjectsCBox.IsChecked.Value)
            {
                ExcludeProjectsAfterDaysTextBox.IsEnabled = false;
            }

            ExcludeProjectsAfterDaysTextBox.Text = Config.Default.ExcludeProjectsAfterDays.ToString(CultureInfo.InvariantCulture);

            PlaySoundsCBox.IsChecked = Config.Default.ShouldPlaySounds;
            if (!PlaySoundsCBox.IsChecked.Value)
            {
                SucceedSoundTextBox.IsEnabled = false;
                FailSoundTextBox.IsEnabled = false;
                BrowseSucceedButton.IsEnabled = false;
                BrowseFailButton.IsEnabled = false;
            }

            SucceedSoundTextBox.Text = Config.Default.SuccededSound;

            FailSoundTextBox.Text = Config.Default.FailedSound;

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
            Config.Default.ShouldExcludePipelines = ExcludedPipelinesCBox.IsChecked.Value;
            Config.Default.ExcludedPipelines = ExcludedPipelinesTextBox.Text;
            using (var reader = new StringReader(ExcludedPipelinesTextBox.Text))
            {
                // Loop over the lines in the string.
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    ParentWoshWindow.XmlParser.ExcludedPipelines.Add(line);
                }
            }

            Config.Default.ShouldShowBrokenStages = ShouldShowBrokenProjectsCBox.IsChecked.Value;

            Config.Default.BrokenProjectKey = BrokenProjectKeyTextBox.Text;

            Config.Default.ShouldAutoExcludeOldProjects = ShouldAutoExcludeOldProjectsCBox.IsChecked.Value;

            try
            {
                Config.Default.ExcludeProjectsAfterDays = int.Parse(ExcludeProjectsAfterDaysTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Please put a whole number in Exclude Projects After (Days)::", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Config.Default.ShouldPlaySounds = PlaySoundsCBox.IsChecked.Value;

            Config.Default.SuccededSound = SucceedSoundTextBox.Text;

            Config.Default.FailedSound = FailSoundTextBox.Text;

            Config.Default.Save();

            ParentWoshWindow.XmlParser.ShouldExcludePipelines = Config.Default.ShouldExcludePipelines;
            ParentWoshWindow.XmlParser.ShouldRemoveAfterExpirary = Config.Default.ShouldAutoExcludeOldProjects;
            ParentWoshWindow.XmlParser.ShouldShowBrokenProjects = Config.Default.ShouldShowBrokenStages;
            ParentWoshWindow.XmlParser.DaysToExpiry = Config.Default.ExcludeProjectsAfterDays;
            ParentWoshWindow.XmlParser.BrokenProjectKey = Config.Default.BrokenProjectKey;

            ParentWoshWindow.ShouldPlaySounds = Config.Default.ShouldPlaySounds;

            ParentWoshWindow.SoundHandler.SuccessSound = Config.Default.SuccededSound;
            ParentWoshWindow.SoundHandler.FailSound = Config.Default.FailedSound;

            ShouldDisplayWarning = false;
            Close();

            ParentWoshWindow.UpdateTimer.Interval = new TimeSpan(0, 0, Config.Default.PollSpeed);
            ParentWoshWindow.UpdateTimer.Start();

            ParentWoshWindow.ForceRedraw();
        }

        private void ExcludedPipelinesCBoxChecked(object sender, RoutedEventArgs e)
        {
            var box = (CheckBox) sender;
            ExcludedPipelinesTextBox.IsEnabled = box.IsChecked.Value;
        }

        private void ShouldAutoExcludeOldProjectsCBoxChecked(object sender, RoutedEventArgs e)
        {
            var box = (CheckBox) sender;
            ExcludeProjectsAfterDaysTextBox.IsEnabled = box.IsChecked.Value;
        }

        private void PlaySoundsCBoxChecked(object sender, RoutedEventArgs e)
        {
            var box = (CheckBox)sender;
            FailSoundTextBox.IsEnabled = box.IsChecked.Value;
            BrowseFailButton.IsEnabled = box.IsChecked.Value;
            SucceedSoundTextBox.IsEnabled = box.IsChecked.Value;
            BrowseSucceedButton.IsEnabled = box.IsChecked.Value;
        }

        private void BrowseFailSound(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                FailSoundTextBox.Text = openFileDialog.FileName;
            }
        }

        private void BrowseSucceedSound(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                SucceedSoundTextBox.Text = openFileDialog.FileName;
            }
        }

        private void SBSIChecked(object sender, RoutedEventArgs e)
        {
            var box = (CheckBox)sender;
            BrokenProjectKeyTextBox.IsEnabled = box.IsChecked.Value;
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