using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace Wosh
{
    public class XmlParser
    {
        public static List<MetaData> parseFile(String filePath)
        {
            string x = System.IO.File.ReadAllText(filePath);
            XmlReader reader = XmlReader.Create(new System.IO.StringReader(x));
            List<MetaData> list = new List<MetaData>();

            reader.ReadToFollowing("Project");

            while (!reader.EOF)
            {
                MetaData data = new MetaData();

                if (reader.MoveToAttribute("name")) data.name = reader.Value;
                if (reader.MoveToAttribute("activity")) data.activity = reader.Value;
                if (reader.MoveToAttribute("lastBuildStatus")) data.lastBuildStatus = reader.Value;
                if (reader.MoveToAttribute("lastBuildLabel")) data.lastBuildLabel = reader.Value;
                if (reader.MoveToAttribute("lastBuildTime")) data.lastBuildTime = reader.Value;
                if (reader.MoveToAttribute("webUrl")) data.webUrl = reader.Value;

                list.Add(data);
                reader.ReadToFollowing("Project");
            }

            return list;
        }
    }

    public class MetaData
    {
        public String name;
        public String activity;
        public String lastBuildStatus;
        public String lastBuildLabel;
        public String lastBuildTime;
        public String webUrl;

        public MetaData()
        {
            name = "(null)";
            activity = "(null)";
            lastBuildLabel = "(null)";
            lastBuildStatus = "(null)";
            lastBuildTime = "(null)";
            webUrl = "(null)";
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