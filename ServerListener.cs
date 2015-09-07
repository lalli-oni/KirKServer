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

        public void receiveMessage(int connectionId)
        {
            Console.WriteLine("Accepting messages... \n");
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

        public void processConnectionRequest(int connectionId)
        {
            if (connectedClients[connectionId].isConnected)
            {
                throw new Exception("The Connection is already connected!\n");
            }

            Console.WriteLine("Processing connection request...\n");
            try
            {
                string message = connectedClients[connectionId].ReaderStream.ReadLine();
                Console.WriteLine("Received: {0}", message + "\n");

                string[] splitData = message.Split('~');
                connectedClients[connectionId].changeUserName(splitData[0]);
                connectedClients[connectionId].changeIPAddress(splitData[1]);

                // Send back a response.
                connectedClients[connectionId].WriterStream.Write(splitData);
                Console.WriteLine("User: {0}", splitData[0] + " connected through IP: " + splitData[1] + "\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
