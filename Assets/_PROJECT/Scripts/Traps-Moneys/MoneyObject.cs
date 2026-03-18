using Architecture_M;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;


public class MoneyObject : MonoBehaviour, IMagnetic {
    [SerializeField] private TMP_Text _text;
    [SerializeField] private RotatingAroundAnimation _rotatingAnimation;
    [SerializeField, Range(0,1)] private float _currentRangPercentage = 0.001f;
    
    private long moneyAmount;

    
    [Inject] private PlayerBank _playerBank;
    [Inject] private Money2dSpawner _money2dSpawner;
    [Inject] private UpgradesCalculator _upgradesCalculator;
    [Inject] private RangManager _rangManager;
    [Inject] private NumberFormatter _formatter;
    
    
    public void SetMoneyAmount(float percent) {
        // Установка как часть _currentRangPercentage процента от суммы некст ранга и на множитель игрока
        moneyAmount = (long)(
            percent 
            * 
            _rangManager.GetNextRangePercentage(_currentRangPercentage) 
            *
            _upgradesCalculator.GetUpgradeMultiplierByLevel()
        );

        _text.text = _formatter.ValuteFormatterInteger(moneyAmount);
        SetAnimation();
    }


    private void SetAnimation() {
        _rotatingAnimation.Animate();
    } 
    
    private void OnTriggerEnter(Collider collider) {
        //_particleSystem.Play(true);
        if (collider.TryGetComponent(out PlayerMovement _)) {
            Collect();
            _money2dSpawner.SpawnOneMoneyInPoint(transform.position);
        }
    }

    private void Collect() {
        _playerBank.AddFlightMoney(moneyAmount);
        _rotatingAnimation.Kill();
        gameObject.DisactiveSelf();
        CanBeMagnetic = false;
    }

    public void DestroyObject() {
        _rotatingAnimation.Kill();
        Destroy(gameObject);
    }

    public bool CanBeMagnetic { get; set; } = true;
    public Vector3 Position =>  transform.position;
    public MagneticType Type { get; } = MagneticType.Money;

    
    public void Attract(Vector3 target, float speed) {
        // Debug.Log("Притяжение буста " + transform.position);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            

        // Debug.Log(Vector3.SqrMagnitude(transform.position - target));
        if (Vector3.SqrMagnitude(transform.position - target) <= 1f) {
            Collect();
        }
    }

}
