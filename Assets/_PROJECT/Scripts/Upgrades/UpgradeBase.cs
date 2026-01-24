using UnityEngine;
using Zenject;

public abstract class UpgradeBase : MonoBehaviour {
    [Header("Коэффициент")]
    [SerializeField] protected float _k;
    [Header("Рост цены")]
    [SerializeField] protected float _priceMultiply;
    [SerializeField] protected float _startPrice;
    protected int _level = 1;

    protected float _currentPrice;
    protected UpgradeItemVisual _visual;
    protected IPlayerStatsWritable _playerStats;

    protected PlayerBank _bank;

    [Inject]
    public void Init(PlayerBank bank, IPlayerStatsWritable playerStats) {
        _playerStats = playerStats;
        _bank = bank;
        _bank.BankChanged += BankOnBankChanged;
    }

    protected void Awake() {
        _visual = GetComponent<UpgradeItemVisual>();
        _currentPrice = _startPrice;
        UpdateVisual();
    }

    private void BankOnBankChanged(float playerCapital) {
        CheckColor();
    }

    protected void CheckColor() {
        if (_bank.CanBuy(_currentPrice)) {
            _visual.SetGreen();
            return;
        }
        _visual.SetRed();
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement _)) {
            if (_bank.CanBuy(_currentPrice)) {
                ApplyUpgrade();
            }
            else {
                Debug.LogWarning("Не хватает срэдств(");
            }
        }
    }

    protected abstract void ApplyUpgrade();
    protected abstract void UpdateVisual();

}
