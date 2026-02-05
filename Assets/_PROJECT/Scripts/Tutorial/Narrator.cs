using System;
using DG.Tweening;
using JetBrains.Annotations;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Narrator : MonoBehaviour {
    [SerializeField] private TMP_Text _text;
    [SerializeField] private RectTransform _girlImage;
    [SerializeField] private Image _backGround;
    
    
    [SerializeField] private float _durationNarratorShow;
    [SerializeField] private float _durationTextShow;
    
    

    private Vector2 _startPos;
    private Vector2 _hidePos;
    
    private void Awake() {
        // Запоминаем финальную позицию картинки
        _startPos = _girlImage.anchoredPosition;

        // Прячем слева за экран
        _hidePos = new Vector2(
            -Screen.width - 200f,
            _startPos.y
        );

        _girlImage.anchoredPosition = _hidePos;

        // Текст в ноль
        _text.transform.localScale = Vector3.zero;
        _text.alpha = 0f;
        
        HideNarrator();
    }



    public void SetTextWithNarattor(string text) {
        _text.text = text;
        ShowNarrator();
    }
    
    

    private void ShowNarrator() {
        DOTween.Kill(this);

        Sequence seq = DOTween.Sequence();
        seq.SetTarget(this);

        // Картинка летит в позицию
        seq.Append(
            _girlImage.DOAnchorPos(_startPos, _durationNarratorShow)
                .SetEase(Ease.OutCubic)
        );

        // Текст увеличивается
        seq.Join(
            _text.transform
                .DOScale(1f, _durationTextShow)
                .SetEase(Ease.OutBack)
        );

        // Плавно появляется
        seq.Join(
            _text.DOFade(1f, _durationTextShow)
        );
        seq.Join(
            _backGround.DOFade(0.5f, _durationTextShow)
        );
        
    }

    public void HideNarrator() {
        DOTween.Kill(this);

        Sequence seq = DOTween.Sequence();
        seq.SetTarget(this);

        seq.Append(
            _girlImage.DOAnchorPos(_hidePos, 0.4f)
                .SetEase(Ease.InCubic)
        );
        seq.Join(
            _text.transform.DOScale(0f, 0.3f)
        );
                                    
        seq.Join(
            _text.DOFade(0f, 0.2f)
        );
        seq.Join(
            _backGround.DOFade(0f, _durationTextShow)
        );
    }
}
