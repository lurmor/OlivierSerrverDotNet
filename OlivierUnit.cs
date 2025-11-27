using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Data;

namespace OlivierSerrverDotNet;

public enum UnitType { ONLY_SOURSE, ONLY_EXIT, BYPASS };


public abstract class OlivierUnit
{
    public bool conected = false;
    public uint SN;
    public EndPoint? remoteEnd;
    public readonly UnitType unitType;
    public NetworkStream? tcpClient;
    //public readonly Type type;

    public OlivierUnit(uint _SN, NetworkStream _tcpClient, UnitType _unitType)
    {
        SN = _SN;
        tcpClient = _tcpClient;
        remoteEnd = tcpClient.Socket.RemoteEndPoint;
        unitType = _unitType;
        conected = true;
    }
    public OlivierUnit(uint _SN, UnitType _unitType)
    {
        SN = _SN;
        tcpClient = null;
        remoteEnd = null;
        unitType = _unitType;

    }


    public static OlivierUnit? CreateOlivierUnit(uint _SN, NetworkStream _tcpClient, UnitType _unitType)
    {
        switch (_unitType)
        {
            case UnitType.ONLY_SOURSE:
                return new OlivierUnitSourse(_SN, _tcpClient);
            case UnitType.ONLY_EXIT:
                return new OlivierUnitExit(_SN, _tcpClient);
            case UnitType.BYPASS:
                return new OlivierUnitBybass(_SN, _tcpClient);
            default:
                return null;
        }

    }

    public bool SendMessage(string message)
    {
        if (tcpClient == null) return false;
        try
        {
            TcpConector.SendMessage(tcpClient, message);
        }
        catch
        {
            return false;
        }
        return true;
    }
}

public class OlivierUnitSourse : OlivierUnit
{
    public OlivierUnitSourse(uint _SN, NetworkStream _tcpClient) : base(_SN, _tcpClient, UnitType.ONLY_SOURSE) { }
}

public class OlivierUnitExit : OlivierUnit
{
    public OlivierUnitExit(uint _SN, NetworkStream _tcpClient) : base(_SN, _tcpClient, UnitType.ONLY_EXIT) { }
}

public class OlivierUnitBybass : OlivierUnit
{
    public OlivierUnitBybass(uint _SN, NetworkStream _tcpClient) : base(_SN, _tcpClient, UnitType.BYPASS) { }
}