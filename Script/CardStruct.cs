using Unity.Netcode;
using UnityEngine;

public enum Cardtype
{
    Gwang,
    Normal,
    Special
}

public struct CardStruct : INetworkSerializable
{
    public int month;
    public Cardtype cardtype;
    public int img_num;

    public CardStruct(int num, Cardtype type, int imnum)
    {
        month = num;
        cardtype = type;   
        img_num = imnum;
    }


    void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        serializer.SerializeValue(ref month);
        serializer.SerializeValue(ref cardtype);
        serializer.SerializeValue(ref img_num);
        //throw new System.NotImplementedException();
    }
}
