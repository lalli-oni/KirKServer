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
        public ConcurrentDictionary<ConnectionModel,string> connectedClients;
        public int nrOfClients;
        private List<string> userNames;

        public ServerListener()
        {
            Console.WriteLine("Starting server at: 10.200.128.171:6789");
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("10.200.128.171"), 6789);
            nrOfClients = 0;
            userNames = new List<string>();
            listener = new TcpListener(serverEndPoint);
            connectedClients = new ConcurrentDictionary<ConnectionModel, string>();
            listener.Start(100);
        }

        ~ServerListener()
        {
            Console.WriteLine("Listener is dying!");
        }

        public void ReceiveMessage(ConnectionModel client)
        {
            string message = client.ReaderStream.ReadLine();
            if (message != null)
            {
                BroadcastMessage(message);
            }
        }

        public async Task<string> ReceiveMessageAsync(ConnectionModel client)
        {
            string message = await client.ReaderStream.ReadLineAsync();
            if (message != null)
            {
                return message;
            }
            Console.WriteLine("Received empty message from " + client.UserName);
            return null;
        }

        public void BroadcastMessage(string broadcastingMessage)
        {
            Console.WriteLine("Broadcasting message: " + broadcastingMessage);
            Parallel.ForEach(connectedClients, connections => connections.Key.SendMessage(broadcastingMessage));
        }

        public async Task ListenForConnectionAsync()
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
                    int i = 0;
                    while (!connectedClients.TryAdd(connectingClient, connectingClient.UserName) || i < 1000)
                    {
                        i++;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }
        public ConnectionModel ListenForConnection()
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

        public bool ProcessConnectionRequest(ConnectionModel client)
        {
            if (client.isConnected)
            {
                throw new Exception("The Connection is already connected!\n");
            }

            Console.WriteLine("Processing connection request...\n");
            try
            {
                string ipMessage = client.ReaderStream.ReadLine();
                Console.WriteLine("Processing connection from: {0}", ipMessage);
                string usernameMessage = client.ReaderStream.ReadLine();
                Console.WriteLine("Processing connection from: {0}", usernameMessage);
                if (!userNames.Contains(usernameMessage))
                {
                    client.ChangeUserName(usernameMessage);
                    client.ChangeIpAddress(ipMessage);
                    // Send back a response.
                    client.WriterStream.Write("Server~Connection successfull!\nYou are connected as {0}", client.UserName);
                    client.isConnected = true;
                    Console.WriteLine("User: {0} connected through IP: {1}", client.UserName, client.ClientIpAddress);
                }
                else
                {
                    client.WriterStream.Write("Server~Error: Username is already in use. Please reconnect using a different username.\n");
                    client.isConnected = true;
                    Console.WriteLine("{1} tried to connect with duplicate username: {0} refusing connection... \n", ipMessage, usernameMessage);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return false;
        }

        public async Task ProcessConnectionRequestAsync(ConnectionModel client)
        {
            if (client.isConnected)
            {
                throw new Exception("The Connection is already connected!\n");
            }

            Console.WriteLine("Processing connection request...\n");
            try
            {
                string ipMessage = await client.ReaderStream.ReadLineAsync();
                Console.WriteLine("Processing connection from: {0}", ipMessage);
                string usernameMessage = await client.ReaderStream.ReadLineAsync();
                Console.WriteLine("Processing connection from: {0}", usernameMessage);
                if (!userNames.Contains(usernameMessage))
                {
                    client.ChangeUserName(usernameMessage);
                    client.ChangeIpAddress(ipMessage);
                    // Send back a response.
                    client.WriterStream.Write("Connection successfull!\nYou are connected as {0}", client.UserName);
                    client.isConnected = true;
                    Console.WriteLine("User: {0} connected through IP: {1}", client.UserName, client.ClientIpAddress);
                }
                else
                {
                    client.WriterStream.Write("Error: Username is already in use. Please reconnect using a different username.\n");
                    client.isConnected = true;
                    Console.WriteLine("{1} tried to connect with duplicate username: {0} refusing connection... \n", ipMessage, usernameMessage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
