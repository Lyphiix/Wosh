using System;
using System.Collections.Generic;

namespace Wosh.logic
{
    public class SoundHandler
    {
        public enum SoundHandlerSoundType
        {
            SoundHandlerSoundFail = 2,
            SoundHandlerSoundSuccess = 1,
            SoundHandlerSoundNo = 0,
        }

        private System.Speech.Synthesis.SpeechSynthesizer cat;
        public SoundHandler()
        {
            cat = new System.Speech.Synthesis.SpeechSynthesizer();
            cat.SetOutputToDefaultAudioDevice();
            cat.SelectVoiceByHints(System.Speech.Synthesis.VoiceGender.Female, System.Speech.Synthesis.VoiceAge.Teen, 0, new System.Globalization.CultureInfo("zh-Hans"));
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = new System.Random().Next(60000 * 60, 120000 * 60);
            timer.Elapsed += CallBack;
            timer.Enabled = true;

            FailSound = "";
            SuccessSound = "";
        }

        private void CallBack(Object source, System.Timers.ElapsedEventArgs e)
        {
            cat.SpeakAsync("Meow");
        }

        public String FailSound;
        public String SuccessSound;

        public void PlaySound(List<Project> old, List<Project> current)
        {
            bool fail = false;
            bool succeed = false;

            var oldDict = GetProjectListAsDict(old);
            var currentDict = GetProjectListAsDict(current);

            foreach (Project oldP in oldDict.Values)
            {
                foreach (Project currentP in currentDict.Values)
                {
                    if (!currentP.Activity.Equals("Building"))
                    {
                        // TODO - make the sounds work
                    }
                }
            }

            if (fail)
            {

            }
            if (succeed)
            {

            }

            System.Threading.ThreadPool.QueueUserWorkItem(callback =>
            {
                System.Windows.Media.MediaPlayer x = new System.Windows.Media.MediaPlayer();
                x.Open(new Uri(SuccessSound));
                x.Play();
            });
        }

        private Dictionary<String, Project> GetProjectListAsDict(List<Project> input)
        {
            Dictionary<String, Project> projectStatus = new Dictionary<String, Project>();
            foreach (Project pt in input)
            {
                projectStatus.Add(pt.Name, pt);
            }
            return projectStatus;
        }
    }
}   
