using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KirkServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServerListener listener = new ServerListener();
            Task listenConnectionsTask = Task.Run(() =>
            {
                while (true)
                {
                    ConnectionModel incomingConnection = listener.listenForConnection();
                    if (listener.processConnectionRequest(incomingConnection))
                    {
                        listener.connectedClients.Add(incomingConnection);
                        Task listenMessagesTask = Task.Run(() =>
                        {
                            while (true)
                            {
                                foreach (var client in listener.connectedClients)
                                {
                                    string msg = client.receiveMessage();
                                    if (msg != null)
                                    {
                                        Task broadcastMessage = Task.Run(() =>
                                        {
                                            foreach (var receivingClient in listener.connectedClients)
                                            {
                                                receivingClient.sendMessage(msg);
                                            }
                                        });
                                    }
                                }
                            }
                        });
                    }
                }
            });

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
