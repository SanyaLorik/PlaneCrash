using Architecture_M;
using UnityEngine;
using Zenject;


public abstract class UpgradeBase : MonoBehaviour {
    [SerializeField] private DelayedTrigger _delayedTrigger;
    [SerializeField] protected UpgradeType UpgradeType;
    [SerializeField] protected UpgradeItemVisual _visual;


    protected UpgradeInfo _upgradeInfo;
    public UpgradeInfo UpgradeInfo => _upgradeInfo;

    protected int Level {
        get => _gameSave.GetSave.GetUpgradeLevel(_upgradeInfo.Id);
        private set => _gameSave.GetSave.SetNewUpgrade(_upgradeInfo.Id, value);
    } 

    protected double _currentPrice;
    protected IPlayerStatsWritable _playerStats;
    private PlayerBank _bank;
    
    
    [Inject] protected UpgradeConfig _config;
    [Inject] protected UpgradesCalculator _upgradesCalculator;
    [Inject] protected PlayerVisual _playerVisual;
    [Inject] protected IGameSave<GameSavePC> _gameSave;
    [Inject] protected LocalizationDataPC _localization;
    
    
    [Inject]
    public void Init(PlayerBank bank, IPlayerStatsWritable playerStats) {
        _playerStats = playerStats;
        _bank = bank;
        _bank.BankChanged += BankOnBankChanged;
    }

    private void Start() {
        CheckColor();
    }

    public abstract void LoadLevel();
    protected abstract void UpdateVisual();
    protected abstract void UpdatePlayerStatsInfo();

    
    public void ApplyUpgrade(int newLevels, bool gameValute = true) {
        Level += newLevels;
        if (gameValute) {
            _bank.Buy(_currentPrice);
            _gameSave.Save();
        }
        UpdatePlayerStatsInfo();
        UpdatePrice();
        UpdateVisual();
        CheckColor();
    }

    protected abstract void UpdatePrice();
    
    
    private void BankOnBankChanged(long playerCapital) {
        CheckColor();
    }

    private void CheckColor() {
        if (_bank.CanBuy(_currentPrice)) {
            _visual.SetGreen();
            _delayedTrigger.SetAvailable();
            return;
        }
        _visual.SetRed();
        _delayedTrigger.SetUnvailable();
    }

    private void OnTriggerEnter(Collider collider) {
        if (!collider.TryGetComponent(out PlayerMovement _)) return;
        if (_bank.CanBuy(_currentPrice)) {
            _delayedTrigger.DelayedTriggerAction(BuyByTrigger); 
        }
    }

    
    private void OnTriggerExit(Collider other) {
        if (!other.TryGetComponent(out PlayerMovement _))
            return;
        _delayedTrigger.CancelTriggerAction();
    }
    
    private void BuyByTrigger() {
        if (_bank.CanBuy(_currentPrice)) {
            ApplyUpgrade(1);
            _visual.SkillISBought();
            _playerVisual.SetBought();
            _gameSave.Save();
        }
    }
    

}
