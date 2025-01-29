using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGainer : MonoBehaviour
{
    [SerializeField] private Public_Data moneyStorage;
    [SerializeField] private float resourceGain = 1;
    [SerializeField] private int resourceType = 0;
    [SerializeField] private float cooldown = 0.5f;
    float cd;
    void Start()
    {
        cd = cooldown;
        if (moneyStorage == null){
            moneyStorage = FindObjectOfType<Public_Data>();
            Debug.LogWarning("Error, no Public_Data connected to gameobject.");
            if (moneyStorage == null) Destroy ( gameObject);
        }
        moneyStorage.ChangeGain(resourceType,resourceGain/cooldown);
    }

    void Update()
    {
        cd -= Time.deltaTime;
        if (cd <= 0){
            cd = cooldown;
            moneyStorage.ChangeMoney(resourceType,resourceGain);
        }
    }
    void OnDestroy(){
        moneyStorage.ChangeGain(resourceType,-resourceGain/cooldown);
    }
}
