using System.Collections;
using System.Threading;
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
    [SerializeField] private RectTransform _lightningRT;
    [SerializeField] private Image _lightningImage;
    [SerializeField] private float _lighRotationSpeed;
    
    private CancellationTokenSource _tokenSource;
    
    
    [Inject] private LocalizationDataPC _localization;
    [Inject] private PetStatusColorConfig _petStatusColorConfig;

    
    private void Start() {
        _openButtonText.text = _localization.OpenButton;
        _openButton.onClick.AddListener(OpenLogic);
        _canvas.DisactiveSelf();
    }
    
    public void ShowOpenPetView(PetChance pet) {
        _canvas.ActiveSelf();
        InitCanvas(pet);
    }

    
    private void InitCanvas(PetChance pet) {
        _petIcon.sprite = pet.PetItemConfig.Sprite;
        _petIconRT.localScale = Vector3.zero;
        _petEggRT.localScale = Vector3.one;
        _lightningRT.DisactiveSelf();
        _openButtonRT.localScale = Vector3.one;
        _lightningImage.color = _petStatusColorConfig.GetColorByStatus(pet.PetItemConfig.PetStatus);
        _lightningRT.DisactiveSelf();
    }

    private void OpenLogic() {
        OpenAnimation();
    }

    
    private void CloseWindow() {
        StartCoroutine(CloseAfterDelay(_timeToCloseWindow));
    }
    
    private IEnumerator CloseAfterDelay(float delay) {
        yield return new WaitForSeconds(delay); // ждем своё время
        _openSequence?.Kill();
        _canvas.DisactiveSelf();
    }
    
    
    private Tween _lightningTween;
    private Sequence _openSequence;
    private void OpenAnimation() {
        _petEggRT.localScale = Vector3.one;
        _petIconRT.localScale = Vector3.zero;
        
        
        _openSequence = DOTween.Sequence();
        
        
        // Картинка летит
        _openSequence.Append(
            _petEggRT
                .DOScale(Vector3.zero, _scaleDuration)
                .SetEase(Ease.OutCubic)
        );
        _openSequence.Join(
            _openButtonRT.
                DOScale(Vector3.zero, _scaleDuration)
                .OnComplete(() => _lightningRT.ActiveSelf())
        );
        _openSequence.Append(
            _petIconRT
                .DOScale(Vector3.one, _scaleDuration)
                .SetEase(Ease.OutCubic)
                .OnComplete(CloseWindow)
        );
        _openSequence.Join(
            _lightningRT.DORotate(new Vector3(0f, 0f, 360f), _lighRotationSpeed, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetSpeedBased(true)
        );
    }
    
    
}
