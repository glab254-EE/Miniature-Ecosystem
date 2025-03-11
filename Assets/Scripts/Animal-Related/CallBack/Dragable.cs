using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AnimalBehaivor))]
public class Dragable : MonoBehaviour
{
    [SerializeField] private float Speed;
    private Camera _Camera;
    bool isdragging;
    private AnimalBehaivor selfAnimalBehaivor;
    CameraMoveAndZoomController cameraMoveBeh;
    private void Start()
    {
        selfAnimalBehaivor = GetComponent<AnimalBehaivor>();
        _Camera = Camera.main;
    }
    private void OnMouseDown()
    {
        isdragging = true;
        if (_Camera.gameObject.TryGetComponent<CameraMoveAndZoomController>(out cameraMoveBeh))
        {
            cameraMoveBeh.Dragginganimal = true;
            Debug.Log(cameraMoveBeh.Dragginganimal);
        }
    }
    private void OnMouseUp()
    {
        isdragging = false;
        selfAnimalBehaivor.Dragged  = false;
        if (cameraMoveBeh != null)
        {
            cameraMoveBeh.Dragginganimal = false;
        }
    } 
    void Update()
    {
        if (isdragging){
            Vector3 movementVector = Vector3.Lerp(transform.position,_Camera.ScreenToWorldPoint(Input.mousePosition),Time.deltaTime*Speed);
            transform.position = movementVector;
            if (selfAnimalBehaivor.Dragged == false) selfAnimalBehaivor.Dragged= true;
        }
    }
}
