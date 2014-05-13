using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Wosh.logic
{
    /*
     * 
     * String content = new System.Net.WebClient().DownloadString(@"http://augo/go/cctray.xml");
     * List<MetaData> data = XmlParser.ParseString(content);
     * foreach (MetaData d in data) {
     *      // Do Something Here
     * }
     * */
    public class XmlParser
    {
        static public List<MetaData> ParseString(String input)
        {
            XmlReader reader = XmlReader.Create(new System.IO.StringReader(input));
            List<MetaData> list = new List<MetaData>();

            reader.ReadToFollowing("Project");

            while (!reader.EOF)
            {
                MetaData data = new MetaData();

                if (reader.MoveToAttribute("name")) data.Name = reader.Value;
                if (reader.MoveToAttribute("activity")) data.Activity = reader.Value;
                if (reader.MoveToAttribute("lastBuildStatus")) data.LastBuildStatus = reader.Value;
                if (reader.MoveToAttribute("lastBuildLabel")) data.LastBuildLabel = reader.Value;
                if (reader.MoveToAttribute("lastBuildTime")) data.LastBuildTime = reader.Value;
                if (reader.MoveToAttribute("webUrl")) data.WebUrl = reader.Value;

                list.Add(data);
                reader.ReadToFollowing("Project");
            }
            return list;
        }

        static public List<GroupedMetaData> ParseStringForGroup(String input)
        {

            List<MetaData> data = ParseString(input);

            Dictionary<String, GroupedMetaData> groupData = new Dictionary<String, GroupedMetaData>();

            foreach (MetaData d in data)
            {
                Char[] x = { ':', ':' };
                String[] v = d.Name.Split(x, StringSplitOptions.RemoveEmptyEntries);
                GroupedMetaData value;
                var v0 = v[0].Trim();
                if (groupData.TryGetValue(v0, out value)) { }
                else
                {
                    // No grouped data, must create our own.
                    value = new GroupedMetaData();
                    value.Name = v0;
                    value.SubData = new List<MetaData>();
                    groupData.Add(v0, value);
                }
                d.Stage = v.Length >= 2 ? v[1].Trim() : String.Empty;
                d.Job = v.Length >= 3 ? v[2].Trim() : String.Empty;
                value.SubData.Add(d);
            }

            return groupData.Values.ToList();
        }
    }
}
