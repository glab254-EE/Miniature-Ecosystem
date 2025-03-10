using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceClick : MonoBehaviour
{
    [SerializeField] private Public_Data data;
    [SerializeField] private Button button;
    [SerializeField] private float resourcePerClick;
    [SerializeField] private int resourceID;
    void Start()
    {
        button.onClick.AddListener(OnClick);
    }
    void OnClick(){
        if (data.Data.TutorialCompleted){
            data.ChangeMoney(resourceID,resourcePerClick);
        }
    }
}
