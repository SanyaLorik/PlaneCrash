using System;
using DG.Tweening;
using UnityEngine;

public class RotateTrapMovement : TrapMovement {
    [SerializeField] private float _duration;
    [SerializeField] private Vector3 _rotatingVector;
    
    
    private Tween _oscillateTween;

    public override void StartMove() {
        _pivot.localPosition = Vector3.zero;
        _pivot.position += _xOffsetVector;
        
        // Слева
        if (transform.position.x > 0) {
            _rotatingVector *= -1;
        }
        // Debug.Log("Вектор вращения: " +  _rotatingVector + " z = " +  transform.position.x );
        
        
        _oscillateTween = _pivot.transform
            .DORotate(_rotatingVector, _duration, RotateMode.LocalAxisAdd)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear)
            .SetLink(gameObject);
    }

    public override void ResetTrap() {
        _oscillateTween?.Kill();
    }
}
