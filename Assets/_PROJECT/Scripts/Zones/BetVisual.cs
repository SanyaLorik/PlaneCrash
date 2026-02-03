using _PROJECT.Scripts.Extensions_Helpers;
using TMPro;
using UnityEngine;
using Zenject;

public class BetVisual : MonoBehaviour {
    [SerializeField] private TMP_Text _playerBank;
    [SerializeField] private TMP_Text _playerBetVisual;
    [SerializeField] private TMP_Text _xMultiplyVisual;
    [SerializeField] private TMP_Text _rewardVisual;
    [SerializeField] private TMP_Text _distanceVisual;
    
    [Header("Player Stats")]
    [SerializeField] private TMP_Text _playerXMultiplyer;
    
    private PlayerStateManager _playerStateManager;
    private PlayerBank _bank;
    private ZoneManager _zoneManager;
    private IPlayerStatsReadOnly _playerStats;
    
    [Inject] private UpgradesCalculator _upgradesCalculator;
    
    [Inject]
    public void Init(PlayerBank bank, PlayerStateManager playerStateManager, IPlayerStatsReadOnly playerStats, ZoneManager zoneManager) {
        _zoneManager = zoneManager;
        
        _bank = bank;
        _bank.BankChanged += OnChangeBank;
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += OnChangeState;
        _playerStats = playerStats;
        _playerStats.ChangeStats += PlayerStatsOnChangeStats;
    }

    private void PlayerStatsOnChangeStats() {
        _playerXMultiplyer.text = $"Множитель метров: {_upgradesCalculator.GetXMultiplierByLevel():F2}";
    }

    private void Start() {
        _zoneManager.ChooseBet += ShowBet;
        _zoneManager.ChooseMultiplyer += ShowMultiplyer;
        PlayerStatsOnChangeStats();
    }

    private void OnChangeState(PlayerState state) {
        _playerBetVisual.text = "";
        _xMultiplyVisual.text = "";
        _rewardVisual.text = "";
        _distanceVisual.text = "";
    }


    private void OnDisable() {
        if (_zoneManager != null) {
            _zoneManager.ChooseBet -= ShowBet;
            _zoneManager.ChooseMultiplyer -= ShowMultiplyer;
        }
        _bank.BankChanged -= OnChangeBank;
        _playerStateManager.ChangeState -= OnChangeState;
    }

    private void OnChangeBank(float capital) {
        _playerBank.text = $"Баланс: {GameHelper.ValuteFormatter(capital)}";
    }

    private void ShowBet(float bet) {
        _playerBetVisual.text = $"Ставка: {GameHelper.ValuteFormatter(bet)}";
        _xMultiplyVisual.text = "";
        _rewardVisual.text = "";
        _distanceVisual.text = "";

    }
    
    private void ShowMultiplyer(float multiplyer) {
        _xMultiplyVisual.text = $"Множитель: x{multiplyer}";
        _rewardVisual.text = $"Выигрыш: {GameHelper.ValuteFormatter(_zoneManager.CurrentBet *  multiplyer)}";
        _distanceVisual.text = $"До финиша: {_zoneManager.CruiserDistance}м.";
    }

}
