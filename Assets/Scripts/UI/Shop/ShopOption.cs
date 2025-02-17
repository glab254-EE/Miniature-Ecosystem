using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopOption : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] internal Button button;
    [SerializeField] private TMP_Text animal_Name;
    [SerializeField] private TMP_Text animal_Cost;
    [SerializeField] private TMP_Text animal_Gain;
    [SerializeField] private TMP_Text purchaseButton_Text;
    [SerializeField] private Image animal_CostedResourcePreview;
    [SerializeField] private Image animal_GainResourcePreview;
    internal AnimalsSO Animal;
    internal Sprite ResourceneededSprite;
    internal Sprite ResourcegainSprite;
    internal float Gain;
    internal int Multiplier;
    private Rounderer _rounderer;
    void Start()
    {
        _rounderer = new();
        if (Animal != null){
            image.sprite = Animal.BaseSprite;
            animal_Name.text = Animal.animalName;
            animal_Cost.text = (Animal.AnimalResourceCost * Multiplier).ToString();
        }
    }
    void Update()
    {
        animal_Cost.text = _rounderer.ToRoundedString(Animal.AnimalResourceCost * Multiplier);       
        animal_Gain.text = $"+{_rounderer.ToRoundedString(Animal.GainAmmount/Animal.GainDelay)}/ск.";
        animal_CostedResourcePreview.sprite = ResourceneededSprite;
        animal_GainResourcePreview.sprite = ResourcegainSprite; 
        purchaseButton_Text.text = $"Buy x{Multiplier}";
    }
}
