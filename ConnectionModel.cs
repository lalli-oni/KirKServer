using System;
using System.Collections.Generic;
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
        private NetworkStream _listenerStream;
        private string _userName;
        private IPAddress _clientipAddress;

        public TcpClient Listener
        {
            get { return _listener; }
            private set { _listener = value; }
        }

        public NetworkStream ListenerStream
        {
            get { return _listenerStream; }
            private set { _listenerStream = value; }
        }

        public string UserName
        {
            get { return _userName; }
            private set { _userName = value; }
        }

        public IPAddress ClientIpAddress
        {
            get { return _clientipAddress; }
            private set { _clientipAddress = value; }
        }

        /// <summary>
        /// Creates a new connection object to hold the connection from the client.
        /// </summary>
        /// <param name="inpListener">The TcpListener object that the client connected to</param>
        /// <param name="inpStream">The NetworkStream connecting the client to the server</param>
        /// <param name="inpIPaddress">The IP address of the client</param>
        public ConnectionModel(TcpClient inpListener, NetworkStream inpStream)
        {
            Listener = inpListener;
            ListenerStream = inpStream;
        }

        /// <summary>
        /// Creates a new connection object to hold the connection from the client.
        /// </summary>
        /// <param name="inpListener">The TcpListener object that the client connected to</param>
        /// <param name="inpStream">The NetworkStream connecting the client to the server</param>
        /// <param name="inpIPaddress">The IP address of the client</param>
        public ConnectionModel(TcpClient inpListener, NetworkStream inpStream, IPAddress inpIPaddress)
        {
            Listener = inpListener;
            ListenerStream = inpStream;
            ClientIpAddress = inpIPaddress;
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
            Listener = inpListener;
            ListenerStream = inpStream;
            UserName = inpUserName;
            ClientIpAddress = inpIPaddress;
        }
    }
}
