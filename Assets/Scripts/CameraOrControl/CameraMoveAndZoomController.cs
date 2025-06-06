using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraMoveAndZoomController : MonoBehaviour
{
    [Header("Zoom-Related")]
    [SerializeField] private float MaxZoom = 4;
    [SerializeField] private float MinZoom = 10;
    [SerializeField] private float FovSensitivity = 2;
    [Header("Movement-Related")]
    [SerializeField] private Vector2 LowerLimits;
    [SerializeField] private Vector2 UpperLimits;
    [SerializeField] private float MoveSpeed = 2;
    [SerializeField] private LayerMask ignoremask;
    private Vector3 resetPosition;
    private Vector3 difference;
    private Vector3 dragOrigin;
    private Camera _Camera;
    private InputSys _inAct;
    float ScrollDriection;
    bool draggingCamera = false;
    void Awake()
    {
        _Camera = Camera.main;
        resetPosition = _Camera.transform.position;
        ScrollDriection = 0f;
        _inAct = new();
        _inAct.Player.OnZoom.performed += OnScroll;
        _inAct.Player.Fire.performed +=OnClick;
        //_inAct.Player.Fire.canceled += OnClick;
        _inAct.Player.Fire.Enable();

        _inAct.Player.OnZoom.Enable();
    }
    private void OnScroll(InputAction.CallbackContext callbackContext)
    {
        Vector2 vector = callbackContext.ReadValue<Vector2>();
        if (vector.y != 0 && !IsPointerOverUI()){
            ScrollDriection = vector.y;
        }
    }
    private void OnClick(InputAction.CallbackContext callbackContext){
        Vector2 position = Pointer.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(position);
        if (draggingCamera == true || !IsPointerOverUI())
        {
            draggingCamera = callbackContext.ReadValueAsButton();
            if (draggingCamera) dragOrigin = _Camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    public bool IsPointerOverUI()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        PointerEventData pointerEventData = new(EventSystem.current);
        pointerEventData.position = mousePosition;
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerEventData, results);
        if (results.Count > 0)
        {
            foreach (RaycastResult result in results)
            {
                int checkmask = 1 << result.gameObject.layer;
                if (checkmask != ignoremask)
                {
                    return true;
                }
            }
        }
        return false;
    }
    void OnDestroy(){
        _inAct.Player.OnZoom.performed -= OnScroll;
        _inAct.Player.Fire.performed -=OnClick;
    //_inAct.Player.Fire.canceled -= OnClick;
        _inAct.Player.OnZoom.Disable();
        _inAct.Player.Fire.Disable();
    }
    void Update(){
        // scroll
        float NextFov = Mathf.Clamp(_Camera.orthographicSize - (ScrollDriection * FovSensitivity),MaxZoom,MinZoom);
        _Camera.orthographicSize = NextFov;
        ScrollDriection = 0;
        // end scroll, start moving
        difference = _Camera.ScreenToWorldPoint(Input.mousePosition)-_Camera.transform.position;
        if (draggingCamera){
            float moveX = dragOrigin.x - difference.x;
            float moveY = dragOrigin.y - difference.y;
            Vector3 targetvector = new Vector3(Mathf.Clamp(moveX,LowerLimits.x,UpperLimits.x),Mathf.Clamp(moveY,LowerLimits.x,UpperLimits.y),_Camera.transform.position.z);
            _Camera.transform.position = Vector3.Lerp(_Camera.transform.position,targetvector,Time.deltaTime*(MoveSpeed/_Camera.orthographicSize));
        }
    }
}
