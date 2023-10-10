using Unity.Netcode;

public struct GameData : INetworkSerializable
{
    public PlayerData player1;
    public PlayerData player2;
    public ulong currentPlayer;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        player1.NetworkSerialize(serializer);
        player2.NetworkSerialize(serializer);       
        serializer.SerializeValue(ref currentPlayer);
    }
}
