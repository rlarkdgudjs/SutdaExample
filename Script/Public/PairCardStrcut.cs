using System;
using UnityEngine;

public struct PairCardStruct
{
    public (int month, Cardtype type) cardA;
    public (int month, Cardtype type) cardB;

    public PairCardStruct((int, Cardtype) c1, (int, Cardtype) c2)
    {
        if (CompareCards(c1, c2) <= 0)
        {
            cardA = c1;
            cardB = c2;
        }
        else
        {
            cardA = c2;
            cardB = c1;
        }
    }

    private static int CompareCards((int, Cardtype) a, (int, Cardtype) b)
    {
        int monthCompare = a.Item1.CompareTo(b.Item1);
        if (monthCompare != 0) return monthCompare;
        return a.Item2.CompareTo(b.Item2);
    }

    public override bool Equals(object obj)
    {
        if (obj is PairCardStruct other)
        {
            return cardA.Equals(other.cardA) && cardB.Equals(other.cardB);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(cardA, cardB);
    }
}
