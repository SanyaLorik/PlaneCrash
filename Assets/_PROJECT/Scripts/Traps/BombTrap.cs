using System;
using System.Collections.Generic;
using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
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

    private Rigidbody _playerRb;
    public List<MoveInfo> _moveInfos;
    
    private Ease _ease1;
    private Ease _ease2;
    
    private void OnTriggerEnter(Collider collider) {
        Debug.Log("OnTriggerEnter!");
        if (collider.TryGetComponent(out PlayerMovement movement)) {
            _particleSystem.Play();
            Explode(collider.GetComponent<Rigidbody>());
            movement.SetPlayerIsBombed();
        }
        // Если бот еще одна логика
        if (collider.TryGetComponent(out BotStateManager bot)) {
            _particleSystem.Play();
            Explode(collider.GetComponent<Rigidbody>());
            bot.PlayerInSpawn();
        }
        // gameObject.DisactiveSelf();
    }

    private void Awake() {
        SetRandomEase();
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
            SetUpDownTrajectory();
            return;
        }

        SetLeftRightTrajectory();
    }

    private void SetUpDownTrajectory() {
        float duration1 = Random.Range(_durationDiapasone.From, _durationDiapasone.To);
        float duration2 = Random.Range(_durationDiapasone.From, _durationDiapasone.To);
        
        DOTween.Sequence()
            .Append(
                transform.DOMoveY(startPos.y + _yMovePoints.From, duration1)
                .SetEase(_ease1)
            )
            .Append(
                transform.DOMoveY(startPos.y + _yMovePoints.To, duration2)
                .SetEase(_ease2)
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);

    }
    
    private void SetLeftRightTrajectory() {
        float duration1 = Random.Range(_durationDiapasone.From, _durationDiapasone.To);
        float duration2 = Random.Range(_durationDiapasone.From, _durationDiapasone.To);

        DOTween.Sequence()
            .Append(
                transform.DOMoveX(startPos.x + _xMovePoints.From, duration1)
                    .SetEase(_ease1)
            )
            .Append(
                transform.DOMoveX(startPos.x + _xMovePoints.To, duration2)
                    .SetEase(_ease2)
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }
    
    


}
