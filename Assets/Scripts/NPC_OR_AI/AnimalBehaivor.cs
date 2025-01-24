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
    [SerializeField] private Vector2 MovementBounds;
    [Header("Settings")]
    [SerializeField] private BeingType animalType = BeingType.Animal_Herbinoves;
    [SerializeField] private bool keepVertical = false;
    [SerializeField] private float speed = 1;
    [SerializeField] private float TurnDuration = 0.5f;
    [SerializeField] private float movementDelay = 2;
    [SerializeField] private float hungerRate = 0.5f/60; // rate or ammount of hunger given per tick/delta time
    [SerializeField] private float maxHunger = 100;
    [SerializeField] private float hungerDamage = 100;
    [SerializeField] private float hungerReducePerBite = 50;
    [SerializeField] private float _hunger; // serialize for testing.
    private Vector2 _minMovementBounds;
    private Vector2 _maxMovementBounds;
    private bool _moving;
    internal bool _dead;
   int _currentPhase; // -1: dead, 0: standing / movement finished, 1:  movement, 2: hunting/fleeing.

    float _phaseTick;
    private Vector2 GetRandomPosVector(Vector2 min, Vector2 max){
        
        Vector2 output = min;
        try{
            output = new Vector2(UnityEngine.Random.Range(min.x,max.x),UnityEngine.Random.Range(min.y,max.y)); // gets random vector
        }catch(System.Exception){
            Debug.Log("Could not get random vector for "+gameObject.name+". Returning minimum.");
        }
        return output;
    }
    private Transform FindClosestHuntable(){
        Transform output = null;
        float closestdistance = -1;
        foreach(AnimalBehaivor animal in FindObjectsOfType<AnimalBehaivor>()){
            if ((animalType == BeingType.Animal_Predators&&animal.animalType != BeingType.Plant)||(animalType == BeingType.Animal_Herbinoves&&animal.animalType == BeingType.Plant)){
                float distance = (transform.position-animal.transform.position).magnitude;
                if ((closestdistance == -1||(distance < closestdistance)) && animal.gameObject != this.gameObject){
                    closestdistance = distance;
                    output = animal.transform;
                }
            }
        }
        return output;
    }
    void Start()
    {
        _minMovementBounds = -MovementBounds;
        _maxMovementBounds = MovementBounds;
        _moving = false;
        _dead = false;  
        _phaseTick = 0; 
        if (spriteRenderer == null){
            gameObject.TryGetComponent<SpriteRenderer>(out spriteRenderer);
        }
        if (rb==null){
            gameObject.TryGetComponent<Rigidbody2D>(out rb);
        }  
        _hunger = maxHunger;
        transform.position = GetRandomPosVector(_minMovementBounds,_maxMovementBounds);
        _currentPhase = 0;
    }
    private void RotateToTarget(Vector2 target){
        Vector2 vectorToTarget = target - (Vector2)transform.position;
        if (!keepVertical){
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.DORotateQuaternion(q,TurnDuration).Play();
        } else if (keepVertical && spriteRenderer != null){
            if (vectorToTarget.x >= 0){
                spriteRenderer.flipX = false;
            } else {
                spriteRenderer.flipX = true;
            }
        }
    }
    private void RotateToTarget(Transform target){
        Vector3 vectorToTarget = target.transform.position - transform.position;
        if (!keepVertical){
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.DORotateQuaternion(q,TurnDuration).Play();
        } else if (keepVertical && spriteRenderer != null){
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
            float desiredtime = Vector2.Distance((Vector2)transform.position,targetpos)/speed;
            currenttween = rb.DOMove(targetpos,desiredtime);
            RotateToTarget(targetpos);
            currenttween.Play();
            yield return currenttween.WaitForCompletion();
            _currentPhase = 0;
            _moving = false;
        }
    }
    private IEnumerator Hunt(){
        _moving = true;
        Tween currenttween = null;
        Vector3 oldpos = Vector3.zero;
        while (_moving && _currentPhase == 2){
            Transform targetpos = FindClosestHuntable(); // finds huntable being
            if (currenttween != null){
                currenttween.Kill();
            }
           if (targetpos != null && (oldpos == Vector3.zero || oldpos != targetpos.position)){
                oldpos = targetpos.position;
                Vector2 convertedtargetpos = oldpos;
                float desiredtime = Vector2.Distance((Vector2)transform.position,targetpos.position)/speed;
                RotateToTarget(targetpos);
                currenttween = rb.DOMove((Vector2)oldpos,desiredtime);
                currenttween.Play(); // runs towards it
            }
            yield return new WaitForSeconds(0.15f); // to not crash it...
        }
        _currentPhase = 0;
    }
    void Update()
    {
        if (_currentPhase == -1 || _dead){
            if (!_dead){
                _dead = true;
                _phaseTick = 0;
            }
            _phaseTick+=Time.deltaTime;
            if (_phaseTick>=5){
                spriteRenderer.DOColor(new Color(255,255,255,0),5).Play(); // fades out in 5 secs, destroying it.
                _currentPhase = -1;
                Destroy(gameObject,5);
            }
        } else if (animalType != BeingType.Plant && !_dead){ // checks if being is NOT plant.
            float newHunger = Mathf.Clamp(_hunger-Time.deltaTime*hungerRate,0,120);
            _hunger=newHunger;
            if (_currentPhase == 0 && movementDelay >= 0){
                if (_phaseTick >= movementDelay){
                    _phaseTick = 0;
                    _currentPhase = _hunger > 50 ? 1 : 2;
                } else {
                    _phaseTick+= Time.deltaTime;
                }
            } else if (_currentPhase == 1 && !_moving){
                StartCoroutine(RandomMovement());
            } else if (_currentPhase == 2 && !_moving){
                StartCoroutine(Hunt());
            }
        } else { // plant based animal.
            if (!_dead){
            float newHunger = Mathf.Clamp(_hunger+Time.deltaTime*hungerRate,0,120);
            _hunger=newHunger;
            }
        }
        if (_hunger<=0&&_currentPhase != -1){ // death by hunger
            _currentPhase = -1;
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {        
        if (col.gameObject.TryGetComponent<AnimalBehaivor>(out AnimalBehaivor behaivor) && _currentPhase == 2){
            if ((animalType == BeingType.Animal_Predators && behaivor.animalType == BeingType.Animal_Herbinoves)||(animalType == BeingType.Animal_Herbinoves && behaivor.animalType == BeingType.Plant)){
                behaivor._hunger -= hungerDamage;
                if (behaivor._hunger - hungerReducePerBite <= 0){
                    _hunger += behaivor._hunger;
                } else {
                _hunger += hungerReducePerBite;
                }
                _currentPhase = 0;
                _phaseTick = 0;
            }
        }
    }
}