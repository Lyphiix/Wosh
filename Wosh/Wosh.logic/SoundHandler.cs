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
                        switch (CompareProjects(pold, pnew)) {
                            case SoundHandlerSoundType.SoundHandlerSoundFail:
                                if (!pold.HasPlayedSound) failSound = true;
                                pnew.HasPlayedSound = true;
                                break;
                            case SoundHandlerSoundType.SoundHandlerSoundSuccess:
                                if (!pold.HasPlayedSound) successSound = true;
                                pnew.HasPlayedSound = true;
                                break;
                            case SoundHandlerSoundType.SoundHandlerSoundNo:
                                pnew.HasPlayedSound = false;
                                break;
                            default:
                                pnew.HasPlayedSound = false;
                                break;
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

        private SoundHandlerSoundType CompareProjects(Project one, Project two)
        {
            if (one.Name.Equals(two.Name))
            {
                if (one.Status() > two.Status())
                {
                    return one.Status();
                }
                else
                {
                    return two.Status();
                }
            }
            return SoundHandlerSoundType.SoundHandlerSoundNo;
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
