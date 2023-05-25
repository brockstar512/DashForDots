using Unity.Collections;
using Unity.Netcode;

public struct MultiplayerData : INetworkSerializable, System.IEquatable<MultiplayerData>
{
    public bool isHost;
    public ulong clientId;
    public int colorId;
    public int currentIndex;
    public int serverIndex;
    public int playerType;//  AI=0,LocalPlayer=1,OpponentPlayer=2
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;
    public int status;//Denote player is active or not
    public bool isRejoin;
    public int score;
    public int aIMode;
    public bool Equals(MultiplayerData other)
    {
        return
            clientId == other.clientId &&
            colorId == other.colorId &&
            playerName == other.playerName &&
            playerId == other.playerId &&
            playerType == other.playerType &&
            currentIndex == other.currentIndex &&
            serverIndex == other.serverIndex &&
            isHost == other.isHost &&
            status == other.status &&
            isRejoin == other.isRejoin &&
            score == other.score &&
            aIMode == other.aIMode;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref playerType);
        serializer.SerializeValue(ref currentIndex);
        serializer.SerializeValue(ref serverIndex);
        serializer.SerializeValue(ref isHost);
        serializer.SerializeValue(ref status);
        serializer.SerializeValue(ref isRejoin);
        serializer.SerializeValue(ref score);
        serializer.SerializeValue(ref aIMode);
    }

}
