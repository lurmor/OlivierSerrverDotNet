using System;
using System.Net;
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

    public void conectTCPcli(NetworkStream tcpStr, string Data)
    {
        var splitedData = Data.Split('-');
        uint SN = uint.Parse(splitedData[1]);
        foreach (var unit in olivierUnits)
        {

            if (unit.SN == SN)
            {
                unit.tcpStream = tcpStr;
                unit.remoteEnd = tcpStr.Socket.RemoteEndPoint;
                unit.conected = true;
                TcpConector.SendMessage(tcpStr, "Conect Sucses");

                return;
            }
        }
        var newunit = OlivierUnit.CreateOlivierUnit(uint.Parse(splitedData[1]), tcpStr,
                                     (UnitType)uint.Parse(splitedData[2]));
        if (newunit != null)
        {
            olivierUnits.Add(newunit);
            TcpConector.SendMessage(tcpStr, "Conect Sucses");
        }
    }

    public void disconectTCPcli(NetworkStream tcpStr)
    {
        foreach (var unit in olivierUnits)
        {
            if (unit.tcpStream == tcpStr)
            {
                unit.tcpStream = null;
                unit.remoteEnd = null;
                unit.conected = false;
                return;
            }
        }
    }

    public OlivierUnit? GetUnit(uint SN)
    {
        foreach (var unit in olivierUnits)
        {
            if (unit.SN == SN)
            {
                return unit;
            }
        }
        return null;
    }
}
