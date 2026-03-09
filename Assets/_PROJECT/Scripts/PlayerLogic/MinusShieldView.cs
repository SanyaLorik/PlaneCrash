using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MinusShieldView : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _shieldCountText;
    [SerializeField] private float _upOffset;
    [SerializeField] private float _animationDuration;
    [field: SerializeField] public RectTransform RectTransform { get; private set; }
    [SerializeField] private CanvasGroup _container;

    public void SetCount(int count) {
        _shieldCountText.text = count.ToString();
        MoneyAnimationRoutine();
    }

    private void MoneyAnimationRoutine() {
        float targetY = RectTransform.anchoredPosition.y + _upOffset;
        Sequence sequence = DOTween.Sequence();

        sequence
            .Append(RectTransform.DOAnchorPosY(targetY, _animationDuration))
            .Join(_container.DOFade(0f, _animationDuration))
            .OnComplete(() => {
                Destroy(gameObject);
            });
    }
    
    
    
}