using System;
using UnityEngine;

[Serializable]
public class Resource
{
    [SerializeField]private string _resName = "";
    [SerializeField]public string ResourceName {
        get{return _resName;}
        private set{_resName = value;}
    }
    [SerializeField]internal Sprite ReferencedSprite = null;
    public float Current {get;internal set;} = 0;
    public float Gain {get;internal set;} = 0;
    internal Resource(string resourceName){
        this._resName = resourceName;
    }
}
