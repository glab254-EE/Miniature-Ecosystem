using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialHandler : MonoBehaviour
{
    [SerializeField] private Public_Data dataReference;
    [SerializeField] private Button skipbutton;
    [SerializeField] private List<GameObject> slides;
    private InputSys _inAct;
    int _slide;
    bool _tutorialCompleted;
    void Awake()
    {
        dataReference = Public_Data.instance;
        _inAct = new();
        _slide = 0;
    }
    void OnSKIP()
    {
        gameObject.SetActive(false);
        dataReference = Public_Data.instance;
        dataReference.Data.TutorialCompleted = true;   
        _inAct.Player.Fire.performed -= NextSlide;     
    }
    private void Start()
    {
        dataReference = Public_Data.instance;
        if (dataReference == null)
        { // failsafe, idk how to trytofind any object
            Destroy(gameObject);
        }
        else
        {
            if (dataReference.Data != null)
            {
                _tutorialCompleted = dataReference.Data.TutorialCompleted;
            }
            else
            {
                _tutorialCompleted = false;
            }
        }

        Debug.Log("Starting tutorial!");
        if (_tutorialCompleted)
        {
            Debug.Log("Nvm, ending early!");
            gameObject.SetActive(false);
            return;
        }
        _inAct.Player.Fire.performed += NextSlide;
        _inAct.Player.Fire.Enable();
        slides[_slide].SetActive(true);
        skipbutton.onClick.AddListener(OnSKIP);
    }
    private void OnDestroy(){
        _inAct.Player.Fire.performed -= NextSlide;
    }
    void NextSlide(InputAction.CallbackContext _){
        if (_.ReadValueAsButton() == true){
            slides[_slide].SetActive(false);
            if (_slide + 1 >= slides.Count){
                gameObject.SetActive(false);
                dataReference = Public_Data.instance;
                dataReference.Data.TutorialCompleted = true;
            }
            else 
            {
                _slide++;
                slides[_slide].SetActive(true);
            }
        }
    }
}
