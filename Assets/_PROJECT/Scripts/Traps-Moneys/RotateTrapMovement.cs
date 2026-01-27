using System;
using DG.Tweening;
using UnityEngine;

public class RotateTrapMovement : TrapMovement {
    [SerializeField] private Transform _pivot;
    [SerializeField] private float _duration;
    [SerializeField] private Vector3 _rotatingVector;
    
    
    private void Start() {
        StartMove();
    }


    public override void StartMove() {
        _pivot.transform
            .DORotate(_rotatingVector, _duration, RotateMode.LocalAxisAdd)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear)
            .SetLink(gameObject);
    }

}
