using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButonNavigationBehaivor : MonoBehaviour
{
    [SerializeField] private GameObject Next;
    [SerializeField] private GameObject Current;
    void OnPress()
    {
        if (Current != null)
        {
            Current.SetActive(false);
        }
        if (Next != null)
        {
            Next.SetActive(true);
        }
    }
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnPress);
    }
}
