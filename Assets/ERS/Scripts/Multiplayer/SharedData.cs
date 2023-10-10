using System;

public class SharedData
{
    private static SharedData _instance;

    public RelayHostData relayHostData;
    public RelayJoinData relayJoinData;

    public static SharedData Singelton
    {
        get
        {
            if (_instance == null)
            {
                _instance = new();
            }
            return _instance;
        }
    }
    private SharedData() { }
}

public struct RelayHostData
{
    public string JoinCode;
    public string IPv4Address;
    public ushort Port;
    public Guid AllocationID;
    public byte[] AllocationIDBytes;
    public byte[] ConnectionData;
    public byte[] Key;
}

public struct RelayJoinData
{
    public string IPv4Address;
    public ushort Port;
    public Guid AllocationID;
    public byte[] AllocationIDBytes;
    public byte[] ConnectionData;
    public byte[] HostConnectionData;
    public byte[] Key;
}