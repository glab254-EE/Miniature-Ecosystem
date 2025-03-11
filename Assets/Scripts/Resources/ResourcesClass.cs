using System;
using UnityEngine;

[System.Serializable]
public class Resource
{
    [SerializeField] internal int ResourceNameID = 0;
    [SerializeField] internal int ReferencedSprite = -1;
    [SerializeField,HideInInspector] internal double Current = 0; // blin, i can't save double
    [SerializeField,HideInInspector] internal double Gain = 0;
    internal Resource(int startingnameid)
    {
        ResourceNameID = startingnameid;
    }
}
