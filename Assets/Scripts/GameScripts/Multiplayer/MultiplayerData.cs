using Unity.Collections;
using Unity.Netcode;

public struct MultiplayerData : INetworkSerializable, System.IEquatable<MultiplayerData>
{
    public ulong clientId;
    public int colorId;
    public int playerType;//  AI=0,LocalPlayer=1,OpponentPlayer=2
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;

    public bool Equals(MultiplayerData other)
    {
        return
            clientId == other.clientId &&
            colorId == other.colorId &&
            playerName == other.playerName &&
            playerId == other.playerId &&
            playerType == other.playerType;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref playerType);
    }

}
