using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;
using Unity.Mathematics;
public class Public_Data : MonoBehaviour
{
    [field:SerializeField] internal List<Resource> baseResources; // idk where to place it, so il do it here
    [field:SerializeField] internal List<Sprite> baseSprites; // idk where to place it, so il do it here
    [field:SerializeField] internal List<string> resourceNames;
    [SerializeField] private GameObject animalPrefab;
    [SerializeField] private Transform animalsParent;
    private PrimaryShopHandler shopData;
    internal static Public_Data instance;
    internal GameData Data;

    internal bool Save()
    {
        bool succseded = false;
        try
        {
            GameData cloned = Data;
            for (int ind=0;ind<Data.Resources.Count;ind++)
            {
                double curr = math.round(Data.Resources[ind].Current*10)/10;
                Data.Resources[ind].Current = curr;
            }
            string _DataString = JsonUtility.ToJson(cloned);
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
        GameData Newdata = new(baseResources[0].ResourceNameID);
        try
        {
            
            if (SecurePlayerPrefs.HasKey("Game_Data")){
                string _Data = SecurePlayerPrefs.GetString("Game_Data");
                Debug.Log(_Data);
                if (_Data == null || _Data == "null" || _Data.Contains("?"))
                {
                    SecurePlayerPrefs.DeleteKey("Game_Data");
                    Debug.LogError("Null was not expected.");
                    return new GameData(baseResources[0].ResourceNameID);
                } 
                else 
                {
                    GameData _GData = JsonUtility.FromJson<GameData>(_Data);
                    for (int i=0;i<_GData.Resources.Count;i++)
                    {
                        if (_GData.Resources[i].ReferencedSprite < 0)
                        {
                            _GData.Resources[i].ReferencedSprite = 0;
                        }
                        if (_GData.Resources[i].ResourceNameID < 0)
                        {
                            _GData.Resources[i].ResourceNameID = 0;
                        }
                    }
                    if (_GData.purchasedAnimals.Count > 0){
                        for (int ind = 0; ind < _GData.purchasedAnimals.Count;ind++)
                        {
                            if (shopData == null || shopData.purchasableOptions.Count < ind || shopData.purchasableOptions[ind] == null) continue;
                            AnimalsSO animal = shopData.purchasableOptions[ind];
                            int count = _GData.purchasedAnimals[ind];
                            for (int _=0; _<count; _++)
                            {
                                AnimalsManager.Instance.AddCellInstance(animal);
                            }
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
        if (instance != null) Destroy(gameObject);
        instance = this;
        shopData = PrimaryShopHandler.instance;
        SecurePlayerPrefs.Init();
        Data = Load();
        DontDestroyOnLoad(gameObject);   
    }
    internal void ChangeMoney(int resourceID,double ammount)
    { 
        if (resourceID >= 0 && Data != null && Data.Resources != null){
            if (resourceID < Data.Resources.Count && Data.Resources[resourceID] != null) { // failsafe to not break it.
                Data.Resources[resourceID].Current += ammount;
            }
            else if (baseResources.Count > resourceID){
                Resource newres =  new(baseResources[resourceID].ResourceNameID);
                newres.Current += ammount;
                Data.Resources.Add(newres);
            }
        }
    }
    internal void SetGain(int resourceID,double ammount)
    {
        if (resourceID >= 0 && Data != null && Data.Resources != null){
            if (resourceID < Data.Resources.Count && Data.Resources[resourceID] != null) { // failsafe to not break it.
                Data.Resources[resourceID].Gain = ammount; 
            }
            else if (baseResources.Count > resourceID){
                Resource newres =  new(baseResources[resourceID].ResourceNameID);
                newres.Gain = ammount;
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
    [SerializeField]    internal bool TutorialCompleted=false;
    [SerializeField]    internal List<Resource> Resources = new();
    [SerializeField]    internal List<int> purchasedAnimals = new();
    [SerializeField]    internal int Unlocked = 0;
    public GameData(int firstresource)
    {
        Resources.Add(new Resource(firstresource));
    }
}