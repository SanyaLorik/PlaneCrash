using UnityEngine;
using Zenject;

public abstract class UpgradeBase : MonoBehaviour {
    [SerializeField] private ParticleSystem _particleSystem;
    
    
    
    protected UpgradeInfo UpgradeInfo;
    protected string Id;
    
    protected int _level = 1;

    protected float _currentPrice;
    protected UpgradeItemVisual _visual;
    protected IPlayerStatsWritable _playerStats;
    protected PlayerBank _bank;
    
    [Inject] protected UpgradeConfig _config;
    [Inject] protected UpgradesCalculator _upgradesCalculator;
    [Inject] private PlayerVisual _playerVsual;
    
    
    
    [Inject]
    public void Init(PlayerBank bank, IPlayerStatsWritable playerStats) {
        _playerStats = playerStats;
        _bank = bank;
        _bank.BankChanged += BankOnBankChanged;
    }

    protected void Awake() {
        _visual = GetComponent<UpgradeItemVisual>();
    }

    private void BankOnBankChanged(float playerCapital) {
        CheckColor();
    }

    protected void CheckColor() {
        if (_bank.CanBuy(_currentPrice)) {
            _visual.SetGreen();
            _particleSystem.Play();
            
            return;
        }
        _visual.SetRed();
        _particleSystem.Stop();
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement _)) {
            if (_bank.CanBuy(_currentPrice)) {
                ApplyUpgrade();
                _playerVsual.SetBought();
            }
            else {
                Debug.LogWarning("Не хватает срэдств(");
            }
        }
    }

    protected abstract void ApplyUpgrade();
    protected abstract void UpdateVisual();

}
