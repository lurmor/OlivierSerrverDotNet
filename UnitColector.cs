using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace OlivierSerrverDotNet;

public class UnitColector
{
    private static UnitColector? instance;
    private List<OlivierUnit> olivierUnits = new List<OlivierUnit>();

    private UnitColector()
    { }

    public static UnitColector getInstance()
    {
        if (instance == null)
            instance = new UnitColector();
        return instance;
    }



    public UnitColector(string fileName)
    {
        LoadFomFile(fileName);
    }
    public int LoadFomFile(string fileName)
    {
        var LoadedUnits = JsonSerializer.Deserialize<List<OlivierUnit>>(
            File.ReadAllText(fileName),
            new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            }
        );
        if (LoadedUnits != null)
            olivierUnits = LoadedUnits;
        return olivierUnits.Count;
    }

    public void SaveToFile(string fileName)
    {
        string json = JsonSerializer.Serialize(
            olivierUnits,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            });

        File.WriteAllText(fileName, json);
    }

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
                TcpConector.SendMessage(tcpStr, "unit ReConect Sucses");
                Console.WriteLine("unit ReConect Sucses");


                return;
            }
        }
        var newunit = new OlivierUnit(uint.Parse(splitedData[1]), tcpStr,
                                     (UnitType)uint.Parse(splitedData[2]));
        if (newunit != null)
        {
            olivierUnits.Add(newunit);
            TcpConector.SendMessage(tcpStr, "unit Conect Sucses");
            Console.WriteLine("unit Conect Sucses");
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
