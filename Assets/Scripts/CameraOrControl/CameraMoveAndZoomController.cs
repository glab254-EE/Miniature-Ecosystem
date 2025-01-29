using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private Vector3 resetPosition;
    private Vector3 difference;
    private Vector3 dragOrigin;
    private Camera Camera;
    private InputSys _inAct;
    float ScrollDriection;
    bool dragging = false;
    void Awake(){
        Camera = Camera.main;
        resetPosition = Camera.transform.position;
        ScrollDriection = 0f;
        _inAct = new();
        _inAct.Player.OnZoom.performed += OnScroll;
        _inAct.Player.Fire.performed +=OnClick;
        //_inAct.Player.Fire.canceled += OnClick;
        _inAct.Player.Fire.Enable();

        _inAct.Player.OnZoom.Enable();
    }
    private void OnScroll(InputAction.CallbackContext callbackContext){
        Vector2 vector = callbackContext.ReadValue<Vector2>();
        if (vector.y != 0){
            ScrollDriection = vector.y;
        }
    }
    private void OnClick(InputAction.CallbackContext callbackContext){
        dragging = callbackContext.ReadValueAsButton();
        if (dragging) dragOrigin = Camera.ScreenToWorldPoint(Input.mousePosition);
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
        float NextFov = Mathf.Clamp(Camera.orthographicSize - (ScrollDriection * FovSensitivity),MaxZoom,MinZoom);
        Camera.orthographicSize = NextFov;
        ScrollDriection = 0;
        // end scroll, start moving
        difference = Camera.ScreenToWorldPoint(Input.mousePosition)-Camera.transform.position;
        if (dragging){
            float moveX = dragOrigin.x - difference.x;
            float moveY = dragOrigin.y - difference.y;
            Vector3 targetvector = new Vector3(Mathf.Clamp(moveX,LowerLimits.x,UpperLimits.x),Mathf.Clamp(moveY,LowerLimits.x,UpperLimits.y),Camera.transform.position.z);
            Camera.transform.position = Vector3.Lerp(Camera.transform.position,targetvector,Time.deltaTime*(MoveSpeed/Camera.orthographicSize));
        }
    }
}
