using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;


public class Public_Data : MonoBehaviour
{
    [SerializeField] private static readonly string saveFileName;
    [SerializeField] private static readonly string encryptionKey;

    public GameData Data {get;private set;}

    // private bool Save(){
    //     bool succseded = false;
    //     try
    //     {
    //         if (Data != null && Data.Money > 0){
    //             string savePath = Application.persistentDataPath + saveFileName + ".json";     
    //             using StreamWriter streamWriter = new StreamWriter(savePath);
    //             streamWriter.Write();
    //             succseded = true;
    //         } 
    //     }
    //     catch (System.Exception Err)
    //     {
    //         Debug.LogError("Error in saving: "+Err.ToString());
    //         throw;
    //     }
    //     return succseded;
    // }
    // private GameData Load()
    // {

    // }
    private void Awake()
    {
        Data = new();
        DontDestroyOnLoad(gameObject);
    }
    internal void ChangeGain(float ammount){
        
    }
}
public class GameData{
    public float Money {get; private set;}
    public float Gain {get;private set;} // used only for other scripts
    /*
        MEANING FOR BELLOW:
        0 - Plant;
        1 - Horbevore;
        2 - Predators;
    */
    internal int[] Beings = new int[2]; 
    internal int[] Unlocks = new int[2];
    internal void ChangeGain(float ammount){
        if (Gain+ammount < 0.05f) Gain=0.05f;
        else
        Gain+=ammount;
    }
    internal void ChangeMoney(float ammount){
        if (Money+ammount < 0) Money=0;
        else
        Money+=ammount;
    }
}