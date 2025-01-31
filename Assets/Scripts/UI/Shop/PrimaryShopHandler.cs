using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PrimaryShopHandler : MonoBehaviour
{
    [Header("Set-up")]
    [SerializeField] private Transform optionsParent;
    [SerializeField] private GameObject optionTemplate;
    [SerializeField] private GameObject animalTemplate;
    [SerializeField]  private List<Button> AmmountButtons;
    [SerializeField]  private List<int> AmmountOptions;
    [SerializeField] private List<AnimalsSO> purchasableOptions;
    [SerializeField] private Public_Data globalData;
    private List<GameObject> visibleOptions;
    private Dictionary<Button,int> purchaseAmmountSelect;
    int currentBuyingAmmount = 1;
    int unlockedlastanimal = -1; 
    int lastPurchasedAnimal = -1;
    private void OnButtonClicked(AnimalsSO animal){
        int neededresID = animal.AnimalResourceSubstractID;
        if (globalData.Data.Resources.Count >= neededresID && globalData.Data.Resources[neededresID].Current >= animal.AnimalResourceCost*currentBuyingAmmount){
            globalData.Data.Resources[neededresID].Current -= animal.AnimalResourceCost*currentBuyingAmmount;
            for (int i =0; i < currentBuyingAmmount; i++){
                GameObject cloned = Instantiate(animalTemplate);
                cloned.GetComponent<AnimalBehaivor>().animalSO = animal;
            }
        }
    }
    private void AddAnimalOption(AnimalsSO animal, int id){
        GameObject newGO = Instantiate(optionTemplate,optionsParent);
        if (newGO.TryGetComponent<ShopOption>(out ShopOption option) == true){
            option.Animal = animal;
            newGO.name = animal.animalName;
            visibleOptions.Add(newGO);
            option.button.onClick.AddListener(()=>{
                if (lastPurchasedAnimal < id){
                    lastPurchasedAnimal=id;
                    globalData.Data.Unlocked++;
                    Debug.Log(globalData.Data.Unlocked);
                }
                OnButtonClicked(animal);
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
                option.Multiplier = currentBuyingAmmount;
            }
        }
    }
    void Start()
    {
        visibleOptions = new();
        purchaseAmmountSelect = new();
        for (int i=0; i< AmmountButtons.Count; i++){
            purchaseAmmountSelect.Add(AmmountButtons[i],AmmountOptions[i]);
        }
        unlockedlastanimal = globalData.Data.Unlocked;
        RefreshAvailable();
        foreach (KeyValuePair<Button,int> valuePair in purchaseAmmountSelect){
            valuePair.Key.onClick.AddListener(()=>{
                if (currentBuyingAmmount != valuePair.Value){
                    valuePair.Key.Select();
                    currentBuyingAmmount = valuePair.Value;
                }
            });
        }
    }

    void Update()
    {
        int newUnlock = globalData.Data.Unlocked;    
        if (newUnlock  > unlockedlastanimal){
            RefreshAvailable();
        }    
        UpdateOptions();
    }
}
