using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;
using Unity.Mathematics;
using System.Threading.Tasks;
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
    float timebeforesave = 10;
    internal async Task<bool> Save()
    {
        bool succseded = false;
        try
        {
            for (int ind=0;ind<Data.Resources.Count;ind++)
            {
                double curr = math.round(Data.Resources[ind].Current*10)/10;
                Data.Resources[ind].Current = curr;
            }
            await Task.Delay(5);
            string _DataString = JsonUtility.ToJson(Data);
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
    internal async Task<GameData> Load()
    {
        GameData Newdata = new(baseResources[0].ResourceNameID);
        try
        {
            if (!Application.isEditor&&SecurePlayerPrefs.HasKey("Game_Data")){
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
                    await Task.Delay(50);
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
    private async Awaitable Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(gameObject);
        await Awaitable.WaitForSecondsAsync(.1f);

        shopData = PrimaryShopHandler.instance;
        if (shopData == null)
        {
            do
            {
                shopData = PrimaryShopHandler.instance;
                await Awaitable.WaitForSecondsAsync(.1f);
            } while (shopData == null);
        }
        SecurePlayerPrefs.Init();
        Task<GameData> ld = Load();
        Data = await ld;
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
    async Task Update()
    {
        if (timebeforesave <= 0)
        {
            timebeforesave = 10;
            Debug.Log("Saving...");
            Task<bool> st = Save();
            bool saved = await st;
            if (!saved) Debug.LogWarning("Failed to save.");
            else Debug.Log("Saved.");
        }
        else
        {
            timebeforesave -= Time.deltaTime;
        }
    }
    async Task OnApplicationQuit()
    {
        Task<bool> st = Save();
        bool saved = await st;
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