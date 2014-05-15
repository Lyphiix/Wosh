﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        // A list of the projects names to be excluded from display.
        public List<String> ExcludedGroupProjects;
        public List<String> ExcludedIndividualProjects;
        // If true, parser will exclude group projects and individual projects from the output list
        public bool ShouldExcludeProjects;

        public int DaysToExpirary;
        // If true, parser will exclude individual projects which are older than x days.
        public bool ShouldRemoveAfterExpirary;

        public XmlParser()
        {
            ExcludedGroupProjects = new List<string>();
            ExcludedIndividualProjects = new List<string>();
            ShouldExcludeProjects = false;
            DaysToExpirary = 30;
            ShouldRemoveAfterExpirary = false;
        }

        public List<Project> ParseString(String input)
        {
            using (var stringReader = new StringReader(input))
            using (var reader = XmlReader.Create(stringReader))
            {
                List<Project> list = new List<Project>();

                while (true)
                {
                    reader.ReadToFollowing("Project");
                    if (reader.EOF) break;

                    Project data = new Project();
                    #region
                    if (reader.MoveToAttribute("name")) data.Name = reader.Value;
                    if (reader.MoveToAttribute("activity")) data.Activity = reader.Value;
                    if (reader.MoveToAttribute("lastBuildStatus")) data.LastBuildStatus = reader.Value;
                    if (reader.MoveToAttribute("lastBuildLabel")) data.LastBuildLabel = reader.Value;
                    if (reader.MoveToAttribute("lastBuildTime")) data.LastBuildTime = reader.Value;
                    if (reader.MoveToAttribute("webUrl")) data.WebUrl = reader.Value;
                    #endregion

                    // Set the group name, the stage, and the job.
                    String[] splitName = data.Name.Split(new[] {":", ":"}, StringSplitOptions.RemoveEmptyEntries);
                    data.GroupName = splitName[0].Trim();
                    data.Stage = splitName.Length >= 2 ? splitName[1].Trim() : String.Empty;
                    data.Job = splitName.Length >= 3 ? splitName[2].Trim() : String.Empty;
                    // If the project name is in the excluded indiviual projects, don't add it to the ouput list.
                    #region
                    if (ShouldExcludeProjects)
                    {
                        if (ExcludedIndividualProjects.Contains(data.Name)) continue;
                    }
                    #endregion

                    // If the project is out of data, exclud it from the output list.
                    #region
                    if (ShouldRemoveAfterExpirary)
                    {
                        TimeSpan difference = getTimeDifferenceBetweenDates(data.LastBuildTime);
                        if (difference.TotalDays >= DaysToExpirary)
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

        public TimeSpan getTimeDifferenceBetweenDates(String date)
        {
            // Time format, (YEAR)-(MONTH)-(DAY)T(HOUR):(MINUTE):(SECOND)
            DateTime now = DateTime.Now;

            string[] time = date.Split(new[] { '-', 'T', ':' }, StringSplitOptions.RemoveEmptyEntries);

            DateTime then = new DateTime(Int32.Parse(time[0]), Int32.Parse(time[1]), Int32.Parse(time[2]),
                                         Int32.Parse(time[3]), Int32.Parse(time[4]), Int32.Parse(time[5]));
            TimeSpan difference = now - then;

            return difference;
        }

        // Splits it up into different pipelines.
        public List<Pipeline> ParseToPipeline(List<Project> input)
        {
            // Create a dictornary to store the grouped data in.
            Dictionary<String, Pipeline> groupData = new Dictionary<String, Pipeline>();
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
            foreach (Project data in input)
            {
                Pipeline value;
                String groupName = data.GroupName;
                // Pass, because we don't want to add this data to the output.
                if (ShouldExcludeProjects)
                {
                    if (ExcludedGroupProjects.Contains(groupName)) continue;
                }
                // Look for the group, if it isn't there, create it.
                if (!groupData.TryGetValue(groupName, out value)) {
                    // No grouped data, must create our own.
                    value = new Pipeline();
                    value.Name = groupName;
                    value.SubData = new List<Project>();
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
