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
    public class ServerListener : AddressService
    {
        public TcpListener listener;
        public List<ConnectionModel> connectedClients;

        public ServerListener()
        {
            Console.WriteLine("Starting server at: 10.200.128.141:6789");
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("10.200.128.141"), 6789);
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

        public async Task listenForConnectionAsync()
        {
            while (true)
            {
                Console.Write("Waiting for a connection... \n");

                // Perform a blocking call to accept requests. 
                // You could also user server.AcceptSocket() here.
                TcpClient client = await listener.AcceptTcpClientAsync();
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
        public ConnectionModel listenForConnection()
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
                    return connectingClient;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }

        public bool processConnectionRequest(ConnectionModel client)
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
                client.changeIPAddress(splitData[0]);
                client.changeUserName(splitData[1]);

                // Send back a response.
                client.WriterStream.Write(splitData);
                client.isConnected = true;
                Console.WriteLine("User: {0}", splitData[0] + " connected through IP: " + splitData[1] + "\n");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task processConnectionRequestAsync(ConnectionModel client)
        {
            if (client.isConnected)
            {
                throw new Exception("The Connection is already connected!\n");
            }

            Console.WriteLine("Processing connection request...\n");
            try
            {
                string message = await client.ReaderStream.ReadLineAsync();
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
