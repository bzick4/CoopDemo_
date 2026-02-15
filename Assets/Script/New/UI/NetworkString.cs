using Unity.Netcode;
using Unity.Collections;

public struct NetworkString : INetworkSerializable, System.IEquatable<NetworkString>
{
    private FixedString128Bytes _value;

    public string Value
    {
        get => _value.ToString();
        set => _value = value;
    }

    public NetworkString(string value)
    {
        _value = value;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _value);
    }

    public bool Equals(NetworkString other) => _value == other._value;
    public override bool Equals(object obj) => obj is NetworkString other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value.ToString();

    public static implicit operator string(NetworkString ns) => ns.Value;
    public static implicit operator NetworkString(string s) => new NetworkString(s);
}