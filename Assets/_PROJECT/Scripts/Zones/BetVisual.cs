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
    [Inject] private NumberFormatter _formatter; 
    [Inject] private LocalizationDataPC _localization; 

    
    [Inject]
    public void Init(PlayerBank bank, PlayerStateManager playerStateManager, IPlayerStatsReadOnly playerStats, ZoneManager zoneManager) {
        _zoneManager = zoneManager;
        _zoneManager.ChooseBet += ShowBet;
        _zoneManager.ChooseMultiplier += ShowMultiplier;
        
        _bank = bank;
        _bank.BankChanged += OnChangeBank;
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += OnChangeState;
        _playerStats = playerStats;
        _playerStats.ChangeStats += PlayerStatsOnChangeStats;
    }

    private void PlayerStatsOnChangeStats() {
        _playerXMultiplyer.text = string.Format(
            _localization.UpgradeMultiplierTemplate,
            $"{_upgradesCalculator.GetUpgradeMultiplierByLevel():F2}"
        );
    }

    private void Start() {
        PlayerStatsOnChangeStats();
    }

    private void OnChangeState(PlayerState state) {
        _playerBetVisual.text = "";
        _xMultiplyVisual.text = "";
        _rewardVisual.text = "";
        _distanceVisual.text = "";
    }


    private void OnChangeBank(float capital) {

        _playerBank.text = string.Format(
            _localization.PlayerBalanceTemplate,
            $"{_formatter.ValuteFormatter(capital)}"
        );

    }

    private void ShowBet(float bet) {
        _playerBetVisual.text = string.Format(
            _localization.BetAmountTemplate,
            $"{_formatter.ValuteFormatter(bet)}"
        );
        
        _xMultiplyVisual.text = "";
        _rewardVisual.text = "";
        _distanceVisual.text = "";

    }
    
    private void ShowMultiplier(float multiplyer) {
        _xMultiplyVisual.text = string.Format(
            _localization.BetMultiplierTemplate,
            $"{multiplyer}"
        );
        
        _rewardVisual.text = string.Format(
            _localization.RewardTemplate,
            $"{_formatter.ValuteFormatter(_zoneManager.BetAmount *  multiplyer)}"
        );
        
        _distanceVisual.text = string.Format(
            _localization.DistanceTemplate,
            $"{_zoneManager.DistanceToCruise}"
        );
        
    }

    
    
    private void OnDisable() {
        if (_zoneManager != null) {
            _zoneManager.ChooseBet -= ShowBet;
            _zoneManager.ChooseMultiplier -= ShowMultiplier;
        }
        _bank.BankChanged -= OnChangeBank;
        _playerStateManager.ChangeState -= OnChangeState;
    }

    
    
}
