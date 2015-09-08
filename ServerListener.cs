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
            listener.Start(100);
        }

        public async Task receiveMessage(ConnectionModel client)
        {
            string message = await client.ReaderStream.ReadLineAsync();
            broadcastMessage(message);
        }

        public void broadcastMessage(string broadcastingMessage)
        {
            Console.WriteLine("Broadcasting message: " + broadcastingMessage);
            Parallel.ForEach(connectedClients, listeningClient => listeningClient.sendMessage(broadcastingMessage));
        }

        public async Task listenForConnection()
        {
            while (true)
            {
                Console.Write("Waiting for a connection... \n");

                // Perform a blocking call to accept requests. 
                // You could also user server.AcceptSocket() here.
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Connected!\n");
                try
                {
                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();
                    ConnectionModel connectingClient = new ConnectionModel(client, stream);
                    connectingClient.isConnected = false;
                    connectedClients.Add(connectingClient);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }

        public async Task processConnectionRequest(ConnectionModel client)
        {
            if (client.isConnected)
            {
                throw new Exception("The Connection is already connected!\n");
            }

            Console.WriteLine("Processing connection request...\n");
            try
            {
                string message = client.ReaderStream.ReadLine();
                Console.WriteLine("Received: {0}", message + "\n");

                string[] splitData = message.Split('~');
                client.changeUserName(splitData[0]);
                client.changeIPAddress(splitData[1]);

                // Send back a response.
                client.WriterStream.Write(splitData);
                client.isConnected = true;
                Console.WriteLine("User: {0}", splitData[0] + " connected through IP: " + splitData[1] + "\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
