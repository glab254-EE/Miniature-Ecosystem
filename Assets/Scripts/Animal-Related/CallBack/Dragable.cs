using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dragable : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private float Speed;
    private Camera _Camera;
    private GameObject _target;
    private AnimalBehaivor _targetAnimalBehaivor;
    private void Start()
    {
        _Camera = Camera.current;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _target = gameObject;
        if (_target.TryGetComponent<AnimalBehaivor>(out _targetAnimalBehaivor))
        {
            _targetAnimalBehaivor.Dragged = true;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_target != null){
            if (_targetAnimalBehaivor != null)
            {
                _targetAnimalBehaivor.Dragged = false;
            }
            _target = null;
        }
    }
    void Update()
    {
        if (_target != null && _target.transform != null){
            Vector3 movementVector = Vector3.Lerp(_target.transform.position,_Camera.ScreenToWorldPoint(Input.mousePosition),Time.deltaTime*Speed);
            _target.transform.position = movementVector;
        }
    }
}
