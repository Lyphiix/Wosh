using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Wosh.logic
{
    /*
     * 
     * String content = new System.Net.WebClient().DownloadString(@"http://augo/go/cctray.xml");
     * List<Project> data = XmlParser.ParseString(content);
     * foreach (Project d in data) {
     *      // Do Something Here
     * }
     * */
    public class XmlParser
    {
        // A list of the projects names to be excluded from display
        public List<String> ExcludedPipelines;
        public List<String> ExcludedProjects;

        /// <summary>
        /// If true, parser will exclude pipelines from the output list
        /// </summary>
        public bool ShouldExcludePipelines;

        /// <summary>
        /// If true, parser will exclude projects from the output list
        /// </summary>
        public bool ShouldExcludeProjects;

        /// <summary>
        /// Number of days a project has to be left untouched before it gets excluded automatically
        /// </summary>
        public int DaysToExpiry;

        /// <summary>
        /// If true, parser will exclude individual projects which are older than x days
        /// </summary>
        public bool ShouldRemoveAfterExpirary;

        public XmlParser()
        {
            ExcludedPipelines = new List<string>();
            ExcludedProjects = new List<string>();
            ShouldExcludeProjects = false;
            DaysToExpiry = 30;
            ShouldRemoveAfterExpirary = false;
        }

        public List<Project> ParseString(String input)
        {
            using (var stringReader = new StringReader(input))
            using (var reader = XmlReader.Create(stringReader))
            {
                var list = new List<Project>();

                while (true)
                {
                    reader.ReadToFollowing("Project");
                    if (reader.EOF) break;

                    var data = new Project();
                    #region
                    if (reader.MoveToAttribute("name")) data.Name = reader.Value;
                    if (reader.MoveToAttribute("activity")) data.Activity = reader.Value;
                    if (reader.MoveToAttribute("lastBuildStatus")) data.LastBuildStatus = reader.Value;
                    if (reader.MoveToAttribute("lastBuildLabel")) data.LastBuildLabel = reader.Value;
                    if (reader.MoveToAttribute("lastBuildTime")) data.LastBuildTime = reader.Value;
                    if (reader.MoveToAttribute("webUrl")) data.WebUrl = reader.Value;
                    #endregion

                    // Set the group name, the stage, and the job.
                    var splitName = data.Name.Split(new[] {":", ":"}, StringSplitOptions.RemoveEmptyEntries);
                    data.GroupName = splitName[0].Trim();
                    data.Stage = splitName.Length >= 2 ? splitName[1].Trim() : String.Empty;
                    data.Job = splitName.Length >= 3 ? splitName[2].Trim() : String.Empty;
                    // If the project name is in the excluded indiviual projects, don't add it to the ouput list.
                    #region
                    if (ShouldExcludeProjects)
                    {
                        if (ExcludedProjects.Contains(data.Name)) continue;
                    }
                    #endregion

                    // If the project is out of data, exclud it from the output list.
                    #region
                    if (ShouldRemoveAfterExpirary)
                    {
                        var difference = GetTimeDifferenceBetweenDates(data.LastBuildTime);
                        if (difference.TotalDays >= DaysToExpiry)
                        {
                            continue;
                        }
                    }
                    #endregion

                    list.Add(data);
                }
                return list;
            }
        }

        public TimeSpan GetTimeDifferenceBetweenDates(String date)
        {
            // Time format, (YEAR)-(MONTH)-(DAY)T(HOUR):(MINUTE):(SECOND)
            var now = DateTime.Now;

            var time = date.Split(new[] { '-', 'T', ':' }, StringSplitOptions.RemoveEmptyEntries);

            var then = new DateTime(Int32.Parse(time[0]), Int32.Parse(time[1]), Int32.Parse(time[2]),
                                         Int32.Parse(time[3]), Int32.Parse(time[4]), Int32.Parse(time[5]));
            var difference = now - then;

            return difference;
        }

        // Splits it up into different pipelines.
        public List<Pipeline> ParseToPipeline(List<Project> input)
        {
            // Create a dictornary to store the grouped data in.
            var groupData = new Dictionary<String, Pipeline>();
            // Comment Block
            #region
            /*
             * Loop throuh all the metadata.
             * 
             * If there is a group with the same "prefix" (E.g "Support :: Build", prefix is "Support")
             *      Add it the the group.
             * If there isn't, create a new grouped metadata object to store it in.
             * 
             * If the group name (the prefix) is in the excluded group projects class varable, we won't add it to the dictonary.
             */
            #endregion 
            // Code Block
            #region
            foreach (var data in input)
            {
                Pipeline value;
                var groupName = data.GroupName;
                // Pass, because we don't want to add this data to the output.
                if (ShouldExcludePipelines)
                {
                    if (ExcludedPipelines.Contains(groupName)) continue;
                }
                // Look for the group, if it isn't there, create it.
                if (!groupData.TryGetValue(groupName, out value)) {
                    // No grouped data, must create our own.
                    value = new Pipeline {Name = groupName, SubData = new List<Project>()};
                    groupData.Add(groupName, value);
                }
                // Add the metadata to the group data's subdata.
                value.SubData.Add(data);
            }
            #endregion

            return groupData.Values.ToList();
        }
    }
}
