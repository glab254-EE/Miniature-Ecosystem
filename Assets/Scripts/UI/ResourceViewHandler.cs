using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class ResourceViewHandler : MonoBehaviour
{
    [SerializeField] private GameObject template;
    [SerializeField] private Transform parent;
    [SerializeField] private Public_Data _Data;
    private List<ResourceUIViewer> _currentResViewers;
    private int _lastResIDShowed;
    private int _allAvalResLastID;
    private void CreateResourceViewer(Resource resource1){
        GameObject clone = Instantiate(template,parent);
        if (clone.TryGetComponent<ResourceUIViewer>(out ResourceUIViewer newui)){
            newui._resSprite = resource1.ReferencedSprite;
            newui._resName = resource1.ResourceName;
            newui._resOtherText = string.Format("{0}\n{1} в сек.",resource1.Current,resource1.Gain);
            newui.referencedres = resource1;
            _currentResViewers.Add(newui);
        } else {
            Destroy(clone);
        }
    }
    private void UpdateViewers(){
        int maxcount = _Data.Data.Resources.Count;
        if (maxcount != _currentResViewers.Count){
            if (_currentResViewers.Count > maxcount){
                for (int i = 0; i < _currentResViewers.Count; i++){
                    if (_currentResViewers[i] != null) Destroy(_currentResViewers[i].gameObject);
                }
            }
            // create missing viewers, changes its values if exsists
            for (int i =0; i < maxcount; i++){ 
                _lastResIDShowed = i;
                Resource res = _Data.Data.Resources[i];
                if (res != null ){
                    CreateResourceViewer(res);
                } 
            }
        }
        for (int i = 0; i < _currentResViewers.Count; i ++){
            ResourceUIViewer viewer = _currentResViewers[i];
            viewer._resOtherText = string.Format("{0}\n{1} в сек.",viewer.referencedres.Current,viewer.referencedres.Gain);
        }
    }
    void Start()
    {
        _currentResViewers = new();
        _allAvalResLastID = 0;
        _lastResIDShowed = -1;   
        _allAvalResLastID = _Data.Data.Resources.Count;
    }

    void Update()
    {
        _allAvalResLastID = _Data.Data.Resources.Count;
        UpdateViewers();

    }
}
