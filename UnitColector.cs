using System;
using System.Net.Sockets;

namespace OlivierSerrverDotNet;

public class UnitColector
{
    private static UnitColector? instance;

    private UnitColector()
    { }

    public static UnitColector getInstance()
    {
        if (instance == null)
            instance = new UnitColector();
        return instance;
    }

    private List<OlivierUnit> olivierUnits = new List<OlivierUnit>();


    public UnitColector(string fileName)
    {
        LoadFomFile(fileName);
    }
    private int LoadFomFile(string fileName) { return 0; }

    public void conectTCPcli(NetworkStream tcpcli, string Data)
    {
        var splitedData = Data.Split('-');
        uint SN = uint.Parse(splitedData[1]);
        foreach (var unit in olivierUnits)
        {

            if (unit.SN == SN)
            {
                unit.tcpClient = tcpcli;
                unit.remoteEnd = tcpcli.Socket.RemoteEndPoint;
                unit.conected = true;
                TcpConector.SendMessage(tcpcli, "Conect Sucses");

                return;
            }
        }
        var newunit = OlivierUnit.CreateOlivierUnit(uint.Parse(splitedData[1]), tcpcli,
                                     (UnitType)uint.Parse(splitedData[2]));
        if (newunit != null)
        {
            olivierUnits.Add(newunit);
            TcpConector.SendMessage(tcpcli, "Conect Sucses");
        }
    }

    public void disconectTCPcli(NetworkStream tcpcli)
    {
        foreach (var unit in olivierUnits)
        {
            if (unit.tcpClient == tcpcli)
            {
                unit.tcpClient = null;
                unit.remoteEnd = null;
                unit.conected = false;
                return;
            }
        }
    }
}
