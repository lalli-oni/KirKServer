using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace KirkServer
{
    public class Program
    {
        private static ServerListener listener;
        private static string broadcastingString;
        private static Task listenConnectionsTask;
        private static Task broadcastMessageTask;
        static void Main(string[] args)
        {
            //TODO: Possibility of using Thread.Join() to regulate this. Creating 2 Thread objects.
            listener = new ServerListener();
            listenConnectionsTask = Task.Run(() =>
            {
                while (true)
                {
                    ConnectionModel incomingConnection = listener.ListenForConnection();
                    if (listener.ProcessConnectionRequest(incomingConnection))
                    {
                        Task waitForMessageTask = Task.Run(() =>
                        {
                            while (incomingConnection.isConnected)
                            {
                                broadcastingString = incomingConnection.ReceiveMessage();
                                BroadcastMessage(broadcastingString, incomingConnection);
                            }
                        });
                        int i = 0;
                        while (listener.connectedClients.TryAdd(incomingConnection, incomingConnection.UserName) || i < 100)
                        {
                            i++;
                        }
                        listener.nrOfClients = listener.connectedClients.Count;
                    }
                }
            });

            while (true)
            {

            }
        }

        public static async Task BroadcastMessage(string message, ConnectionModel sender)
        {
            foreach (var receivingClient in listener.connectedClients)
            {
                try
                {
                    await receivingClient.Key.SendMessageAsync(sender.UserName + "~" + message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    if (!receivingClient.Key.isConnected)
                    {
                        string outValue;
                        for (int i = 0; i < 10; i++)
                        {
                            listener.connectedClients.TryRemove(receivingClient.Key, out outValue);
                        }
                    }
                }
            }
        }
    }
}
