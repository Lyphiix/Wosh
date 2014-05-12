using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace Wosh
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
        public static List<MetaData> ParseString(String input)
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

    public class MetaData
    {
        public String Name;
        public String Activity;
        public String LastBuildStatus;
        public String LastBuildLabel;
        public String LastBuildTime;
        public String WebUrl;

        public MetaData()
        {
            Name = "(null)";
            Activity = "(null)";
            LastBuildLabel = "(null)";
            LastBuildStatus = "(null)";
            LastBuildTime = "(null)";
            WebUrl = "(null)";
        }
    }

    [TestFixture]
    public class Test
    {
        [Test]
        public static void tester() {
            Console.WriteLine("Hello, World");
        }
    }

}