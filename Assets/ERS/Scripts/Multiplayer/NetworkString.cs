using System;
using Unity.Collections;
using Unity.Netcode;

[Serializable]
public struct NetworkString : INetworkSerializable
{
    private FixedString32Bytes str;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref str);
    }

    public override string ToString()
    {
        return str.ToString();
    }

    public static implicit operator string(NetworkString s) => s.ToString();
    public static implicit operator NetworkString(string s) => new NetworkString() { str = new FixedString32Bytes(s) };
}


[Serializable]
public struct NetworkString64 : INetworkSerializable
{
    private FixedString64Bytes str;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref str);
    }

    public override string ToString()
    {
        return str.ToString();
    }

    public static implicit operator string(NetworkString64 s) => s.ToString();
    public static implicit operator NetworkString64(string s) => new NetworkString64() { str = new FixedString64Bytes(s) };
}
