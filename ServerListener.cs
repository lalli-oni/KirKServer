using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KirkServer
{
    class ServerListener : AddressService
    {
        public TcpListener listener;
        public List<ConnectionModel> connectedClients;

        public ServerListener()
        {
            //Establishes a local endpoint by getting the localhost IP using the AddressService
            IPEndPoint serverEndPoint = new IPEndPoint(getIP("localhost").Result, 6789);
            listener = new TcpListener(serverEndPoint);
            connectedClients = new List<ConnectionModel>();
        }

        public void receiveMessage(int connectionId)
        {
            while (connectedClients[connectionId].isConnected)
            {
                string message;
                while ((message = connectedClients[connectionId].ReaderStream.ReadLine()) != null)
                {
                    broadcastMessage(message);
                    Console.WriteLine(message);
                    connectedClients[connectionId].ReaderStream.Dispose();
                }
            }
        }

        public void broadcastMessage(string broadcastingMessage)
        {
            foreach (ConnectionModel client in connectedClients)
            {
                client.WriterStream.WriteLineAsync(broadcastingMessage);
            }
        }

        public void listenForConnection()
        {
            listener.Start(100);
            while (true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests. 
                // You could also user server.AcceptSocket() here.
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Connected!");

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();
                ConnectionModel connectingClient = new ConnectionModel(client, stream);
                connectingClient.isConnected = false;
                connectedClients.Add(connectingClient);
                int index = connectedClients.Count - 1;
                Task processConnectionTask = new Task(() => readConnectionRequest(index));
                processConnectionTask.Start();
            }
        }

        public async Task<bool> readConnectionRequest(int connectionId)
        {
            if (connectedClients[connectionId].isConnected)
            {
                throw new Exception("The Connection is already connected!");
            }

            Console.WriteLine("Processing connection request...");
            String connectionData = null;
            string message;

            // Loop to receive all the data sent by the client. 
            while ((message = connectedClients[connectionId].ReaderStream.ReadLine()) != null )
            {
                Console.WriteLine("Received: {0}", message);

                string[] splitData = message.Split('~');
                connectedClients[connectionId].changeUserName(splitData[0]);
                connectedClients[connectionId].changeIPAddress(splitData[1]);

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);

                // Send back a response.
                connectedClients[connectionId].WriterStream.Write(splitData);
                Console.WriteLine("User: {0}", splitData[0] + " connected through IP: " + splitData[1]);
                return true;
            }
            return false;
        }
    }
}
