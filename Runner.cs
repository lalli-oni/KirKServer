using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KirkServer
{
    public class Runner : ServerListener
    {
        private static TaskFactory taskHandler;
        private Task listeningTask;
        private int connectingClientIndex;
        

        public Runner()
        {
            taskHandler = new TaskFactory(TaskScheduler.Default);
            while (true)
            {
                if (listeningTask == null)
                {
                    startListeningAsync();
                }
                else
                {
                    if (listeningTask.Status == TaskStatus.RanToCompletion)
                    {
                        listeningAsync();
                    }
                }
                messagingAsync();
                while (listeningTask == null)
                {
                    Console.WriteLine("Initializing...");
                    Thread.Sleep(1000);
                }
            }
            //taskHandler.StartNew(() => receiveMessage(index));
        }

        public void startListeningAsync()
        {
            listeningTask = Task.Factory.StartNew(() => ListenForConnection());
        }

        public async void listeningAsync()
        {
            listeningTask.Start();
        }

        private async void messagingAsync()
        {
            if (connectedClients.Any())
            {
                Parallel.ForEach(connectedClients, client =>
                {
                    //if (client.isConnected == false)
                    //{
                    //    processConnectionRequest(client);
                    //}
                    //else
                    //{
                    //    client.receiveMessage();
                    //}
                });
                //foreach (ConnectionModel client in connectedClients)
                //{
                //    if (client.isConnected == false)
                //    {
                //        await processConnectionRequest(client);
                //    }
                //    else
                //    {
                //        client.receiveMessage();
                //    }
                //}
            }
        }
    }
}
