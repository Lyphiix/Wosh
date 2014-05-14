using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Wosh.logic
{
    public class MetaData
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

        public Color ColorForMetaData()
        {
            if (Activity.Equals("Building")) return Colors.Yellow;
            if (LastBuildStatus.Equals("Success")) return Colors.LimeGreen;
            if (LastBuildStatus.Equals("Failure")) return Colors.Red;
            return Colors.White;
        }
    }
    public class GroupedMetaData
    {
        public String Name;
        public List<MetaData> SubData;

        public Color ColorForGroupedMetaData()
        {
            foreach (var meta in SubData)
            {
                if (meta.LastBuildStatus.Equals("Failure")) return Colors.Red;
                if (meta.Activity.Equals("Building")) return Colors.Yellow;
            }
            return Colors.LimeGreen;
        }
    }
}
