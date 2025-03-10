using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;
public class Public_Data : MonoBehaviour
{
    [field:SerializeField] internal List<Resource> baseResources; // idk where to place it, so il do it here
    [field:SerializeField] internal List<Sprite> baseSprites; // idk where to place it, so il do it here
    [SerializeField] private GameObject animalPrefab;
    [SerializeField] private Transform animalsParent;

    internal GameData Data;

    internal bool Save()
    {
        bool succseded = false;
        try
        {
            string _DataString = JsonConvert.SerializeObject(Data);
            Debug.Log(_DataString);
            if (_DataString != null && _DataString != "null"){
                SecurePlayerPrefs.SetString("Game_Data",_DataString);
                SecurePlayerPrefs.Save();
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
            
            if (SecurePlayerPrefs.HasKey("Game_Data")){
                string _Data = SecurePlayerPrefs.GetString("Game_Data","{}");
                Debug.Log(_Data);
                if (_Data == null || _Data == "null")
                {
                    SecurePlayerPrefs.DeleteKey("Game_Data");
                    Debug.LogError("Null was not expected.");
                    return new GameData(baseResources[0].ResourceName);
                } 
                else 
                {
                    GameData _GData = JsonConvert.DeserializeObject<GameData>(_Data);
                    if (_GData.Beings.Count > 0){
                        foreach (AnimalsSO animal in Data.Beings){
                            GameObject cloned = Instantiate(animalPrefab);
                            cloned.GetComponent<AnimalBehaivor>().animalSO = animal;
                        }
                    }
                    Newdata = _GData;
                }

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
        SecurePlayerPrefs.Init();
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
                newres.Gain += ammount;
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
    internal List<Resource> Resources;
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