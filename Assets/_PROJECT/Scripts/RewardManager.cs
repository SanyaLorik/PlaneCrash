using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour {
    [SerializeField] private GameObject _canvas;
    [SerializeField] private CanvasGroup _canvasAlphaGroup;
    [SerializeField] private RectTransform _canvasBody;
    
    [SerializeField] private TMP_Text _distanceText;
    [SerializeField] private TMP_Text _bet;
    [SerializeField] private TMP_Text _multiplier;
    [SerializeField] private TMP_Text _rewardText;
    
    
    [SerializeField] private PlayerStateManager _playerStateManager;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private float _distanceRewardDivide = 2f;
    
    [SerializeField] private Button _backButton;
    [SerializeField] private Transform _cruiser;


    private Vector2 _startCavasPosition; 
    private Vector2 _finalCavasPosition; 

    private bool _inAnimation => _animation != null && _animation.active;
    private Sequence _animation;
    
    
    private void Start() {
        _backButton.onClick.AddListener(RewardLogic);
        _playerStateManager.OnChangeState += StateChange;
        _finalCavasPosition = _canvasBody.anchoredPosition;
        _startCavasPosition = new Vector2(_finalCavasPosition.x, -Screen.height/2);
    }

    private void StateChange(PlayerState state) {
        if (state == PlayerState.Cruisered) {
            ShowCruiserReward();
        }
        else if (state == PlayerState.Grounded) {
            ShowDistanceReward();
        }
    }

    public void ShowCruiserReward() {
        ShowReward();
        _distanceText.text = $"Дистанция: {_cruiser.position.z:F2}";
        _multiplier.text = $"Множитель: x{ZoneManager.Instance.CurrentMultiplyer}";
        _bet.text = $"Ставка: {ZoneManager.Instance.CurrentBet:F2}";
        
        float reward = ZoneManager.Instance.CurrentBet * ZoneManager.Instance.CurrentMultiplyer; 
        _rewardText.text = $"Выигрышь: {reward:F2}";
        PlayerBank.Instance.AddMoney(reward);
        // _rewardText.text = money.ToString();
    }
    
    public void ShowDistanceReward() {
        ShowReward();
        float distance = _playerStateManager.CurrentPlayerDistance;
        float reward = distance / _distanceRewardDivide;
        PlayerBank.Instance.GiveMeYourFuckingMoneyNigga(ZoneManager.Instance.CurrentBet);
        PlayerBank.Instance.AddMoney(reward);
        
        _distanceText.text = $"Дистанция: {_playerStateManager.CurrentPlayerDistance:F2}";
        _rewardText.text = $"Выигрышь: {reward:F2}";
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

    private void ShowReward() {
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
