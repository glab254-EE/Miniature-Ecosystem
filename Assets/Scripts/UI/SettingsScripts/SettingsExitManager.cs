using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsExitManager : MonoBehaviour
{
    [SerializeField] private GameObject Current;
    void OnPress()
    {
        if (Current != null)
        {
            Current.SetActive(false);
            Time.timeScale = 1;
        }
    }
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnPress);
    }
}
