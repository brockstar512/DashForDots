using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
public struct PlayerTurn : INetworkSerializable, System.IEquatable<PlayerTurn>
{
    public int turn;
    public Vector2 selectedDot;
    public Vector3 neighborDot;

    public bool Equals(PlayerTurn other)
    {
        return
            turn == other.turn &&
            selectedDot == other.selectedDot &&
            neighborDot == other.neighborDot;

    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref turn);
        serializer.SerializeValue(ref selectedDot);
        serializer.SerializeValue(ref neighborDot);
    }
}
