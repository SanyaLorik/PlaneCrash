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
    [SerializeField] private RectTransform _container;
    [SerializeField] private RectTransform _pointerToHide;
    private float _yInScreen;
    private float _yBottomScreen;

    [Header("Иконки")]
    [SerializeField] private RectTransform _petEggRT;
    [SerializeField] private RectTransform _petIconRT;
    [SerializeField] private Image _petIcon;
    
    [Header("Кнопка")]
    [SerializeField] private Button _openButton;
    [SerializeField] private RectTransform _openButtonRT;
    [SerializeField] private TMP_Text _openButtonText;

    [Header("Анимации")] 
    [SerializeField] private float _scaleDuration = 1.5f;
    [SerializeField] private float _flightCanvasDuration = 1f;
    [SerializeField] private float _showNewPetDuration = 1f;
    [SerializeField] private Ease _scale01Ease;
    [SerializeField] private Ease _scale10Ease;
    [SerializeField] private Ease _showCanvasEase;
    [SerializeField] private Ease _hideCanvasEase;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Image _colorLighning;
    
    
    private CancellationTokenSource _tokenSource;
    private Tween _lightningTween;
    private Sequence _openSequence;
    private PetStatus _newPetStatus;
    
    
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    [Inject] private LocalizationDataPC _localization;
    [Inject] private PetStatusColorConfig _petStatusColorConfig;
    [Inject] private IInputActivity _inputActivity;
    [Inject] private RectTransformHelper _fillAmounthMover;



    private void Start() {
        _openButtonText.text = _localization.OpenButton;
        _openButton.onClick.AddListener(OpenAnimation);
        _canvas.ActiveSelf();
        _yBottomScreen = _fillAmounthMover.GetYBottomScreen(_container, _pointerToHide);
        _yInScreen = _container.anchoredPosition.y;
        _container.anchoredPosition = new Vector2(_container.anchoredPosition.x, _yBottomScreen);
        _canvas.DisactiveSelf();
        Debug.Log($"_yInScreen = {_yInScreen}, _yBottomScreen = {_yBottomScreen}");
    }
    
    public void ShowOpenPetView(PetChance pet) {
        _interstitialDelaying.DisableTimer();
        _inputActivity.Disable();

        OpenCanvasAnimation();
        _openButtonRT.ActiveSelf();
        InitCanvas(pet);
    }

    
    private void InitCanvas(PetChance pet) {
        _petIconRT.localScale = Vector3.zero;
        _petEggRT.localScale = Vector3.one;
        _openButtonRT.localScale = Vector3.one;
        
        _petIcon.sprite = pet.PetItemConfig.Sprite;
        _newPetStatus = pet.PetItemConfig.PetStatus;
        _colorLighning.color = _defaultColor;
        
    }
    

    private void OpenAnimation() {
        _openSequence = DOTween.Sequence();
        
        // Картинка летит
        _openSequence.Append(
            _petEggRT
                .DOScale(Vector3.zero, _scaleDuration)
                .SetEase(_scale10Ease)
                .OnComplete(OnShowPet)
        );
        _openSequence.Join(
            _openButtonRT.
                DOScale(Vector3.zero, _scaleDuration)
                .SetEase(_scale10Ease)
                .OnComplete(_openButtonRT.DisactiveSelf)
        );
        _openSequence.Append(
            _petIconRT
                .DOScale(Vector3.one, _scaleDuration)
                .SetEase(_scale01Ease)
                .OnComplete(CloseWindow)
        );
    }

    private void OnShowPet() {
        _colorLighning.color = _petStatusColorConfig.GetColorByStatus(_newPetStatus);
    }
    
    private void CloseWindow() {
        StartCoroutine(WaitToHideRoutine());
    }


    private IEnumerator WaitToHideRoutine() {
        yield return new WaitForSeconds(_showNewPetDuration);
        HideCanvasAnimation();
        _interstitialDelaying.EnableTimer();
        _inputActivity.Enable();
    } 
    
    
    private void HideCanvasAnimation() {
        _container.DOMoveY(_yBottomScreen, _flightCanvasDuration)
            .SetEase(_hideCanvasEase)
            .OnComplete(_canvas.DisactiveSelf);
    }
    
    private void OpenCanvasAnimation() {
        _canvas.ActiveSelf();
        _container.DOAnchorPosY(_yInScreen, _flightCanvasDuration)
            .SetEase(_showCanvasEase);
    }
    
    
}
