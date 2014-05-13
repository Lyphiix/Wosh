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
    public interface IXmlParser
    {
        List<MetaData> ParseString(String input);
    }

    public class XmlParser : IXmlParser
    {
        public List<MetaData> ParseString(String input)
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
    }
}
