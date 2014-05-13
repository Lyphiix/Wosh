using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Wosh.logic
{
    public class MetaData
    {
        public String Name;
        // Used for grouping.
        public String Stage;
        public String Job;
        // --
        public String Activity;
        public String LastBuildStatus;
        public String LastBuildLabel;
        public String LastBuildTime;
        public String WebUrl;
    }
    public class GroupedMetaData
    {
        public String Name;
        public List<MetaData> SubData;
    }
}
