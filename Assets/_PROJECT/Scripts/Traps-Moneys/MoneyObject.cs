using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;


public class MoneyObject : MonoBehaviour, IMagnetic {
    
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private TMP_Text _text;
    public int MoneyAmount;

    
    
    private PlayerBank _playerBank;
    
    [Inject]
    public void Init(PlayerBank playerBank) {
        _playerBank = playerBank;
    }
    
    
    
    public void SetMoneyAmount(int amount) {
        MoneyAmount = amount;
        _text.text = MoneyAmount.ToString();
    }
    
    
    private void OnTriggerEnter(Collider collider) {
        _particleSystem.Play(true);
        if (collider.TryGetComponent(out PlayerMovement _)) {
            Collect();
        }
       
    }

    private void Collect() {
        _playerBank.AddFlightMoney(MoneyAmount);
        Debug.Log("Начисление бабла!");
        gameObject.DisactiveSelf();
        CanBeMagnetic = false;
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
