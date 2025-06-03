using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsMenuOpenBehaivor : MonoBehaviour
{
    [SerializeField] private GameObject ToToggle;
    private InputSys inputActions;
    public void OnActivate(InputAction.CallbackContext cb)
    {
        if (ToToggle != null)
        {
            Time.timeScale = 0f;
            if (ToToggle.activeInHierarchy)
            {
                Time.timeScale = 1f;
            }
            ToToggle.SetActive(!ToToggle.activeInHierarchy);
        }
    }
    public void Start()
    {
        inputActions = new();
        inputActions.Player.OpenSettingsMenu.performed += OnActivate;
        inputActions.Player.OpenSettingsMenu.Enable();
    }
    public void OnDestroy()

    {
        inputActions.Player.OpenSettingsMenu.performed -= OnActivate;
        inputActions.Player.OpenSettingsMenu.Disable();
    }
}
