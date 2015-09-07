using System;
using System.Collections.Generic;
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
        private static TaskFactory taskHandler;

        public ServerListener()
        {
            //Establishes a local endpoint by getting the localhost IP using the AddressService
            IPEndPoint serverEndPoint = new IPEndPoint(getIP("localhost").Result, 6789);
            listener = new TcpListener(serverEndPoint);
            connectedClients = new List<ConnectionModel>();
            taskHandler = new TaskFactory();
        }

        public void receiveMessage(TcpListener inpListener)
        {
            // Start listening for client requests.
            inpListener.Start();


            // Enter the listening loop. 
            while (true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests. 
                // You could also user server.AcceptSocket() here.
                TcpClient client = inpListener.AcceptTcpClient();
                Console.WriteLine("Connected!");

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();
                ConnectionModel connectingClient = new ConnectionModel(client, stream);
                connectedClients.Add(connectingClient);
                Action<object> readAction = (object obj) =>
                {
                    readMessage(connectingClient);
                    Console.WriteLine("Task={0}, obj={1}, Thread={2}",
                    Task.CurrentId, obj,
                    Thread.CurrentThread.ManagedThreadId);
                };
                Console.WriteLine("Created action " + readAction.ToString());
                taskHandler.StartNew(readAction,connectedClients.Count);
            }
        }



        public void readMessage(ConnectionModel inpConnection)
        {
            Console.WriteLine("Waiting for message...");
            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;
            int i;

            // Loop to receive all the data sent by the client. 
            while ((i = inpConnection.ListenerStream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine("Received: {0}", data);

                // Process the data sent by the client.
                data = data.ToUpper();

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                if (data[0] == '/')
                {
                    switch (data[1])
                    {
                        case 'a':
                            string additionInput = data.Remove(3);
                            string[] numbers = additionInput.Split('.');
                            msg = System.Text.Encoding.ASCII.GetBytes(numbers[0]);
                            break;
                        default:
                            break;
                    }
                }

                // Send back a response.
                inpConnection.ListenerStream.Write(msg, 0, msg.Length);
                Console.WriteLine("Sent: {0}", data);
            }
        }

        static public void stopListening()
        {

        }
    }
}
