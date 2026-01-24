using System;
using System.Net.Http.Headers;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class TrapPositionCalculator : MonoBehaviour {
    [Header("Смещение от буста от лева до права")]
    [SerializeField] private PairedValue<float> _distanceX;
    [SerializeField] private PairedValue<float> _distanceY;
    [SerializeField] private PairedValue<float> _distanceZ;
    
    
    private LevelBounds _levelBounds;

    [Inject]
    public void Init(LevelBounds levelBounds) {
        _levelBounds  = levelBounds;
    }



    public Vector3 GetNearBoostPosition(Vector3 boost) {
        Vector3 position = new Vector3(
            boost.x + GetRandomOffset(_distanceX),
            boost.y + GetRandomOffset(_distanceY), 
            boost.z + GetPositiveOffset(_distanceZ)
        );
        position = _levelBounds.ClampPosition(position);
        return position;
    }
    
    public Vector3 GetInBoostPosition(Vector3 boost) {
        Vector3 position = new Vector3(
            boost.x, // чуть лэва права похуй
            boost.y,
            boost.z - GetPositiveOffset(_distanceZ)  // перед бустом)))
        );
        position = _levelBounds.ClampPosition(position);
        return position;
    }

    private float GetRandomOffset(PairedValue<float> offset) {
        float resultOffset = Random.Range(offset.From, offset.To);
        return Random.value > 0.5f ? resultOffset : - resultOffset;
    }
    
    private float GetPositiveOffset(PairedValue<float> offset) 
        => Random.Range(offset.From, offset.To);

    
}
