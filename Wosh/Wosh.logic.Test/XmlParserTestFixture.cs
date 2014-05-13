using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Wosh.logic;


namespace Wosh.logic.Test
{
    [TestFixture]
    public class XmlParserTestFixture
    {    
        [Test]
        public  void TestHttpConection()
        {
            String content = new System.Net.WebClient().DownloadString(@"http://augo/go/cctray.xml");
            Assert.That(content, Is.Not.Null.Or.EqualTo(string.Empty));
        }

        [Test]
        public void TestXmlParser()
        {
            var content = 
@"<Projects>
<Project name='SupporterTransactionSystem :: Build' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='1.0.0.1500' lastBuildTime='2014-04-24T17:05:22' webUrl='http://go/go/pipelines/SupporterTransactionSystem/1500/Build/1'/>
<Project name='SupporterTransactionSystem :: Build :: build_and_unit_test_debug' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='1.0.0.1500' lastBuildTime='2014-04-24T17:05:22' webUrl='http://go/go/tab/build/detail/SupporterTransactionSystem/1500/Build/1/build_and_unit_test_debug'/>
<Project name='SupporterTransactionSystem :: UnitTests' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='1.0.0.1500' lastBuildTime='2014-04-24T17:09:27' webUrl='http://go/go/pipelines/SupporterTransactionSystem/1500/UnitTests/1'/>
<Project name='SupporterTransactionSystem :: UnitTests :: unit_tests' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='1.0.0.1500' lastBuildTime='2014-04-24T17:09:27' webUrl='http://go/go/tab/build/detail/SupporterTransactionSystem/1500/UnitTests/1/unit_tests'/>
<Project name='SupporterTransactionSystem :: PublishNugetPackages' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='1.0.0.1500' lastBuildTime='2014-04-24T17:11:24' webUrl='http://go/go/pipelines/SupporterTransactionSystem/1500/PublishNugetPackages/1'/>
</Projects>";

            IXmlParser xmlParser = new XmlParser();
            List<MetaData> data = xmlParser.ParseString(content);
            foreach (MetaData d in data)
            {
                Assert.That(d.Name, Is.Not.Null);
            }
        }
    }
}
