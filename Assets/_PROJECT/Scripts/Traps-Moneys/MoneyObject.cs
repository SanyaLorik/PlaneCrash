using Architecture_M;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;


public class MoneyObject : MonoBehaviour, IMagnetic {
    [SerializeField] private TMP_Text _text;
    [SerializeField] private RotatingAroundAnimation _rotatingAnimation;
    
    public int MoneyAmount;

    
    [Inject] private PlayerBank _playerBank;
    [Inject] private Money2dSpawner _money2dSpawner;
    
    
    public void SetMoneyAmount(int amount) {
        MoneyAmount = amount;
        _text.text = MoneyAmount.ToString();
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
        _playerBank.AddFlightMoney(MoneyAmount);
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
