using DG.Tweening;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RewardManager : MonoBehaviour {
    [SerializeField] private GameObject _canvas;
    [SerializeField] private RectTransform _canvasBody;
    
    [SerializeField] private TMP_Text _distanceText;
    [SerializeField] private TMP_Text _rewardText;

    [SerializeField] private Button _backButton;
    [SerializeField] private RectTransform _behindScreenCavasPosition; 
    
    
    [SerializeField] private float _canvasHideSpeed; 
    
    private Vector2 _screenCavasPosition; 


    private bool _inAnimation => _animation != null && _animation.active;
    private Sequence _animation;
    
    private PlayerStateManager _playerStateManager;
    private PlayerMovement _playerMovement;
    private ZoneManager _zoneManager;
    
    
    [Inject] private PlayerBank _playerBank;
    [Inject] private UpgradesCalculator _upgradesCalculator;
    [Inject] private LocalizationDataPC _localization;
    [Inject] private NumberFormatter _formatter;
    [Inject]
    public void Init(PlayerStateManager playerStateManager, PlayerMovement playerMovement, ZoneManager zoneManager) {
        _zoneManager = zoneManager;
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += OnStateChange;
        _playerMovement =  playerMovement;
    }

  

    private void Start() {
        _backButton.onClick.AddListener(RewardAnimation);
        _screenCavasPosition = _canvasBody.anchoredPosition;
        _canvasBody.anchoredPosition = _behindScreenCavasPosition.anchoredPosition;
        _canvas.ActiveSelf();
    }

    private void OnStateChange(PlayerState state) {
        if (state == PlayerState.Cruisered) {
            ShowReward(true);
        }
        else if (state == PlayerState.Grounded) {
            ShowReward(false);
        }
    }

    private void ShowReward(bool cruisered) {
        if (_playerStateManager.BeforeState == PlayerState.Walking) {
            _playerMovement.TpPlayerInBetZone();
            return;
        }
        ShowRewardWindowAnimation();
        double reward;
        if (cruisered) {
            reward = 
                _upgradesCalculator.GetUpgradeMultiplierByLevel() 
                *
                (_zoneManager.BetAmount * _zoneManager.BetMultiplier) 
                + 
                _playerStateManager.CurrentPlayerDistance(); 
        }
        else {
            reward = 
                _playerStateManager.CurrentPlayerDistance() 
                *
                _upgradesCalculator.GetUpgradeMultiplierByLevel();
            _playerBank.GiveMeYourFuckingMoneyNigga(_zoneManager.BetAmount);
            
        }
        
        _playerBank.AddMoney(reward);
        ShowBaseRewardVisual(reward);
    }


    private void ShowBaseRewardVisual(double reward) {
        // Выигрышь
        _distanceText.text = _playerStateManager.CurrentPlayerDistance() + _localization.Meters;
        _rewardText.text = _formatter.ValuteFormatter(reward);
        
    }

    private void RewardAnimation() {
        Sequence buttonPop =  DOTween.Sequence();
        buttonPop
            .Append(_backButton.transform.DOScale(1.2f, 0.2f).From(1f).SetEase(Ease.OutBounce))
            .Append(_backButton.transform.DOScale(1f, 0.2f).From(1.2f).SetEase(Ease.OutBounce));
        
        
        HideRewardWindow();
        _playerStateManager.ChangePlayerState(PlayerState.Walking);
        _playerMovement.TpPlayerInSpawn();
    }

    
    private void ShowRewardWindowAnimation() {
        _canvas.ActiveSelf();
        _animation =  DOTween.Sequence();
        _animation
            .Append(_canvasBody.DOAnchorPos(_screenCavasPosition, _canvasHideSpeed))
            .Append(_backButton.transform.DOScale(1, _canvasHideSpeed).From(0).SetEase(Ease.OutBounce));
        // _canvas.ActiveSelf();
    }

    
    private void HideRewardWindow() {
        _animation =  DOTween.Sequence();

        _animation
            .Append(_canvasBody.DOAnchorPos(_behindScreenCavasPosition.anchoredPosition, _canvasHideSpeed))
            .OnComplete(() => _canvas.DisactiveSelf());

    }

    private void OnDestroy() {
        if (_inAnimation) {
            _animation.Kill();
        }
    }
}
