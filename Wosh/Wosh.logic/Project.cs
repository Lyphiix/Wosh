﻿using System;
using System.Collections.Generic;

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

        public bool HasPlayedSound;
        
    }
    public class Pipeline
    {
        public String Name;
        public List<Project> SubData;
        public bool IsBrokenProject;
    }
}
