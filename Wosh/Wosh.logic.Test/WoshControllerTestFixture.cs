using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wosh.logic;
using NUnit.Framework;

namespace Wosh.logic.Test
{
    [TestFixture]
    class WoshControllerTestFixture
    {
        [Test]
        public void TestWoshControllerTimer()
        {
            WoshController controller = new WoshController();
            Console.ReadKey();
        }
    }
}
