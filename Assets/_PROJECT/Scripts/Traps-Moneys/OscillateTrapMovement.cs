using DG.Tweening;
using UnityEngine;


// Движение лево право / верх низ, как задали
public class OscillateTrapMovement : TrapMovement {
    
    public override void StartMove() {
        Vector3 offsetVector = Random.value < 0.5 ? 
            new Vector3(0f, Random.Range(_offset.From, _offset.To), 0f) : 
            new Vector3(Random.Range(_offset.From, _offset.To), 0f, 0f);
        
        SetOscillateTrajectory(offsetVector);
        
    }

    public override void ResetTrap() {
        _oscillateTween?.Kill();
    }


    
    private Tween _oscillateTween;
    private void SetOscillateTrajectory(Vector3 offset) {
        Vector3 startPos = transform.position;
        int sign = Random.value < 0.5 ? -1 : 1;

        float duration = Random.Range(_durationDiapasone.From, _durationDiapasone.To);
        Ease ease = GetRandomEase();

        _oscillateTween = transform
            .DOMove(startPos + offset * sign, duration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }
}
