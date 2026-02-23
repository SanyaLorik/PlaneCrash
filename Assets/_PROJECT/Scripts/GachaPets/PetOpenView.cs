using System.Collections;
using System.Threading;
using Architecture_M;
using DG.Tweening;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PetOpenView : MonoBehaviour {
    [SerializeField] private GameObject _canvas;

    [Header("Иконки")]
    [SerializeField] private RectTransform _petEggRT;
    [SerializeField] private RectTransform _petIconRT;
    [SerializeField] private Image _petIcon;
    
    [Header("Кнопка")]
    [SerializeField] private Button _openButton;
    [SerializeField] private RectTransform _openButtonRT;
    [SerializeField] private TMP_Text _openButtonText;
    [SerializeField] private float _timeToCloseWindow;
    
    [Header("Анимации")]
    [SerializeField] private float _scaleDuration;
    [SerializeField] private Ease _ease;
    
    
    private CancellationTokenSource _tokenSource;
    private Tween _lightningTween;
    private Sequence _openSequence;
    
    
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    [Inject] private LocalizationDataPC _localization;
    [Inject] private PetStatusColorConfig _petStatusColorConfig;
    [Inject] private IInputActivity _inputActivity;

    
    private void Start() {
        _openButtonText.text = _localization.OpenButton;
        _openButton.onClick.AddListener(OpenAnimation);
        _canvas.DisactiveSelf();
    }
    
    public void ShowOpenPetView(PetChance pet) {
        _interstitialDelaying.DisableTimer();
        _inputActivity.Disable();
        
        _canvas.ActiveSelf();
        _openButtonRT.ActiveSelf();
        InitCanvas(pet);
    }

    
    private void InitCanvas(PetChance pet) {
        _petIcon.sprite = pet.PetItemConfig.Sprite;
        _petIconRT.localScale = Vector3.zero;
        _petEggRT.localScale = Vector3.one;
        _openButtonRT.localScale = Vector3.one;
    }
    

    private void OpenAnimation() {
        _petEggRT.localScale = Vector3.one;
        _petIconRT.localScale = Vector3.zero;
        _openSequence = DOTween.Sequence();
        
        
        // Картинка летит
        _openSequence.Append(
            _petEggRT
                .DOScale(Vector3.zero, _scaleDuration)
                .SetEase(_ease)
        );
        _openSequence.Join(
            _openButtonRT.
                DOScale(Vector3.zero, _scaleDuration)
                .SetEase(_ease)
                .OnComplete(_openButtonRT.DisactiveSelf)
        );
        _openSequence.Append(
            _petIconRT
                .DOScale(Vector3.one, _scaleDuration)
                .SetEase(_ease)
                .OnComplete(CloseWindow)
        );
    }
    
    private void CloseWindow() {
        StartCoroutine(CloseAfterDelay(_timeToCloseWindow));
    }
    
    private IEnumerator CloseAfterDelay(float delay) {
        yield return new WaitForSeconds(delay); // ждем своё время
        _openSequence?.Kill();
        _canvas.DisactiveSelf();
        _interstitialDelaying.DisableTimer();
        _inputActivity.Enable();
    }
    
}
