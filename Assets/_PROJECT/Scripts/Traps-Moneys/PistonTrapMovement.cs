using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class PistonTrapMovement : TrapMovement {
    
    
    
    public override void StartMove() {
        _pivot.localPosition = Vector3.zero;
        _pivot.position += _xOffsetVector;
        
        if (Mathf.Approximately(transform.position.x, _levelBounds.RightX)) {
            _pivot.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else {
            _pivot.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        
        
        float bitePoint = (_levelBounds.RightX + _levelBounds.LeftX) / 2f;
        SetPistonTrajectory(bitePoint);
    }

    public override void ResetTrap() {
        _oscillateTween?.Kill();
    }

    private Tween _oscillateTween;

    private void SetPistonTrajectory(float bitePoint) {
        
        Ease ease = GetRandomEase();
        float duration = GetRandomDurationDiapasone();
        
        
        _oscillateTween = _pivot.transform.
            DOMoveX(bitePoint, duration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }
}
