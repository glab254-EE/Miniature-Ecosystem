using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using Unity.Mathematics;

public class AnimalBehaivor : MonoBehaviour
{
    [Header("Set Up")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] internal AnimalsSO animalSO;
    [Header("Settings-Movement")]
    [SerializeField] private float randomSpeedDivider = 10f; // used for a bit of randomness of movement.
    [SerializeField] private float AnimationSpeed = 1f;
    [Header("Settings-Animation")]
    [SerializeField] private List<Sprite> AnimationSprites;
    [SerializeField] private float AnimationStepTime = 0.25f;
    [SerializeField] private float AnimationSizeAdditionX = 1f;
    [SerializeField] private float AnimationSizeAdditionY = 1f;
    internal bool Dragged;
    private Vector2 _minMovementBounds;
    private Vector2 _maxMovementBounds;
    private bool _moving;
    int _currentPhase; // -1: dead, 0: standing / movement finished, 1:  movement

    float _phaseTick;
    Vector2 _startingScaleOrSize;
    float _animationBretheTick = 0f;
    float _animationTick = 0f;
    int _currentSprite;
    bool _loweringAnimation = false;
    private void AnimationNextSprite()
    {
        if (AnimationSprites.Count >= 2)
        {
            _currentSprite++;
            if (_currentSprite >= AnimationSprites.Count)
            {
                _currentSprite = 0;
            }
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = AnimationSprites[_currentSprite];
            }
            _animationTick = AnimationStepTime;
        } 
    }
    private Vector2 GetRandomPosVector(Vector2 min, Vector2 max)
    {

        Vector2 output = min;
        try
        {
            output = new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y)); // gets random vector
        }
        catch (System.Exception)
        {
            Debug.Log("Could not get random vector for " + gameObject.name + ". Returning minimum.");
        }
        return output;
    }

    void Start()
    {
        if (animalSO.MinAnimalSize > 0 && animalSO.MaxAnimalSize > 0)
        {
            float newsize = UnityEngine.Random.Range(animalSO.MinAnimalSize, animalSO.MaxAnimalSize);
            transform.localScale = new Vector3(newsize, newsize, newsize);
        }
        _animationTick = AnimationStepTime;
        _minMovementBounds = animalSO.MinMovementBounds;
        _maxMovementBounds = animalSO.MaxMovementBounds;
        _moving = false;
        _phaseTick = 0;
        if (spriteRenderer == null)
        {
            gameObject.TryGetComponent<SpriteRenderer>(out spriteRenderer);
        }
        if (rb == null)
        {
            gameObject.TryGetComponent<Rigidbody2D>(out rb);
        }
        transform.position = GetRandomPosVector(_minMovementBounds, _maxMovementBounds);
        _currentPhase = 0;
        if (animalSO.BaseSprite != null)
        {
            spriteRenderer.sprite = animalSO.BaseSprite;
        }
        if (animalSO.AnimationSprites != null)
        {
            AnimationSprites = animalSO.AnimationSprites;
            if (AnimationSprites.Count >= 1)
            {
                spriteRenderer.sprite = animalSO.AnimationSprites[0];
            }
        }
        _startingScaleOrSize = new Vector2(transform.localScale.x, transform.localScale.y);
        float _PosZ = UnityEngine.Random.Range(transform.position.z - 0.15f, transform.position.z + 0.15f);
        transform.position = new Vector3(transform.position.x, transform.position.y, _PosZ);
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
        if (animalSO != null )
        { // checks if being is NOT plant.
            if (_currentPhase == 0 && animalSO.MovementDelay >= 0)
            {
                if (_phaseTick >= animalSO.MovementDelay)
                {
                    _phaseTick = 0;
                    _currentPhase = 1;
                }
                else
                {
                    _phaseTick += Time.deltaTime;
                }
            }
            else if (!_moving)
            {
                StartCoroutine(RandomMovement());
            }
        }
        _animationTick -= Time.deltaTime;
        if (_animationTick <= 0)
        {
            AnimationNextSprite();
        }
        if (!_loweringAnimation && _animationBretheTick < 1)
        {
            _animationBretheTick += Time.deltaTime * AnimationSpeed;
            if (_animationBretheTick >= 1) _loweringAnimation = true;
        }
        else if (_loweringAnimation && _animationBretheTick > 0)
        {
            _animationBretheTick -= Time.deltaTime * AnimationSpeed;
            if (_animationBretheTick <= 0) _loweringAnimation = false;
        }
        Vector3 NewTargetScale = new Vector3(Mathf.Lerp(_startingScaleOrSize.x, _startingScaleOrSize.x + AnimationSizeAdditionX, _animationBretheTick), Mathf.Lerp(_startingScaleOrSize.y, _startingScaleOrSize.y + AnimationSizeAdditionY, _animationBretheTick), transform.localScale.z);
        transform.localScale = NewTargetScale;
    }
}