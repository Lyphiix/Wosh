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
        public int occurence;
        // --
        public String Activity;
        public String LastBuildStatus;
        public String LastBuildLabel;
        public String LastBuildTime;
        public String WebUrl;
    }
    public class Pipeline
    {
        public String Name;
        public List<Project> SubData;
        public bool ShouldPlaySoundComparedTo(Pipeline comp)
        {
            foreach (Project p in SubData)
            {
                foreach (Project c in comp.SubData)
                {
                    // Checks to see whether the data we are looking at is the same type.
                    if (c.Name.Equals(p.Name))
                    {
                        // Checks to see whether both are success, if they are, we do not want to know about it.
                        if (!(c.LastBuildStatus.Equals("Success") && p.LastBuildStatus.Equals("Success")))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
