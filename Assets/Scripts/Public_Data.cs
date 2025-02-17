using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;


public class Public_Data : MonoBehaviour
{
    [field:SerializeField] internal List<Resource> baseResources; // idk where to place it, so il do it here
    [SerializeField] private GameObject animalPrefab;
    [SerializeField] private Transform animalsParent;
    private string saveFileName = "SaveFile1";

    public GameData Data {get;private set;}
    private byte[] savedKey = {54,12,251,237,79,132,131,72,49, 190, 207, 87, 224, 86,255,0};
    FileStream dataStream;
    string fullSaveFileName;

    internal bool Save()
    {
        bool succseded = false;
        try
        {
            Aes IAes = Aes.Create();
            byte[] inputIV = IAes.IV;
            if (Data != null){
                if (File.Exists(fullSaveFileName)) File.Delete(fullSaveFileName);
                dataStream = new FileStream(fullSaveFileName,FileMode.Create); // creates new path to write on
                dataStream.Write(inputIV, 0, inputIV.Length);
                CryptoStream newstream = new(dataStream,IAes.CreateEncryptor(savedKey,inputIV),CryptoStreamMode.Write); // encrypted text w/ location, idfk how this works
                StreamWriter streamWriter = new(newstream); // creates new writer to write on
                string _text = JsonUtility.ToJson(Data);
                streamWriter.Write(_text);
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
    internal GameData Load()
    {
        GameData Newdata = new(baseResources[0].ResourceName);
        try
        {
            if (File.Exists(fullSaveFileName))
            {
                dataStream = new FileStream(fullSaveFileName, FileMode.Open);
                Aes oAes = Aes.Create();
                byte[] outputIV = new byte[oAes.IV.Length];
                dataStream.Read(outputIV, 0, outputIV.Length);
                CryptoStream oStream = new CryptoStream(
                    dataStream,
                    oAes.CreateDecryptor(savedKey, outputIV),
                    CryptoStreamMode.Read);
                StreamReader reader = new StreamReader(oStream);
                string _text = reader.ReadToEnd();
                Newdata = JsonUtility.FromJson<GameData>(_text);
                if (Newdata != null && Newdata.Beings != null){
                    foreach (AnimalsSO Animal in Newdata.Beings){
                        GameObject newobject = Instantiate(animalPrefab,animalsParent);
                        if (newobject.TryGetComponent<AnimalBehaivor>(out AnimalBehaivor behaivor)){
                            behaivor.animalSO = Animal;
                        } else {
                            Destroy(newobject);
                        }
                    }
                }
                Debug.Log(_text);
                reader.Close();
                oStream.Close();
                dataStream.Close();
            }
        }
        catch (System.Exception Err){
            Debug.LogError("Error in loading: "+Err.ToString());
            throw;
        }
        return Newdata;
    }
    private void Awake()
    {
        fullSaveFileName = Application.persistentDataPath + "/" + (Application.isEditor?"Development":saveFileName) + ".json";
        Data = Load();
        DontDestroyOnLoad(gameObject);
        
    }
    internal void ChangeMoney(int resourceID,float ammount)
    { 
        if (resourceID >= 0 && Data != null && Data.Resources != null){
            if (resourceID < Data.Resources.Count && Data.Resources[resourceID] != null) { // failsafe to not break it.
                Data.Resources[resourceID].Current += ammount;
            }
            else if (baseResources.Count > resourceID){
                Resource newres =  new(baseResources[resourceID].ResourceName);
                Sprite sprite = baseResources[resourceID].ReferencedSprite;
                newres.ReferencedSprite = sprite != null ? sprite : null;
                newres.Current += ammount;
                Data.Resources.Add(newres);
            }
        }
    }
    internal void ChangeGain(int resourceID,float ammount)
    {
        if (resourceID >= 0 && Data != null && Data.Resources != null){
            if (resourceID < Data.Resources.Count && Data.Resources[resourceID] != null) { // failsafe to not break it.
                Data.Resources[resourceID].Gain += ammount; 
            }
            else if (baseResources.Count > resourceID){
                Resource newres =  new(baseResources[resourceID].ResourceName);
                Sprite sprite = baseResources[resourceID].ReferencedSprite;
                newres.ReferencedSprite = sprite != null ? sprite : null;
                newres.Gain += ammount;
                Debug.Log(newres.ResourceName);
                Data.Resources.Add(newres);
            }
        }
    }
    void OnApplicationQuit()
    {
        bool saved = Save();
        if (!saved) Debug.LogWarning("Something went wrong while saving data.");
        else Debug.Log("Saved.");
    }
    
}
[System.Serializable]
public class GameData
{
    internal bool TutorialCompleted=false;
    public List<Resource> Resources {get; internal set;}
    internal List<AnimalsSO> Beings = new(); 
    internal int Unlocked = 0;
    public GameData(){
        Resources = new();
    }
    public GameData(string firstresource){
        Resources = new();
        Resources.Add(new Resource(firstresource));
    }
}