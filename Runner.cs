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
        private Task[] midConnectionTasks;
        private Task[] postConnectionTasks;
        public Runner()
        {
            taskHandler = new TaskFactory();
            preConnectionTasks = new Task[2];
            midConnectionTasks = new Task[2];
            postConnectionTasks = new Task[2];
            preConnectionTasks[0] = Task.Factory.StartNew(() => listenForConnection());
            Task.Factory.ContinueWhenAny(preConnectionTasks, (b) => midConnectionTasks[0]);
            //taskHandler.StartNew(() => receiveMessage(index));
        }

        public void makeConnectionThread()
        {


        }
    }
}
