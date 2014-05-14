using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wosh.logic
{
    public class SoundHandler
    {
        public static bool shouldPlaySound(List<Pipeline> oldPipe, List<Pipeline> currentPipe)
        {
            // projectList
            if (currentPipe.Count > oldPipe.Count)
            {
                // There has been a new addition, notify.
                return true;
            }

            if (currentPipe.Count == oldPipe.Count)
            {
                // There has been no noticed change.
                // Time to look for one.
            }

            return false;
        }
    }
}   
