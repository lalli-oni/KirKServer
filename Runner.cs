using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KirkServer
{
    class Runner : ServerListener
    {
        public Runner()
        {
            running();
        }

        public void running()
        {
            receiveMessage(base.listener);

        }
    }
}
