using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public abstract class TrapMovement : MonoBehaviour {
    [SerializeField] protected PairedValue<float> _durationDiapasone;
    [SerializeField] protected PairedValue<float> _offset;
    [SerializeField] protected Transform _pivot;
    [SerializeField] protected Vector3 _xOffsetVector;

    
    protected LevelBounds _levelBounds;

    [Inject] 
    public void Init(LevelBounds levelBounds) {
        _levelBounds = levelBounds;
    }

    
    
    public abstract void StartMove();
    
    
    protected Ease GetRandomEase() {
        Ease[] eases =
        {
            Ease.InOutSine,
            Ease.InOutQuad,
            Ease.InOutCubic,
            Ease.InOutBack,
            Ease.InOutCirc
        };
        return eases[Random.Range(0, eases.Length)];
    }


    protected float GetRandomDurationDiapasone() {
        return Random.Range(_durationDiapasone.From, _durationDiapasone.To);
    }
    
}
