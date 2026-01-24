using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

[Serializable]
public struct MoveInfo {
    public Vector3 Target;
    public float Duration;
    public Ease Ease;
}

public class BombTrap : MonoBehaviour {
    
    [Header("Explosion settings")]
    [SerializeField] private float _radius = 6f;
    [SerializeField] private float _force = 800f;
    [SerializeField] private float _upwardModifier = 1.5f;
    [SerializeField] private ParticleSystem _particleSystem;
    
    
    [SerializeField] private PairedValue<float> _yMovePoints;
    [SerializeField] private PairedValue<float> _xMovePoints;
    
    [SerializeField] private PairedValue<float> _durationDiapasone;
    
    

    
    
    // == ПА ПРИКОЛУ ДЛЯ ТЕСТА
    [SerializeField] private Renderer _bombRenderer; 
    [SerializeField] private float _brightnessMultiply = 5f;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private Material _triggerObjectMat;
    private LevelBounds _levelBounds;
    private BoostSpawner _boostSpawner;
    
    private Rigidbody _playerRb;
    public List<MoveInfo> _moveInfos;
    
    private Ease _ease1;
    private Ease _ease2;


    [Inject]
    public void Init(LevelBounds levelBounds, BoostSpawner boostSpawner) {
        _levelBounds = levelBounds;
        _boostSpawner = boostSpawner;
    }


    
    private void Awake() {
        SetRandomEase();
        
        
        _triggerObjectMat = _bombRenderer.GetComponent<Renderer>().material;
        _triggerObjectMat.EnableKeyword("_EMISSION");
        SetGreen();
    }
    
    
    private void OnTriggerEnter(Collider collider) {
        // Debug.Log("OnTriggerEnter!");
        if (collider.TryGetComponent(out PlayerMovement movement)) {
            if(!movement.ObjectGetAllow) return;
            _particleSystem.Play();
            if (movement.TryToKill()) {
                Debug.Log("Killed");
                Explode(collider.GetComponent<Rigidbody>());
                movement.SetPlayerIsBombed();
                return;
            }

            _boostSpawner.SetPlayerToNextRightBooster(movement.transform.position);
            Debug.Log("Вы пережили бомбу!");
        }
        // Если бот еще одна логика
        if (collider.TryGetComponent(out BotStateManager bot)) {
            _particleSystem.Play();
            Explode(collider.GetComponent<Rigidbody>());
            bot.PlayerInSpawn();
        }
        // gameObject.DisactiveSelf();
    }



    private void SetRandomEase() {
        Ease[] eases =
        {
            Ease.InOutSine,
            Ease.InOutQuad,
            Ease.InOutCubic,
            Ease.InOutBack,
            Ease.InOutCirc
        };

        _ease1 = eases[Random.Range(0, eases.Length)];
        _ease2 = eases[Random.Range(0, eases.Length)];
    }

    private void Explode(Rigidbody rb) {
        rb.linearVelocity = Vector3.zero;
        rb.AddExplosionForce(
            _force,
            transform.position,
            _radius,
            _upwardModifier,
            ForceMode.Impulse
        );
    }

    private Vector3 startPos; 
    public void SetMovable() {
        startPos = transform.position;
        
        if (Random.value < 0.5f) {
            SetRed();
            // Near Boost
            if(Random.value < 0.5) SetUpDownTrajectory();
            else SetLeftRightTrajectory();
            return;
        }
        // Floor or Wall
        if (Random.value < 0.5) SetFloorUpTrajectory();
        else SetWallTrajectory();
    }

    private void SetUpDownTrajectory() {
        // Debug.Log("SetUpDownTrajectory");
        float duration1 = Random.Range(_durationDiapasone.From, _durationDiapasone.To);
        float duration2 = Random.Range(_durationDiapasone.From, _durationDiapasone.To);
        
        
        float offset1 = Random.Range(_yMovePoints.From, _yMovePoints.To);
        // float offset2 = Random.Range(_yMovePoints.From, _yMovePoints.To);
        
        DOTween.Sequence()
            .Append(
                transform.DOMoveY(startPos.y + offset1, duration1)
                .SetEase(_ease1)
            )
            .Append(
                transform.DOMoveY(startPos.y - offset1, duration2)
                .SetEase(_ease2)
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);

    }
    
    private void SetLeftRightTrajectory() {
        // Debug.Log("SetLeftRightTrajectory");
        float duration1 = Random.Range(_durationDiapasone.From, _durationDiapasone.To);
        float duration2 = Random.Range(_durationDiapasone.From, _durationDiapasone.To);

        float offset1 = Random.Range(_xMovePoints.From, _xMovePoints.To);
        // float offset2 = Random.Range(_xMovePoints.From, _xMovePoints.To);
        
        DOTween.Sequence()
            .Append(
                transform.DOMoveX(startPos.x + offset1, duration1)
                    .SetEase(_ease1)
            )
            .Append(
                transform.DOMoveX(startPos.x - offset1, duration2)
                    .SetEase(_ease2)
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }
    
    
    private void SetWallTrajectory() {
        SetYellow();
        // Debug.Log("SetLeftRightTrajectory");
        float duration = Random.Range(_durationDiapasone.From, _durationDiapasone.To);
        float x = _levelBounds.LeftX;
        if(Random.value < 0.5f) x =  _levelBounds.RightX;
        
        DOTween.Sequence()   
            .Append(
                transform.DOMoveX(x, duration)
                    .SetEase(_ease1)
            )
            .Append(
                transform.DOMoveX(startPos.x, duration)
                    .SetEase(_ease2)
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }
    
    
    private void SetFloorUpTrajectory() {
        SetBlue();
        // Debug.Log("SetUpDownTrajectory");
        float duration1 = Random.Range(_durationDiapasone.From, _durationDiapasone.To);
        float duration2 = Random.Range(_durationDiapasone.From, _durationDiapasone.To);
        
        
        float offset1 = Random.Range(_yMovePoints.From, _yMovePoints.To) * 3;
        // float offset2 = Random.Range(_yMovePoints.From, _yMovePoints.To);
        
        DOTween.Sequence()
            .Append(
                transform.DOMoveY(startPos.y + offset1, duration1/2)
                    .SetEase(_ease1)
            )
            .Append(
                transform.DOMoveY(startPos.y - offset1, duration2/2)
                    .SetEase(_ease2)
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);

    }
    
    
    
    
    private void SetRed() {
        Color emission = Color.red * _brightnessMultiply; // множитель яркости
        _triggerObjectMat.SetColor(EmissionColor, emission);
        // Debug.Log("Установка красным");
    }
    
    private void SetGreen() {
        Color emission = Color.green * _brightnessMultiply; // множитель яркости
        _triggerObjectMat.SetColor(EmissionColor, emission);
        // Debug.Log("Установка зеленым");
    } 
    
    private void SetBlue() {
        Color emission = Color.blue * _brightnessMultiply; // множитель яркости
        _triggerObjectMat.SetColor(EmissionColor, emission);
        // Debug.Log("Установка зеленым");
    }
    
    
    private void SetYellow() {
        Color emission = Color.yellow * _brightnessMultiply; // множитель яркости
        _triggerObjectMat.SetColor(EmissionColor, emission);
        // Debug.Log("Установка зеленым");
    }


    
}
