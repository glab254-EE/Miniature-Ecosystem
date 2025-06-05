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
    }
    public void OnClick(){
        if (data == null)
        {
            data = Public_Data.instance;
        }
        if (Application.isEditor)
        {
            data.ChangeMoney(resourceID, resourcePerClick*100);
        }
        else
        {
            data.ChangeMoney(resourceID, resourcePerClick);
        }
    }
}
