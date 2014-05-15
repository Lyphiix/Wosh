using System;
using System.Collections.Generic;
using System.Windows.Media;
using NUnit.Framework;

namespace Wosh.logic
{
    public class Project
    {
        public String Name;
        // Used for grouping.
        public String GroupName;
        public String Stage;
        public String Job;
        // --
        public String Activity;
        public String LastBuildStatus;
        public String LastBuildLabel;
        public String LastBuildTime;
        public String WebUrl;

        public bool hasPlayedSound;
        public SoundHandler.SoundHandlerSoundType Status()
        {
            if (LastBuildStatus.Equals("Success"))
            {
                return SoundHandler.SoundHandlerSoundType.SoundHandlerSoundSuccess;
            }
            else if (LastBuildStatus.Equals("Failure"))
            {
                return SoundHandler.SoundHandlerSoundType.SoundHandlerSoundFail;
            }
            return SoundHandler.SoundHandlerSoundType.SoundHandlerSoundNo;
        }
    }
    public class Pipeline
    {
        public String Name;
        public List<Project> SubData;
    }
}
