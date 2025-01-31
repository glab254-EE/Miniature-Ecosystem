using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopOption : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] internal Button button;
    [SerializeField] private TMP_Text animal_Name;
    [SerializeField] private TMP_Text animal_Cost;
    internal AnimalsSO Animal;
    internal int Purchased;
    internal int Multiplier;
    void Start()
    {
        if (Animal != null){
            image.sprite = Animal.BaseSprite;
            animal_Name.text = Animal.animalName;
            animal_Cost.text = ((Purchased + 1) * Animal.AnimalResourceCost * Multiplier).ToString();
        }
    }
    void Update()
    {
        animal_Cost.text = ((Purchased + 1) * Animal.AnimalResourceCost * Multiplier).ToString();        
    }
}
