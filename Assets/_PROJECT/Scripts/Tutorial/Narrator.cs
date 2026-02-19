using System;
using System.Collections;
using DG.Tweening;
using JetBrains.Annotations;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Narrator : MonoBehaviour {
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private RectTransform _girlImage;
    [SerializeField] private Image[] _backgrounds;
    
    
    [SerializeField] private float _durationNarratorShow;
    [SerializeField] private float _durationTextShow;

    
    [SerializeField] private float _textShowSpeed = 0.05f;  
    

    [Header("Тряска бабы с сиьсками")]
    [SerializeField] private float _strength = 5f;    // насколько далеко двигаем
    [SerializeField] private float _speed = 0.05f;  
    

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
    }


    public void ActiveCanvas(bool state) {
        _canvas.SetActive(state);
    }

    public void SetTextWithNarattor(string text, float speakTimeAnimation) {
        _text.text = text;
        // Сбиваем старые анимации текста
        _text.transform.DOKill();

        // Маленький "пульс"


        ShowNarrator(speakTimeAnimation);

    }
    
    

    private Coroutine _timerCoroutine;
    private void ShowNarrator(float speakTime) {
        Debug.Log("ShowNarrator");
        if (_timerCoroutine!=null) {
            StopCoroutine(_timerCoroutine);
        }
        DOTween.Kill(this);
        Sequence seq = DOTween.Sequence();

        // Картинка летит
        seq.Append(
            _girlImage.DOAnchorPos(_startPos, _durationNarratorShow)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => {
                    if (_timerCoroutine != null) {
                        StopCoroutine(_timerCoroutine);
                    }
                    _timerCoroutine = StartCoroutine(Timer(speakTime));
                    _girlImage.anchoredPosition = _startPos; // фиксируем базу
                    _stopSpeaking = false;
                    StartTalking();
                })
        );
        foreach (var background in _backgrounds) {
            seq.Join(
                background.DOFade(1f, _durationTextShow)
            );
        }
        seq.Join(
            _text.transform
                .DOScale(1.15f, _textShowSpeed) // быстро увеличился
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    _text.transform
                        .DOScale(1f, _textShowSpeed); // вернулся назад
            })
        );

        // Только появление текста (без scale)

        _stopSpeaking = false;

        
    }



    private IEnumerator Timer(float time) {
        yield return new WaitForSeconds(time);
        Debug.Log("_stopSpeaking = true");
        _stopSpeaking = true;
    }
    
    
    
    
    public void HideNarrator() {
        Debug.Log("HideNarrator");

        if (_timerCoroutine != null)
            StopCoroutine(_timerCoroutine);

        DOTween.Kill(this);

        _stopSpeaking = true;

        StopTalking(); // ВАЖНО

        Sequence seq = DOTween.Sequence();
        seq.SetTarget(this);

        seq.Append(
            _girlImage.DOAnchorPos(_hidePos, 0.4f)
                .SetEase(Ease.InCubic)
        );

        seq.Join(
            _text.transform.DOScale(0f, 0.3f)
        );

        foreach (var background in _backgrounds)
        {
            seq.Join(
                background.DOFade(0f, _durationTextShow)
            );
        }
    }

    

    private Tween _talkTween;

    private void StartTalking() {
        DoNextMove();
    }

    private void DoNextMove() {
        if (_stopSpeaking) return;

        Vector2 offset = new Vector2(
            Random.Range(-_strength, _strength),
            Random.Range(-_strength, _strength)
        );

        _talkTween = _girlImage
            .DOAnchorPos(_startPos + offset, _speed)
            .SetEase(Ease.Linear)
            .OnComplete(DoNextMove);
    }

    private void StopTalking() {
        _stopSpeaking = true;

        if (_talkTween != null && _talkTween.IsActive()) {
            _talkTween.Kill();
            _talkTween = null;
        }

        _girlImage.anchoredPosition = _startPos;
    }



    private bool _stopSpeaking;
}
