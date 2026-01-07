using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour {
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TMP_Text _distanceText;
    [SerializeField] private TMP_Text _bet;
    [SerializeField] private TMP_Text _multiplier;
    [SerializeField] private TMP_Text _rewardText;
    
    
    [SerializeField] private PlayerStateManager _playerStateManager;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private float _distanceRewardDivide = 2f;
    
    [SerializeField] private Button _backButton;
    [SerializeField] private Transform _cruiser;
    
    
    private void Start() {
        _backButton.onClick.AddListener(RewardLogic);
        _playerStateManager.OnChangeState += StateChange;
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
        _canvas.SetActive(true);
        _distanceText.text = $"Дистанция: {_cruiser.position.z:F2}";
        _multiplier.text = $"Множитель: x{ZoneManager.Instance.CurrentMultiplyer}";
        _bet.text = $"Ставка: {ZoneManager.Instance.CurrentBet:F2}";
        
        float reward = ZoneManager.Instance.CurrentBet * ZoneManager.Instance.CurrentMultiplyer; 
        _rewardText.text = $"Выигрышь: {reward:F2}";
        PlayerBank.Instance.AddMoney(reward);
        // _rewardText.text = money.ToString();
    }
    
    public void ShowDistanceReward() {
        _canvas.SetActive(true);
        float distance = _playerStateManager.CurrentPlayerDistance;
        float reward = distance / _distanceRewardDivide;
        PlayerBank.Instance.GiveMeYourFuckingMoneyNigga(ZoneManager.Instance.CurrentBet);
        PlayerBank.Instance.AddMoney(reward);
        
        _distanceText.text = $"Дистанция: {_playerStateManager.CurrentPlayerDistance:F2}";
        _rewardText.text = $"Выигрышь: {reward:F2}";
    }
    
    private void RewardLogic() {
        _canvas.SetActive(false);
        _playerStateManager.ChangePlayerState(PlayerState.Walking);
        _playerMovement.TpPlayerInSpawn();
    }


}
