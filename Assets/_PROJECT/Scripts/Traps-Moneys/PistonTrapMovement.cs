using System;
using DG.Tweening;
using Random = UnityEngine.Random;

public class PistonTrapMovement : TrapMovement {
    public override void StartMove() {
        float wallX = Random.value < 0.5f ? 
            _levelBounds.RightX : 
            _levelBounds.LeftX;
        
        SetPistonTrajectory(wallX);
    }
    
    
    private void SetPistonTrajectory(float wallX) {
        float endX = transform.position.x;
        Ease ease = GetRandomEase();
        float duration = GetRandomDurationDiapasone();
        
        DOTween.Sequence()   
            .Append(
                transform.DOMoveX(wallX, duration)
                    .SetEase(ease)
            )
            .Append(
                transform.DOMoveX(endX, duration)
                    .SetEase(ease)
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }
}
