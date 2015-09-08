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
        private int connectingClientIndex;

        private delegate void clientConnectedDelegate();
        private event clientConnectedDelegate clientConnected;

        public Runner()
        {
            taskHandler = new TaskFactory();
            preConnectionTasks = new Task[2];
            midConnectionTasks = new Task[2];
            postConnectionTasks = new Task[2];
            while (true)
            {
                if (preConnectionTasks[0] == null)
                {
                    startListeningAsync();
                }
                else
                {
                    if (preConnectionTasks[0].Status == TaskStatus.RanToCompletion)
                    {
                        listeningAsync();
                    }
                }
                messagingAsync();
                while (preConnectionTasks[0] == null)
                {
                    Console.WriteLine("Initializing...");
                    Thread.Sleep(1000);
                }
            }
            //taskHandler.StartNew(() => receiveMessage(index));
        }

        public void startListeningAsync()
        {
            preConnectionTasks[0] = Task.Factory.StartNew(() => listenForConnection());
        }

        public async void listeningAsync()
        {
            preConnectionTasks[0].Start();
        }

        private async void messagingAsync()
        {
            if (connectedClients.Any())
            {
                foreach (ConnectionModel client in connectedClients)
                {
                    if (client.isConnected == false)
                    {
                        await processConnectionRequest(client);
                    }
                    else
                    {
                        taskHandler.StartNew(() => receiveMessage(client));
                    }
                }
            }
        }
    }
}
