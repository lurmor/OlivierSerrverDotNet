using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Data;
using System.Text.Json.Serialization;

namespace OlivierSerrverDotNet;

public enum UnitType { SOLO, DUO, MATRIX, HUBPLUS };
public enum ModuleType { NAM, DAC, ADC, BT, BP };

[Serializable]


// public abstract class OlivierUnit
public class OlivierUnit
{
    [JsonIgnore]
    public bool conected = false;
    public readonly uint SN;
    [JsonIgnore]
    public EndPoint? remoteEnd;
    public readonly UnitType unitType;
    [JsonIgnore]
    public NetworkStream? tcpStream;
    [JsonIgnore]
    // List<Transfer> transfers = new List<Transfer>();
    //public readonly Type type

    public List<ModuleType> Modules = new List<ModuleType>();
    int maxModules = 0;

    public OlivierUnit(uint _SN, NetworkStream _tcpStream, UnitType _unitType)
    {
        SN = _SN;
        tcpStream = _tcpStream;
        remoteEnd = tcpStream.Socket.RemoteEndPoint;
        unitType = _unitType;
        conected = true;
        if (unitType == UnitType.SOLO) maxModules = 1;
        if (unitType == UnitType.DUO) maxModules = 2;
        if (unitType == UnitType.MATRIX) maxModules = 2;
        if (unitType == UnitType.HUBPLUS) maxModules = 4;
    }
    [JsonConstructor]
    public OlivierUnit(uint SN, UnitType unitType)
    {
        this.SN = SN;
        tcpStream = null;
        remoteEnd = null;
        this.unitType = unitType;

    }
    int setModules(params ModuleType[] modules)
    {
        if (modules.Length != maxModules)
        {
            return -1;
        }
        Modules.Clear();
        Modules = modules.ToList<ModuleType>();
        return Modules.Count;
    }



    // public static OlivierUnit? CreateOlivierUnit(uint _SN, NetworkStream _tcpStream, UnitType _unitType)
    // {
    //     switch (_unitType)
    //     {
    //         case UnitType.ONLY_SOURSE:
    //             return new OlivierUnitSourse(_SN, _tcpStream);
    //         case UnitType.ONLY_EXIT:
    //             return new OlivierUnitExit(_SN, _tcpStream);
    //         case UnitType.BYPASS:
    //             return new OlivierUnitBybass(_SN, _tcpStream);
    //         default:
    //             return null;
    //     }

    // }

    public bool SendMessage(string message)
    {
        if (tcpStream == null) return false;
        try
        {
            TcpConector.SendMessage(tcpStream, message);
        }
        catch
        {
            Console.WriteLine("ERROR send" + tcpStream + message);
            return false;
        }
        return true;
    }


}


public class Transfer
{
    public readonly OlivierUnit from;
    public readonly OlivierUnit to;
    int Rchanel, Lchanel;

    public Transfer(OlivierUnit from, OlivierUnit to, int Lchanel, int Rchanel)
    {
        this.Rchanel = Rchanel;
        this.Lchanel = Lchanel;
        this.from = from;
        this.to = to;
        //from.addTransfer(this);

    }
    int getChCount()
    {
        int chCount = 0;
        if (Rchanel != 0) chCount++;
        if (Lchanel != 0) chCount++;
        return chCount;
    }

}
