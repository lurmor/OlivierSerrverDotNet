using System;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OlivierSerrverDotNet;

public class TcpConector
{
    private TcpListener _tcpListener;
    private List<TcpClient> tcpClients = new List<TcpClient>();

    public TcpConector(int Port)
    {
        _tcpListener = new TcpListener(IPAddress.Any, Port);
        _tcpListener.Start();
    }
    public void Update()
    {
        AcceptClients();
        foreach (TcpClient client in tcpClients)
        {
            if (client.Connected)
            {
                NetworkStream stream = new NetworkStream(client.Client, ownsSocket: false);
                using (stream)
                {
                    try
                    {
                        string? message = ReceiveMessage(stream);
                        if (message != null)
                        {
                            Console.WriteLine("Received: " + message);

                            // Обработка сообщения и отправка ответа клиенту
                            string response = ProcessMessage(stream, message);
                            ///SendMessage(stream, response);
                            //Console.WriteLine("Response: " + response);

                        }
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e);
                    }
                }
            }
        }
    }

    private void AcceptClients()
    {
        if (_tcpListener.Pending())
        {
            TcpClient client = _tcpListener.AcceptTcpClient();
            tcpClients.Add(client);
            Console.WriteLine("Client connected!");
        }
    }

    private string? ReceiveMessage(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];
        if (stream.DataAvailable)
        {

            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead <= 0)
                return null;


            return Encoding.UTF8.GetString(buffer);
        }
        else return null;
    }

    private string ProcessMessage(NetworkStream client, string message)
    {
        string res = "pupupu";
        if (message[0] == 'R') UnitColector.getInstance().conectTCPcli(client, message);

        return res;
    }

    public static void SendMessage(NetworkStream stream, string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        byte[] buffer = new byte[messageBytes.Length + 2];
        buffer[1] = (byte)messageBytes.Length;
        Array.Copy(messageBytes, 0, buffer, 2, messageBytes.Length);

        stream.Write(buffer, 0, buffer.Length);
    }

}
