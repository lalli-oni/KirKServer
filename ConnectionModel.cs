﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KirkServer
{
    class ConnectionModel
    {
        private TcpClient _listener;
        private NetworkStream _stream;
        private string _userName;
        private IPAddress _clientipAddress;
        private bool _isConnected;
        private StreamWriter _writerStream;
        private StreamReader _readerStream;
        private Task _sendMsgTask;

        public TcpClient Listener
        {
            get { return _listener; }
            private set { _listener = value; }
        }

        public NetworkStream Stream
        {
            get { return _stream; }
            private set { _stream = value; }
        }

        public StreamWriter WriterStream
        {
            get { return _writerStream; }
            set { _writerStream = value; }
        }

        public StreamReader ReaderStream
        {
            get { return _readerStream; }
            set { _readerStream = value; }
        }

        public string UserName
        {
            get { return _userName; }
            private set { _userName = value; }
        }

        public IPAddress ClientIpAddress
        {
            get { return _clientipAddress; }
            set { _clientipAddress = value; }
        }

        public bool isConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; }
        }

        /// <summary>
        /// Creates a new connection object to hold the connection from the client.
        /// </summary>
        /// <param name="inpListener">The TcpListener object that the client connected to</param>
        /// <param name="inpStream">The NetworkStream connecting the client to the server</param>
        /// <param name="inpIPaddress">The IP address of the client</param>
        public ConnectionModel(TcpClient inpListener, NetworkStream inpStream)
        {
            isConnected = false;
            Listener = inpListener;
            Stream = inpStream;
            ReaderStream = new StreamReader(Stream);
            WriterStream = new StreamWriter(Stream) { AutoFlush = true};
        }

        /// <summary>
        /// Creates a new connection object to hold the connection from the client.
        /// </summary>
        /// <param name="inpListener">The TcpListener object that the client connected to</param>
        /// <param name="inpStream">The NetworkStream connecting the client to the server</param>
        /// <param name="inpIPaddress">The IP address of the client</param>
        public ConnectionModel(TcpClient inpListener, NetworkStream inpStream, IPAddress inpIPaddress)
        {
            isConnected = false;
            Listener = inpListener;
            Stream = inpStream;
            ClientIpAddress = inpIPaddress;
            ReaderStream = new StreamReader(Stream);
        }

        /// <summary>
        /// Creates a new connection object to hold the connection from the client.
        /// </summary>
        /// <param name="inpListener">The TcpListener object that the client connected to</param>
        /// <param name="inpStream">The NetworkStream connecting the client to the server</param>
        /// <param name="inpUserName">The username associated with the client</param>
        /// <param name="inpIPaddress">The IP address of the client</param>
        public ConnectionModel(TcpClient inpListener, NetworkStream inpStream, string inpUserName, IPAddress inpIPaddress)
        {
            isConnected = false;
            Listener = inpListener;
            Stream = inpStream;
            UserName = inpUserName;
            ClientIpAddress = inpIPaddress;
            ReaderStream = new StreamReader(Stream);
        }

        public void changeUserName(string inpUserName)
        {
            UserName = inpUserName;
        }

        public void changeIPAddress(string inpIPaddress)
        {
            if (ClientIpAddress == null)
            {
                ClientIpAddress = IPAddress.Parse(inpIPaddress);
            }
            else
            {
                throw new Exception("IP address is already set, changing IP addressess not supported!");
            }
        }

        public async Task sendMessage(string broadcastingMessage)
        {
            Console.WriteLine("Broadcasting message from " + this.UserName + ": " + broadcastingMessage);
            await this.WriterStream.WriteLineAsync(this.UserName + ": " + broadcastingMessage);
            await WriterStream.FlushAsync();
        }
        public async Task<string> receiveMessage()
        {
            string message = null;
            Console.WriteLine("Receiving message from " + this.UserName);
            message = ReaderStream.ReadToEndAsync().Result;
            await WriterStream.FlushAsync();
            Console.WriteLine("Message Received.");
            return message;
        }
    }
}
