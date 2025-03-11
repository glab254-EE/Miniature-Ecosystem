using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class ResourceViewHandler : MonoBehaviour
{
    [SerializeField] private GameObject template;
    [SerializeField] private Transform parent;
    [SerializeField] private Public_Data _Data;
    private Rounderer _rounderer;
    private List<ResourceUIViewer> _currentResViewers;
    private int _lastResIDShowed;
    private int _allAvalResLastID;
    private void CreateResourceViewer(Resource resource1)
    {
        GameObject clone = Instantiate(template,parent);
        if (clone.TryGetComponent<ResourceUIViewer>(out ResourceUIViewer newui)){
            newui._resName = _Data.resourceNames[resource1.ResourceNameID];
            
            newui._resOtherText = string.Format("{0} ед.\n{1} в сек.",_rounderer.ToRoundedString(resource1.Current),_rounderer.ToRoundedString(resource1.Gain));
            newui.referencedres = resource1;
            _currentResViewers.Add(newui);
        } else {
            Destroy(clone);
        }
    }
    private void CreateResourceViewer(Resource resource1,int sprite)
    {
        GameObject clone = Instantiate(template,parent);
        if (clone.TryGetComponent<ResourceUIViewer>(out ResourceUIViewer newui)){
            newui._resSprite = _Data.baseSprites[sprite];
            newui._resName = _Data.resourceNames[resource1.ResourceNameID];
            
            newui._resOtherText = string.Format("{0} ед.\n{1} в сек.",_rounderer.ToRoundedString(resource1.Current),_rounderer.ToRoundedString(resource1.Gain));
            newui.referencedres = resource1;
            _currentResViewers.Add(newui);
        } else {
            Destroy(clone);
        }
    }
    private void UpdateViewers(){
        int maxcount = _Data.Data.Resources.Count;
        if (maxcount != _currentResViewers.Count){
            if (_currentResViewers.Count > maxcount)
            {
                for (int i = 0; i < _currentResViewers.Count; i++){
                    if (_currentResViewers[i] != null) Destroy(_currentResViewers[i].gameObject);
                }
            }
            // create missing viewers, changes its values if exsists
            for (int i =0; i < maxcount; i++)
            { 
                _lastResIDShowed = i;
                Resource res = _Data.Data.Resources[i]; // blia, forgot another data 😭😭😭
                if (res != null ){
                    if (_Data.baseResources.Count > i && _Data.baseResources[i].ReferencedSprite != -1)
                    {
                        CreateResourceViewer(res,_Data.baseResources[i].ReferencedSprite);
                    } else CreateResourceViewer(res);
                } 
            }
        }
        for (int i = 0; i < _currentResViewers.Count; i ++)
        {
            ResourceUIViewer viewer = _currentResViewers[i];
            viewer._resOtherText = string.Format("{0} ед.\n{1} в сек.",_rounderer.ToRoundedString(viewer.referencedres.Current),_rounderer.ToRoundedString(viewer.referencedres.Gain));
        }
    }
    void Start()
    {
        _rounderer = new();
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
