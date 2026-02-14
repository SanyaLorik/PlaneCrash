using DG.Tweening;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RewardManager : MonoBehaviour {
    [SerializeField] private GameObject _canvas;
    [SerializeField] private CanvasGroup _canvasAlphaGroup;
    [SerializeField] private RectTransform _canvasBody;
    [SerializeField] private PlayerBank _playerBank;
    
    [SerializeField] private TMP_Text _distanceText;
    [SerializeField] private TMP_Text _bet;
    [SerializeField] private TMP_Text _betMultiplier;
    [SerializeField] private TMP_Text _upgradeMultiplier;
    [SerializeField] private TMP_Text _rewardText;

    [Header("UI элементы")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _buttonText;
    
    
    [SerializeField] private float _distanceRewardDivide = 2f;
    
    [SerializeField] private Button _backButton;
    [SerializeField] private Transform _cruiser;


    private Vector2 _startCavasPosition; 
    private Vector2 _finalCavasPosition; 

    private bool _inAnimation => _animation != null && _animation.active;
    private Sequence _animation;
    
    private PlayerStateManager _playerStateManager;
    private PlayerMovement _playerMovement;
    private ZoneManager _zoneManager;
    
    [Inject] private UpgradesCalculator _upgradesCalculator;
    [Inject] private LocalizationDataPC _localization;
    [Inject] private NumberFormatter _formatter;
    
    [Inject]
    public void Init(PlayerStateManager playerStateManager, PlayerMovement playerMovement, IPlayerStatsReadOnly playerStats, ZoneManager zoneManager) {
        _zoneManager = zoneManager;
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += OnStateChange;
        _playerMovement =  playerMovement;

    }

    
    
    private void Start() {
        _backButton.onClick.AddListener(RewardAnimation);
        _finalCavasPosition = _canvasBody.anchoredPosition;
        _startCavasPosition = new Vector2(_finalCavasPosition.x, -Screen.height/2);

        _canvasBody.anchoredPosition = _startCavasPosition;
        _titleText.text = _localization.FlightResultTitle;
        _buttonText.text = _localization.FlightComebackButton;
    }

    private void OnStateChange(PlayerState state) {
        if (state == PlayerState.Cruisered) {
            ShowCruiserReward();
        }
        else if (state == PlayerState.Grounded) {
            ShowDistanceReward();
        }
    }

    private void ShowCruiserReward() {
        ShowRewardWindow();
        _betMultiplier.ActiveSelf();
        _bet.ActiveSelf();
        
        float reward = 
            _upgradesCalculator.GetUpgradeMultiplierByLevel() 
            *
            (_zoneManager.BetAmount * _zoneManager.BetMultiplier) 
            + 
            _zoneManager.CruiserDistance; 
        
                
        _playerBank.AddMoney(reward);

        ShowBaseReward(reward);
        
        // Множитель ставки
        _betMultiplier.text = string.Format(
            _localization.BetMultiplierTemplate,
            $"{_zoneManager.BetMultiplier}"
        );
        
        // Ставка
        _bet.text = string.Format(
            _localization.BetAmountTemplate,
            $"{_formatter.ValuteFormatter(_zoneManager.BetAmount)}"
        );
    }
    
    private void ShowDistanceReward() {
        ShowRewardWindow();
        float reward = _playerStateManager.CurrentPlayerDistance 
                       *
                       _upgradesCalculator.GetUpgradeMultiplierByLevel();
        
        _playerBank.GiveMeYourFuckingMoneyNigga(_zoneManager.BetAmount);
        _playerBank.AddMoney(reward);
        
        ShowBaseReward(reward);

        _betMultiplier.DisactiveSelf();
        _bet.DisactiveSelf();
    }

    private void ShowBaseReward(float reward) {
        _distanceText.text = string.Format(
            _localization.DistanceTemplate,
            $"{_playerStateManager.CurrentPlayerDistance:F0}"
        );
        
        // Выигрышь
        _rewardText.text = string.Format(
            _localization.RewardTemplate,
            $"{_formatter.ValuteFormatter(reward)}"
        );
        
        // Множитель дистанции
        _upgradeMultiplier.text = string.Format(
            _localization.UpgradeMultiplierTemplate,
            $"{_upgradesCalculator.GetUpgradeMultiplierByLevel():F2}"
        );
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

    private void ShowRewardWindow() {
        _animation =  DOTween.Sequence();
        _animation
            .Append(_canvasAlphaGroup.DOFade(1, 1f).From(0))
            .Join(_canvasBody.DOAnchorPos(_finalCavasPosition, 0.6f).From(_startCavasPosition))
            .Append(_backButton.transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBounce));
    }

    
    private void HideRewardWindow() {
        Sequence animation =  DOTween.Sequence();

        animation
            .Append(_canvasAlphaGroup.DOFade(0, 0.6f).From(1))
            .Join(_canvasBody.DOAnchorPos(_startCavasPosition, 0.6f).From(_finalCavasPosition));
    }

    private void OnDestroy() {
        if (_inAnimation) {
            _animation.Kill();
        }
    }
}
