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
            timer.Interval = new System.Random().Next(60000 * 2, 120000 * 2);
            timer.Elapsed += CallBack;
            timer.Enabled = true;
        }

        private void CallBack(Object source, System.Timers.ElapsedEventArgs e)
        {
            cat.SpeakAsync("Meow");

        }

        public void PlaySound(List<Project> oldPipe, List<Project> currentPipe)
        {
            Dictionary<String, Project> oldDict = GetProjectListAsDict(oldPipe);
            Dictionary<String, Project> newDict = GetProjectListAsDict(currentPipe);

            bool failSound = false;
            bool successSound = false;

            foreach (String key in oldDict.Keys)
            {
                Project pold;
                if (oldDict.TryGetValue(key, out pold))
                {
                    Project pnew;
                    if (newDict.TryGetValue(key, out pnew))
                    {
                        if (!pold.Status().Equals(pnew.Status()))
                        {
                            switch (pnew.Status())
                            {
                                case SoundHandlerSoundType.SoundHandlerSoundFail:
                                    failSound = true;
                                    break;
                                case SoundHandlerSoundType.SoundHandlerSoundSuccess:
                                    successSound = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            if (failSound)
            {
                System.Media.SystemSounds.Hand.Play();
                System.Threading.Thread.Sleep(1000);
            }

            if (successSound)
            {
                System.Media.SystemSounds.Exclamation.Play();
                System.Threading.Thread.Sleep(1000);
            }

            Console.WriteLine("Success: {0} Sound, Failure: {1} Sound", successSound ? "Played" : "Didn't Play", failSound ? "Played" : "Didn't Play");
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
