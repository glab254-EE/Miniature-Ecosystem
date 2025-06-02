using System.Collections;
using System.Collections.Generic;
using System.Data;
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
        _inAct = new();
        _slide = 0;
    }
    void OnSKIP()
    {
        gameObject.SetActive(false);
        dataReference.Data.TutorialCompleted = true;   
        _inAct.Player.Fire.performed -= NextSlide;     
    }
    void Start()
    {
        if (dataReference == null)
        { // failsafe, idk how to trytofind any object
            Destroy(gameObject);
        }
        else
        {
            _tutorialCompleted = dataReference.Data.TutorialCompleted;
        }
        if (!_tutorialCompleted)
        {
            _inAct.Player.Fire.performed += NextSlide;
            _inAct.Player.Fire.Enable();
            slides[_slide].SetActive(true);
            if (skipbutton != null) skipbutton.onClick.AddListener(OnSKIP);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private void OnDestroy(){
        _inAct.Player.Fire.performed -= NextSlide;
    }
    void NextSlide(InputAction.CallbackContext _){
        if (_.ReadValueAsButton() == true){
            slides[_slide].SetActive(false);
            if (_slide + 1 >= slides.Count){
                gameObject.SetActive(false);
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
