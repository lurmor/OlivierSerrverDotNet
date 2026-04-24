using System;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Data;

namespace OlivierSerrverDotNet;

public class NTP
{
    private UdpClient udp;
    private IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

    public NTP(int port)
    {
        udp = new UdpClient(port);
        Console.WriteLine("Advanced NTP server started on port 123...\n");
    }

    public void Update()
    {
        if (udp.Available > 0)
        {
            byte[] request = udp.Receive(ref remoteEP);
            Console.WriteLine($"[{DateTime.Now}] Request from {remoteEP.Address} Request {request.ToString()}");
            if (request[0] == 'T')
            {
                long ms = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                byte[] resp = BitConverter.GetBytes(ms);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(resp);

                udp.Send(resp, resp.Length, remoteEP);
            }
        }

    }



}
