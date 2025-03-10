using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialHandler : MonoBehaviour
{
    [SerializeField] private Public_Data dataReference;
    [SerializeField] private List<GameObject> slides;
    private InputSys _inAct;
    int _slide;
    bool _tutorialCompleted;
    void Awake(){
        _inAct = new();
        _slide = 0;
    }
    void Start()
    {
        if (dataReference==null){ // failsafe, idk how to trytofind any object
            Destroy(gameObject);
        } else {
            _tutorialCompleted = dataReference.Data.TutorialCompleted;
        }
        if (!_tutorialCompleted){
            _inAct.Player.Fire.performed += NextSlide;
            _inAct.Player.Fire.Enable();
            slides[_slide].SetActive(true);
        }
    }
    private void OnDestroy(){
        if (!_tutorialCompleted){
            _inAct.Player.Fire.performed -= NextSlide;
        }
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
