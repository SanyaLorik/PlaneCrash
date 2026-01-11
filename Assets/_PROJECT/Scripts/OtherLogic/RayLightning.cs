using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SanyaBeerExtension;
using Random = UnityEngine.Random;

public class NewMonoBehaviourScript : MonoBehaviour { 
    [SerializeField] private SpriteRenderer[] _rays;
    [SerializeField] private PairedValue<float> _duration;

    private void Start() {
        foreach (var ray in _rays) {
            float duration = Random.Range(_duration.From, _duration.To);
            float startOffset = Random.Range(0f, duration);

            ray.DOFade(0.1f, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad)
                .SetLink(ray.gameObject) 
                .Goto(startOffset, true);
            
            ray.transform.DOScaleY(ray.transform.localScale.x * 1.2f, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetLink(ray.gameObject)
                .Goto(startOffset, true);
        }
    }


}
