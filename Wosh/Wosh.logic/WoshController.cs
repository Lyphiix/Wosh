using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wosh;

namespace Wosh.logic
{
    public class WoshController
    {
        System.Timers.Timer updateTimer;

        public WoshController()
        {
            updateTimer = new System.Timers.Timer(10000);
            updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            updateTimer.Interval = 2000;
            updateTimer.Enabled = true;
        }

        private static void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Timer triggered");
        }
    }
}
