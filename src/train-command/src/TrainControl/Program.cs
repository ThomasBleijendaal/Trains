using System;
using System.Net;
using SharpOSCCore;

namespace TrainControl
{
    class Program
    {
        static UDPSender _sender = new UDPSender("192.168.2.255", 15670);

        static void Main(string[] args)
        {
            do
            {
                if (int.TryParse(Console.ReadLine(), out var integer))
                {
                    var message = new OscMessage("/train/1", new IPEndPoint(IPAddress.Parse("192.168.2.10"), 15670), integer);

                    _sender.Send(message);
                }
            }
            while (true);
        }
    }
}
