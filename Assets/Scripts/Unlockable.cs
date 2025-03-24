using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlockable 
{
    [SerializeField] internal List<Unlockable> next;
    [SerializeField] internal string id;
}
