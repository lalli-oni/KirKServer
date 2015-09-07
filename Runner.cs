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
        private static TaskFactory taskHandler;
        private Task[] preConnectionTasks;
        public Runner()
        {
            taskHandler = new TaskFactory();
            taskHandler.StartNew(listenForConnection);
            //taskHandler.StartNew(() => receiveMessage(index));
        }

        public void makeConnectionThread()
        {


        }
    }
}
