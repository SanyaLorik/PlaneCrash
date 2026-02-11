using System;
using _PROJECT.Scripts.Extensions_Helpers;
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
    [SerializeField] private TMP_Text _distanceMultiplier;
    [SerializeField] private TMP_Text _rewardText;
    
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
    
    [Inject]
    public void Init(PlayerStateManager playerStateManager, PlayerMovement playerMovement, IPlayerStatsReadOnly playerStats, ZoneManager zoneManager) {
        _zoneManager = zoneManager;
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += OnStateChange;
        _playerMovement =  playerMovement;

    }
    
    
    private void Start() {
        _backButton.onClick.AddListener(RewardLogic);
        _finalCavasPosition = _canvasBody.anchoredPosition;
        _startCavasPosition = new Vector2(_finalCavasPosition.x, -Screen.height/2);
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
        
        float reward = _upgradesCalculator.GetXMultiplierByLevel() * (_zoneManager.CurrentBet * _zoneManager.CurrentMultiplyer) + _zoneManager.CruiserDistance; 
        _distanceText.text = $"Дистанция: {_playerStateManager.CurrentPlayerDistance:F0} м.";
        _betMultiplier.text = $"Множитель ставки: x{_zoneManager.CurrentMultiplyer}";
        _bet.text = $"Ставка: {_zoneManager.CurrentBet:F2}$";
        _rewardText.text = $"Выигрышь: {GameHelper.ValuteFormatter(reward)}$";
        _distanceMultiplier.text = $"Множитель: x{_upgradesCalculator.GetXMultiplierByLevel():F2}";
        
        _playerBank.AddMoney(reward);
        // _rewardText.text = money.ToString();
    }
    
    private void ShowDistanceReward() {
        ShowRewardWindow();
        float distance = _playerStateManager.CurrentPlayerDistance;
        float reward = distance * _upgradesCalculator.GetXMultiplierByLevel();
        _playerBank.GiveMeYourFuckingMoneyNigga(_zoneManager.CurrentBet);
        _playerBank.AddMoney(reward);
        
        _distanceText.text = $"Дистанция: {_playerStateManager.CurrentPlayerDistance:F0} м.";
        _rewardText.text = $"Выигрышь: {GameHelper.ValuteFormatter(reward)}";
        _distanceMultiplier.text = $"Множитель: x{_upgradesCalculator.GetXMultiplierByLevel():F2}";

        _betMultiplier.DisactiveSelf();
        _bet.DisactiveSelf();
    }
    
    private void RewardLogic() {
        Sequence buttonPop =  DOTween.Sequence();
        buttonPop
            .Append(_backButton.transform.DOScale(1.2f, 0.2f).From(1f).SetEase(Ease.OutBounce))
            .Append(_backButton.transform.DOScale(1f, 0.2f).From(1.2f).SetEase(Ease.OutBounce));
        
        
        HideReward();
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

    
    private void HideReward() {
        Sequence animation =  DOTween.Sequence();

        animation
            .Append(_canvasAlphaGroup.DOFade(0, 0.6f).From(1))
            .Join(_canvasBody.DOAnchorPos(_startCavasPosition, 0.6f).From(_finalCavasPosition));
    }

    private void OnDestroy() {
        KillAnimation();
    }

    private void KillAnimation() {
        if (_inAnimation) {
            _animation.Kill();
        }
    }
}
