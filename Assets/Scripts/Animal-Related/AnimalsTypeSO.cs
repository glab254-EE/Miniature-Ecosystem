using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalSO",menuName = "Create New Animal", order = -2)] [System.Serializable]
public class AnimalsSO : ScriptableObject
{
    [field:Header("Animal visualization set-up.")]
    [field:SerializeField] public BeingType BType {get; internal set;}= BeingType.Others; // aka not moving or moving
    [field:SerializeField] public Vector2 MinMovementBounds{get; internal set;}
    [field:SerializeField] public Vector2 MaxMovementBounds{get; internal set;}
    [field:SerializeField] public float MovementSpeed {get; internal set;} = 1; // aka not moving or moving
    [field:SerializeField] public float TurnDuration{get; internal set;} = 0.5f;
    [field:SerializeField] public float MovementDelay {get; internal set;}= 2;
    [field:SerializeField] public bool KeepVertical {get; internal set;} // reference animator for animations    
    [field:SerializeField] public Sprite BaseSprite {get; internal set;} = null; // aka not moving or moving
    [field:Header("Animal Gaining Set-up")]
    [field:SerializeField] public float GainDelay{get; internal set;} = 1f;
    [field:SerializeField] public float GainAmmount{get; internal set;} = 1f;
    [field:SerializeField] public int ResourceID{get; internal set;} = 0;
    [field:Header("ETC")]
    [field:SerializeField] public string animalName{get;internal set;} = "";
    [field:SerializeField] public float AnimalResourceCost{get;internal set;} = 45;
    [field:SerializeField] public int AnimalResourceSubstractID{get;internal set;} = 0;
    [field:SerializeField] public float MinAnimalSize{get; internal set;} = 0.9f;
    [field:SerializeField] public float MaxAnimalSize{get; internal set;} = 1.1f;
}
