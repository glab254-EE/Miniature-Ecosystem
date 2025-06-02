using System.Collections.Generic;
using UnityEngine;

public class AnimalsManager : MonoBehaviour
{
    internal static AnimalsManager Instance;
    [SerializeField] private string AnimalsParentName = "Animals";
    [SerializeField] private GameObject AnimalsPrefab;
    [SerializeField] private Transform AnimalsParent;
    [SerializeField] private int AnimalCountLimit = 5;
    private Dictionary<string, int> cellCount = new();
    private List<double> GainPerSecond = new();
    private Public_Data data;
    private float Tick;
    private void AddToWorld(AnimalsSO cell)
    {
        if (AnimalsPrefab == null) return;
        GameObject cloned = Instantiate(AnimalsPrefab,AnimalsParent);
        if (cloned.TryGetComponent<AnimalBehaivor>(out AnimalBehaivor behaivor))
        {
            behaivor.animalSO = cell;
            if (cellCount.ContainsKey(cell.AnimalID))
            {
                cellCount[cell.AnimalID] += 1;
            }
            else
            {
                cellCount.Add(cell.AnimalID, 1);
            }
        }
        else Destroy(cloned);
    }

    internal void AddCellInstance(AnimalsSO cell)
    {

        if (cell == null)
        {
            Debug.LogWarning("No cell inputed.");
            return;
        }
        try
        {
            if (!cellCount.ContainsKey(cell.AnimalID) || cellCount[cell.AnimalID] <= AnimalCountLimit)
            {
                AddToWorld(cell);
            }
            else
            {
                cellCount[cell.AnimalID] += 1;
            }
            if (cell.GainAmmount > 0 && cell.GainDelay > 0)
            {
                if (GainPerSecond.Count <= cell.ResourceID)
                {
                    GainPerSecond.Add(cell.GainAmmount / cell.GainDelay);
                }
                else
                {
                    GainPerSecond[cell.ResourceID] += cell.GainAmmount / cell.GainDelay;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Animals manager failed to instance animal. " + e);
        }
        
    }
    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);        
    }
    void Start()
    {
        data = Public_Data.instance;
        if (AnimalsParent == null) AnimalsParent = GameObject.Find(AnimalsParentName).transform;
    }
    private void GainSecond()
    {
        for (int id = 0; id < GainPerSecond.Count; id++)
        {
            data.SetGain(id, GainPerSecond[id]);
            data.ChangeMoney(id, GainPerSecond[id]);
        }
    }
    void Update()
    {
        if (Tick < 1)
        {
            Tick += Time.deltaTime;
        }
        else
        {
            Tick = 0;
            GainSecond();
        }
    }
}
