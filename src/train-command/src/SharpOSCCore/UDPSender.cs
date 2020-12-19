using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SharpOSCCore
{
    public class UDPSender
    {
        public int Port
        {
            get { return _port; }
        }
        int _port;

        public string Address
        {
            get { return _address; }
        }
        string _address;

        IPEndPoint _remoteIpEndPoint;
        Socket _socket;

        public UDPSender(string address, int port)
        {
            _port = port;
            _address = address;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            var addresses = Dns.GetHostAddresses(address);
            if (addresses.Length == 0)
            {
                throw new Exception("Unable to find IP address for " + address);
            }

            _remoteIpEndPoint = new IPEndPoint(addresses[0], port);
        }

        public void Send(byte[] message)
        {
            _socket.SendTo(message, _remoteIpEndPoint);
        }

        public void Send(OscPacket packet)
        {
            var data = packet.GetBytes();
            Send(data);
        }

        public void Close()
        {
            _socket.Close();
        }
    }
}
