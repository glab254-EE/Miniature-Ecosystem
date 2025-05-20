using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceClick : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private float resourcePerClick;
    [SerializeField] private int resourceID;
    private Public_Data data;
    void Start()
    {
        data = Public_Data.instance;
        button.onClick.AddListener(OnClick);
    }
    void OnClick(){
        if (data == null)
        {
            data = Public_Data.instance;
        }
        data.ChangeMoney(resourceID, resourcePerClick);
    }
}
