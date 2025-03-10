using UnityEngine;

[System.Serializable]
public class Resource
{
    [SerializeField] internal string ResourceName = "";
    [SerializeField] internal int ReferencedSprite = -1;
    internal float Current = 0; // blin, i can't save double
    internal float Gain = 0;
    internal Resource(string resourceName)
    {
        this.ResourceName = resourceName;
    }
}
