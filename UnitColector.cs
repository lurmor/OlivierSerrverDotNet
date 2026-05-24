using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Linq;

namespace OlivierSerrverDotNet;

public class UnitColector
{
    private static UnitColector? instance;
    private List<OlivierUnit> olivierUnits = new List<OlivierUnit>();
    private List<Transfer> transfers = new List<Transfer>();

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
        string json = ToJsonUnit();
        File.WriteAllText(fileName, json);
    }

    public string ToJsonUnit()
    {

        string json = JsonSerializer.Serialize(
            olivierUnits,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            });
        return json;
    }
    public string ToJsonTrans()
    {
        var lightTransfers = transfers.Select(t => new
        {
            from = new { SN = t.from.SN },
            to = new { SN = t.to.SN }
        });
        string json = JsonSerializer.Serialize(
            lightTransfers,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            });
        return json;
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
    public Transfer? UnitConect(OlivierUnit from, OlivierUnit to, int Lchanel = 1, int Rchanel = 2)
    {
        foreach (Transfer btransfer in transfers)
        {
            if (btransfer.from == from && btransfer.to == to)
                return btransfer;
        }

        Transfer transfer = new Transfer(from, to, Lchanel, Rchanel);
        bool isOk = false;
        if (transfer.to.remoteEnd != null)
        {
            isOk = true;
            string data = "DT" + transfer.to.remoteEnd.ToString().Split(':')[0];
            isOk &= transfer.from.SendMessage(data);
            data = "DR";
            isOk &= transfer.to.SendMessage(data);
            transfers.Add(transfer);
        }
        if (!isOk)
        {
            Console.Error.WriteLine("Falled add transfer");
            return null;
        }
        return transfer;

    }
    public bool UnitDisConect(Transfer transfer)
    {
        bool isOk = true;
        string data = "DS";
        isOk &= transfer.from.SendMessage(data);
        isOk &= transfer.to.SendMessage(data);
        transfers.Remove(transfer);
        return isOk;
    }

    public bool UnitDisConect(OlivierUnit from, OlivierUnit to)
    {
        foreach (Transfer transfer in transfers)
        {
            if (transfer.from == from && transfer.to == to)
                return UnitDisConect(transfer);
        }
        return false;
    }

    public string MassageToUnit(NetworkStream tcpStr, string Data)
    {

        var unit = GetUnit(tcpStr);
        Console.WriteLine("   " + unit.SN.ToString() + Data);
        return "";
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
    public OlivierUnit? GetUnit(NetworkStream tcpStr)
    {
        foreach (var unit in olivierUnits)
        {
            if (unit.tcpStream == tcpStr)
            {
                return unit;
            }
        }
        return null;
    }
}
