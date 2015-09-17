using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        public ConcurrentBag<ConnectionModel> connectedClients;
        public int nrOfClients;

        public ServerListener()
        {
            Console.WriteLine("Starting server at: 10.200.128.171:6789");
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("10.200.128.171"), 6789);
            nrOfClients = 0;
            listener = new TcpListener(serverEndPoint);
            connectedClients = new ConcurrentBag<ConnectionModel>();
            listener.Start(100);
        }

        ~ServerListener()
        {
            Console.WriteLine("Listener is dying!");
        }

        public void receiveMessage(ConnectionModel client)
        {
            string message = client.ReaderStream.ReadLine();
            if (message != null)
            {
                broadcastMessage(message);
            }
        }

        public async Task<string> receiveMessageAsync(ConnectionModel client)
        {
            string message = await client.ReaderStream.ReadLineAsync();
            if (message != null)
            {
                return message;
            }
            Console.WriteLine("Received empty message from " + client.UserName);
            return null;
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
