using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class PistonTrapMovement : TrapMovement {
    
    
    
    public override void StartMove() {
        _pivot.localPosition = Vector3.zero;
        _pivot.position += _xOffsetVector;
        
        if (transform.position.x > 0) {
            _pivot.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        float bitePoint = (_levelBounds.RightX + _levelBounds.LeftX) / 2f;
        SetPistonTrajectory(bitePoint);
    }
    

    
    
    
    private void SetPistonTrajectory(float bitePoint) {
        
        Ease ease = GetRandomEase();
        float duration = GetRandomDurationDiapasone();
        
        
        DOTween.Sequence()   
            .Append(
                _pivot.transform.DOMoveX(bitePoint, duration)
                    .SetEase(ease)
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }
}
