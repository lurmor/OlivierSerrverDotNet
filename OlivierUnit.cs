using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Data;
using System.Text.Json.Serialization;

namespace OlivierSerrverDotNet;

public enum UnitType { ONLY_SOURSE, ONLY_EXIT, BYPASS };

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
    //public readonly Type type;

    public OlivierUnit(uint _SN, NetworkStream _tcpStream, UnitType _unitType)
    {
        SN = _SN;
        tcpStream = _tcpStream;
        remoteEnd = tcpStream.Socket.RemoteEndPoint;
        unitType = _unitType;
        conected = true;
    }
    [JsonConstructor]
    public OlivierUnit(uint SN, UnitType unitType)
    {
        this.SN = SN;
        tcpStream = null;
        remoteEnd = null;
        this.unitType = unitType;

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
            return false;
        }
        return true;
    }
}

// public class OlivierUnitSourse : OlivierUnit
// {
//     public OlivierUnitSourse(uint _SN, NetworkStream _tcpStream) : base(_SN, _tcpStream, UnitType.ONLY_SOURSE) { }
// }

// public class OlivierUnitExit : OlivierUnit
// {
//     public OlivierUnitExit(uint _SN, NetworkStream _tcpStream) : base(_SN, _tcpStream, UnitType.ONLY_EXIT) { }
// }

// public class OlivierUnitBybass : OlivierUnit
// {
//     public OlivierUnitBybass(uint _SN, NetworkStream _tcpStream) : base(_SN, _tcpStream, UnitType.BYPASS) { }
// }