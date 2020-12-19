using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SharpOSCCore
{
    public delegate void HandleOscPacket(OscPacket packet);
    public delegate void HandleBytePacket(byte[] packet);

    public class UDPListener : IDisposable
    {
        public int Port { get; private set; }

        object _callbackLock;

        UdpClient _receivingUdpClient;
        IPEndPoint _remoteIpEndPoint;

        HandleBytePacket _bytePacketCallback = null;
        HandleOscPacket _oscPacketCallback = null;

        Queue<byte[]> _queue;
        ManualResetEvent _closingEvent;

        public UDPListener(int port)
        {
            Port = port;
            _queue = new Queue<byte[]>();
            _closingEvent = new ManualResetEvent(false);
            _callbackLock = new object();

            // try to open the port 10 times, else fail
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    _receivingUdpClient = new UdpClient(port);
                    break;
                }
                catch (Exception)
                {
                    // Failed in ten tries, throw the exception and give up
                    if (i >= 9)
                    {
                        throw;
                    }

                    Thread.Sleep(5);
                }
            }
            _remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            // setup first async event
            var callBack = new AsyncCallback(ReceiveCallback);
            _receivingUdpClient.BeginReceive(callBack, null);
        }

        public UDPListener(int port, HandleOscPacket callback) : this(port)
        {
            _oscPacketCallback = callback;
        }

        public UDPListener(int port, HandleBytePacket callback) : this(port)
        {
            _bytePacketCallback = callback;
        }

        void ReceiveCallback(IAsyncResult result)
        {
            Monitor.Enter(_callbackLock);
            byte[] bytes = null;

            IPEndPoint remoteEP = null;

            try
            {
                bytes = _receivingUdpClient.EndReceive(result, ref remoteEP);
            }
            catch (ObjectDisposedException e)
            {
                // Ignore if disposed. This happens when closing the listener
            }

            // Process bytes
            if (bytes != null && bytes.Length > 0)
            {
                if (_bytePacketCallback != null)
                {
                    _bytePacketCallback(bytes);
                }
                else if (_oscPacketCallback != null)
                {
                    OscPacket packet = null;
                    try
                    {
                        packet = OscPacket.GetPacket(bytes, remoteEP);
                    }
                    catch (Exception e)
                    {
                        // If there is an error reading the packet, null is sent to the callback
                    }

                    _oscPacketCallback(packet);
                }
                else
                {
                    lock (_queue)
                    {
                        _queue.Enqueue(bytes);
                    }
                }
            }

            if (_closing)
            {
                _closingEvent.Set();
            }
            else
            {
                // Setup next async event
                var callBack = new AsyncCallback(ReceiveCallback);
                _receivingUdpClient.BeginReceive(callBack, remoteEP);
            }
            Monitor.Exit(_callbackLock);
        }

        bool _closing = false;
        public void Close()
        {
            lock (_callbackLock)
            {
                _closingEvent.Reset();
                _closing = true;
                _receivingUdpClient.Close();
            }
            _closingEvent.WaitOne();

        }

        public void Dispose()
        {
            Close();
        }

        public OscPacket Receive()
        {
            if (_closing)
            {
                throw new Exception("UDPListener has been closed.");
            }

            lock (_queue)
            {
                if (_queue.Count() > 0)
                {
                    var bytes = _queue.Dequeue();
                    var packet = OscPacket.GetPacket(bytes, null);
                    return packet;
                }
                else
                {
                    return null;
                }
            }
        }

        public byte[] ReceiveBytes()
        {
            if (_closing)
            {
                throw new Exception("UDPListener has been closed.");
            }

            lock (_queue)
            {
                if (_queue.Count() > 0)
                {
                    var bytes = _queue.Dequeue();
                    return bytes;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
