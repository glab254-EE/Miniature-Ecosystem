using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PrimaryShopHandler : MonoBehaviour
{
    [Header("Set-up")]
    [SerializeField] private Transform optionsParent;
    [SerializeField] private GameObject optionTemplate;
    [SerializeField] private GameObject animalTemplate;
    [SerializeField] internal List<AnimalsSO> purchasableOptions; // changed to internal for letting data access it.
    private Public_Data globalData;
    internal static PrimaryShopHandler instance;
    private List<GameObject> visibleOptions;
    private Dictionary<Button,int> purchaseAmmountSelect;
    int currentBuyingAmmount = 1;
    [SerializeField] List<Unlockable> unlocked;
    int unlockedlastanimal = -1; 
    int lastPurchasedAnimal = -1;
    private int CalculateMaxPurchase(double cost, double currentbuy){
        int output = 0;
        double leftovers = currentbuy;
        while (true){
            if (leftovers < cost) break;
            output++;
            leftovers -= cost;
        }
        return output;
    }
    private int CalculateMaxPurchase(double cost, double currentbuy, int max){
        int output = 0;
        double leftovers = currentbuy;
        for (int i = 0; i < max; i++){
            if (leftovers < cost) break;
            output++;
            leftovers -= cost;
        }
        return output;
    }
    private void OnButtonClicked(AnimalsSO animal, int id){
        int neededresID = animal.AnimalResourceSubstractID;
        int _currentbuyammount = 1;
        if (currentBuyingAmmount == -1){
            _currentbuyammount = CalculateMaxPurchase(animal.AnimalResourceCost,globalData.Data.Resources[neededresID].Current);
        }
        else if (currentBuyingAmmount > 1){
            _currentbuyammount = CalculateMaxPurchase(animal.AnimalResourceCost,globalData.Data.Resources[neededresID].Current,currentBuyingAmmount);
        } else {
            _currentbuyammount = 1;
        }

        if (globalData.Data.Resources.Count > neededresID && globalData.Data.Resources[neededresID].Current > animal.AnimalResourceCost*_currentbuyammount){
            if (lastPurchasedAnimal < id){
                lastPurchasedAnimal=id;
                globalData.Data.Unlocked=id+1;
            }
            globalData.Data.Resources[neededresID].Current -= animal.AnimalResourceCost*_currentbuyammount;
            for (int i =0; i < _currentbuyammount; i++){
                AnimalsManager.Instance.AddCellInstance(animal);
                if (globalData.Data.purchasedAnimals.Count <= id)
                {
                    globalData.Data.purchasedAnimals.Add(1);
                }
                else
                {
                    globalData.Data.purchasedAnimals[id] += 1;
                }
            }
        }
    }
    private void AddAnimalOption(AnimalsSO animal, int id){
        GameObject newGO = Instantiate(optionTemplate,optionsParent);
        if (newGO.TryGetComponent<ShopOption>(out ShopOption option) == true){
            option.Animal = animal;
            newGO.name = animal.animalName;
            visibleOptions.Add(newGO);
            if (globalData.baseResources[animal.ResourceID].ReferencedSprite != -1){
                Sprite sprite = globalData.baseSprites[globalData.baseResources[animal.AnimalResourceSubstractID].ReferencedSprite];
                option.ResourceneededSprite = sprite;
            }
            if (globalData.baseResources[animal.AnimalResourceSubstractID].ReferencedSprite != -1){
                Sprite sprite = globalData.baseSprites[globalData.baseResources[animal.AnimalResourceSubstractID].ReferencedSprite];
                option.ResourceneededSprite = sprite;
            }

            option.button.onClick.AddListener(()=>{
                OnButtonClicked(animal,id);
                });
        } else {
            Destroy(newGO);
        }
    }
    private void RemoveAllOptions(){
        foreach (Transform t in optionsParent){
            Destroy(t.gameObject);
        }
    }
    private void RefreshAvailable(){
        RemoveAllOptions();
        unlockedlastanimal = globalData.Data.Unlocked;
        for (int i=0; i<purchasableOptions.Count; i++){
            if (i>unlockedlastanimal) break;
            AnimalsSO animal = purchasableOptions[i];
            if (animal != null){
                AddAnimalOption(animal,i);
            }
        }
    }
    private void UpdateOptions(){
        for (int i = 0; i < visibleOptions.Count; i++){
            GameObject go = visibleOptions[i];
            if (go != null && go.TryGetComponent<ShopOption>(out ShopOption option)){
                if (currentBuyingAmmount == -1){
                    option.Multiplier = CalculateMaxPurchase(option.Animal.AnimalResourceCost,globalData.Data.Resources[option.Animal.AnimalResourceSubstractID].Current);
                }
                else if (currentBuyingAmmount > 1){
                    option.Multiplier = CalculateMaxPurchase(option.Animal.AnimalResourceCost,globalData.Data.Resources[option.Animal.AnimalResourceSubstractID].Current,currentBuyingAmmount);
                } else {
                    option.Multiplier = currentBuyingAmmount;
                }
            }
        }
    }
    bool canUpdate = false;
    async Task Start()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(gameObject);
        globalData = Public_Data.instance;
        visibleOptions = new();
        purchaseAmmountSelect = new();
        await Task.Delay(15);
        canUpdate = true;
        unlockedlastanimal = globalData.Data.Unlocked;
        RefreshAvailable();
    }
    void Update()
    {
        if (!canUpdate || globalData.Data == null) return;
        int newUnlock = globalData.Data.Unlocked;    
        if (newUnlock  > unlockedlastanimal){
            RefreshAvailable();
        }    
        UpdateOptions();
    }
}
