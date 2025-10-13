using System;
using System.Net;
using System.Net.Sockets;

namespace OlivierSerrverDotNet;

public class TcpConector
{
    private TcpListener _tcpListener;
    public int Port = 34502;
    public TcpConector()
    {
        _tcpListener = new TcpListener(IPAddress.Any, Port);
        _tcpListener.Start();
    }

}
