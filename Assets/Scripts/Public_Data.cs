using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;


public class Public_Data : MonoBehaviour
{
    [SerializeField] private string saveFileName;

    public GameData Data {get;private set;}
    private byte[] savedKey;
    FileStream dataStream;
    string fullSaveFileName;

    internal bool Save(){
        bool succseded = false;
        try
        {
            Aes IAes = Aes.Create();
            savedKey = IAes.Key;
            byte[] inputIV = IAes.IV;
            if (Data != null){
                string savePath = fullSaveFileName;     
                dataStream = new FileStream(savePath,FileMode.Create); // creates new path to write on
                dataStream.Write(inputIV, 0, inputIV.Length);
                CryptoStream newstream = new(dataStream,IAes.CreateEncryptor(savedKey,inputIV),CryptoStreamMode.Write); // encrypted text w/ location, idfk how this works
                StreamWriter streamWriter = new(newstream); // creates new writer to write on
                streamWriter.Write(JsonUtility.ToJson(Data));
                streamWriter.Close();
                newstream.Close();
                dataStream.Close();
                succseded = true;
            } 
        }
        catch (System.Exception Err)
        {
            Debug.LogError("Error in saving: "+Err.ToString());
            throw;
        }
        return succseded;
    }
    internal GameData Load(){
        GameData Newdata = new();
        if (File.Exists(fullSaveFileName)){
            dataStream = new FileStream(fullSaveFileName, FileMode.Open);
            // Create new AES instance.
            Aes oAes = Aes.Create();

            // Create an array of correct size based on AES IV.
            byte[] outputIV = new byte[oAes.IV.Length];
            
            // Read the IV from the file.
            dataStream.Read(outputIV, 0, outputIV.Length);

            // Create CryptoStream, wrapping FileStream
            CryptoStream oStream = new CryptoStream(
                   dataStream,
                   oAes.CreateDecryptor(savedKey, outputIV),
                   CryptoStreamMode.Read);

            // Create a StreamReader, wrapping CryptoStream
            StreamReader reader = new StreamReader(oStream);
            
            // Read the entire file into a String value.
            string text = reader.ReadToEnd();
            // Always close a stream after usage.
            reader.Close();

            // Deserialize the JSON data 
            //  into a pattern matching the GameData class.
            Newdata = JsonUtility.FromJson<GameData>(text);
        }
        return Newdata;
    }
    // private GameData Load()
    // {

    // }
    private void Awake()
    {
        fullSaveFileName = Application.persistentDataPath + "/" + (Application.isPlaying?saveFileName:"Development") + ".json";
        Data = new();
        DontDestroyOnLoad(gameObject);
        Load();
    }
}
public class GameData{
    public float[] Money {get; internal set;} = new float[3];
    internal int[] Beings = new int[3]; 
    internal int[] Unlocks = new int[3];
    internal void ChangeMoney(int resource,float ammount){ 
        if (resource >= 0 && resource <= Money.Length) { // failsafe to not break it.
            Money[resource] += ammount;
            if (Application.isEditor) Debug.Log(Money[0]+" | "+Money[1]+" | "+ Money[2]);
        }
    }
}