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
                    ConnectionModel incomingConnection = listener.listenForConnection();
                    if (listener.processConnectionRequest(incomingConnection))
                    {
                        listener.connectedClients.Add(incomingConnection);
                        Task waitForMessageTask = Task.Run(() =>
                        {
                            while (incomingConnection.isConnected)
                            {
                                broadcastingString = incomingConnection.receiveMessage();
                                broadcastMessage(broadcastingString);
                            }
                        });
                        listener.nrOfClients = listener.connectedClients.Count;
                    }
                }
            });

            while (true)
            {

            }
        }

        public static async Task listenForConnection()
        {
            await listener.listenForConnectionAsync();
        }

        public static async Task listenForMessage()
        {
            foreach (ConnectionModel client in listener.connectedClients)
            {
                if (client.isConnected)
                {
                    broadcastingString = await client.receiveMessageAsync();
                }
            }
            Console.WriteLine("Received message: " + broadcastingString);
            foreach (ConnectionModel client in listener.connectedClients)
            {
                await client.sendMessageAsync(broadcastingString);
            }
                //Parallel.ForEach(listener.connectedClients, (client) => client.sendMessage(broadcastingString));
                broadcastingString = null;
        }

        public static async Task broadcastMessage(string message)
        {
            foreach (var receivingClient in listener.connectedClients)
            {
                await receivingClient.sendMessageAsync(message);
            }
        }
    }
}
