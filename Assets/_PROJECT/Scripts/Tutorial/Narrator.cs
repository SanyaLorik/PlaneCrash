using System;
using System.Collections;
using Architecture_M;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;


[Serializable]
public struct VoiceSounds {
    public string PhraseId;
    public AudioClip RusClip;
    public AudioClip EngClip;
}



public class Narrator : MonoBehaviour {
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private RectTransform _girlImage;
    [SerializeField] private Image[] _backgrounds;
    [SerializeField] private RectTransform _hidePoint;
    
    
    [SerializeField] private float _durationNarratorShow;
    [SerializeField] private float _durationTextShow;

    
    [SerializeField] private float _textShowSpeed = 0.05f;  
    

    [Header("Тряска бабы с сиьсками")]
    [SerializeField] private float _strength = 5f;    // насколько далеко двигаем
    [SerializeField] private float _speed = 0.05f;
    
    [Header("Озвучка по ID")]
    [SerializeField] private VoiceSounds[] _voiceSounds;  
    [SerializeField, Range(0,2)] private float _pitchValue;  
    

    private Vector2 _startPos;
    private Vector2 _hidePos;
    
    [Inject] private LocalizationDataPC _localization;
    [Inject] private SoundManager _soundManager;
    [Inject] private TutorialCompiller _tutorialCompiller;


    private bool _isRusTutorial = true;
    private void Start() {
        if (!_tutorialCompiller.TutorialPassed) {
            // Запоминаем финальную позицию картинки
            _startPos = _girlImage.anchoredPosition;

            // Прячем слева за экран
            _hidePos = new Vector2(
                _hidePoint.position.x - 300f,
                _startPos.y
            );

            _girlImage.anchoredPosition = _hidePos;

            // Текст в ноль
            _text.transform.localScale = Vector3.zero;


            if (_localization.Substitute != LanguageEnum.Russian) {
                _isRusTutorial = false;
            }
        }
    }

    private void OnEnable() {
        if (!_tutorialCompiller.TutorialPassed) {
            _tutorialCompiller.TutorialIsOver += OnTutorialIsOver;
            ActiveCanvas(true);
        }
        else {
            ActiveCanvas(false);
        }
    }

    private void OnTutorialIsOver() {
        HideNarratorAnimation();
        ActiveCanvas(false);
    }


    public float SetTextWithNarattor(string textId) {
        Debug.Log("Вызов наратора");
        _text.text = _localization.GetTranslatedName(textId, _localization.TutorTranslates);
        // Сбиваем старые анимации текста
        _text.transform.DOKill();

        // Маленький "пульс"    
        // Вычислить из длительности звука 
        float speakTimeAnimation = 3f;
        foreach (VoiceSounds voice in _voiceSounds) {
            if (voice.PhraseId == textId) {
                AudioClip clip = _isRusTutorial ? voice.RusClip : voice.EngClip;
                speakTimeAnimation = clip.length * 1.05f;
                Debug.Log(clip.length);
                _soundManager.PlayNarratorSound(clip, _pitchValue);
            }
        }
        ShowNarrator(speakTimeAnimation);
        return speakTimeAnimation;
    }


    private void ActiveCanvas(bool state) {
        _canvas.SetActive(state);
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
    
    
    public void HideNarratorAnimation() {
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
