using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class UIToggle : MonoBehaviour
{
    [SerializeField] protected InputAction OpeningKey;
    [SerializeField] protected GameObject ToToggle;
    protected InputSys inputActions;
    public virtual void OnActivate(InputAction.CallbackContext cb)
    {
        if (ToToggle != null)
        {
            switch (ToToggle.activeInHierarchy)
            {
                case true:
                    ToToggle.SetActive(false);
                    break;
                case false:
                    ToToggle.SetActive(true);
                    break;
            }
        }
    }
    public virtual void Start()
    {
        OpeningKey.performed += OnActivate;
    }
    public virtual void OnDestroy()
    {
        inputActions = new();
        OpeningKey.performed += OnActivate;
    }
}
