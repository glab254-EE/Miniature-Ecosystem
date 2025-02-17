using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using Unity.Mathematics;

public class AnimalBehaivor : MonoBehaviour
{
    [Header("Set Up")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] internal AnimalsSO animalSO;
    [Header("Settings")]
    [SerializeField] private float randomSpeedDivider = 10f; // used for a bit of randomness of movement.
    private Vector2 _minMovementBounds;
    private Vector2 _maxMovementBounds;
    private bool _moving;
   int _currentPhase; // -1: dead, 0: standing / movement finished, 1:  movement

    float _phaseTick;
    private Public_Data moneyStorage;
    private float _gainCooldown;
    private int _resourceType;
    private float _resourceGain;
    float cd;

    private Vector2 GetRandomPosVector(Vector2 min, Vector2 max){
        
        Vector2 output = min;
        try{
            output = new Vector2(UnityEngine.Random.Range(min.x,max.x),UnityEngine.Random.Range(min.y,max.y)); // gets random vector
        }catch(System.Exception){
            Debug.Log("Could not get random vector for "+gameObject.name+". Returning minimum.");
        }
        return output;
    }
   
    void Start()
    {
        if (animalSO.MinAnimalSize > 0 && animalSO.MaxAnimalSize > 0){
            float newsize = UnityEngine.Random.Range(animalSO.MinAnimalSize,animalSO.MaxAnimalSize);
            transform.localScale = new Vector3(newsize,newsize,newsize);
        }
        _minMovementBounds = animalSO.MinMovementBounds;
        _maxMovementBounds = animalSO.MaxMovementBounds;
        _moving = false;
        _phaseTick = 0; 
        if (spriteRenderer == null){
            gameObject.TryGetComponent<SpriteRenderer>(out spriteRenderer);
        }
        if (rb==null){
            gameObject.TryGetComponent<Rigidbody2D>(out rb);
        }  
        transform.position = GetRandomPosVector(_minMovementBounds,_maxMovementBounds);
        _currentPhase = 0;
        if (animalSO.BaseSprite != null) { 
            spriteRenderer.sprite = animalSO.BaseSprite;
        }
        // animal gain set-up
        if (animalSO.GainAmmount > 0){
            _gainCooldown = animalSO.GainDelay;
            _resourceGain = animalSO.GainAmmount;
            _resourceType = animalSO.ResourceID;
            cd = _gainCooldown;
            if (moneyStorage == null){
                moneyStorage = FindObjectOfType<Public_Data>();
                if (moneyStorage == null) Destroy ( gameObject);
            }
            moneyStorage.ChangeGain(_resourceType,_resourceGain/_gainCooldown);
        }
    }
    private void RotateToTarget(Vector2 target){
        Vector2 vectorToTarget = target - (Vector2)transform.position;
        if (!animalSO.KeepVertical){
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.DORotateQuaternion(q,animalSO.TurnDuration).Play();
        } else if (animalSO.KeepVertical && spriteRenderer != null){
            if (vectorToTarget.x >= 0){
                spriteRenderer.flipX = false;
            } else {
                spriteRenderer.flipX = true;
            }
        }
    }
  
    private IEnumerator RandomMovement(){
        _moving = true;
        Vector2 targetpos = GetRandomPosVector(_minMovementBounds,_maxMovementBounds);
        Tween currenttween = null;
        if (targetpos != null){
            float desiredtime = Vector2.Distance((Vector2)transform.position,targetpos)/animalSO.MovementSpeed;
            float tweenTime = desiredtime+UnityEngine.Random.Range(-desiredtime/randomSpeedDivider,desiredtime/randomSpeedDivider);
            currenttween = rb.DOMove(targetpos,desiredtime);
            RotateToTarget(targetpos);
            currenttween.Play();
            yield return currenttween.WaitForCompletion();
            _currentPhase = 0;
            _moving = false;
        }
    }
    void Update()
    {
        if (animalSO != null &&animalSO.BType != BeingType.Plant ){ // checks if being is NOT plant.
            if (_currentPhase == 0 && animalSO.MovementDelay >= 0){
                if (_phaseTick >= animalSO.MovementDelay){
                    _phaseTick = 0;
                    _currentPhase = 1;
                } else {
                    _phaseTick+= Time.deltaTime;
                }
            } else if (!_moving){
                StartCoroutine(RandomMovement());
            }
        } 
        else { // plant based animal.
        }
        cd -= Time.deltaTime;
        if (cd <= 0){
            cd = _gainCooldown;
            moneyStorage.ChangeMoney(_resourceType,_resourceGain);
        }
    }
    void OnDestroy(){
        if (_resourceGain>0){
            moneyStorage.ChangeGain(_resourceType,-_resourceGain/_gainCooldown);
        }
    }
}