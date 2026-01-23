using TMPro;
using UnityEngine;



public class MoneyObject : MonoBehaviour {
    
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private TMP_Text _text;
    public int MoneyAmount;

    public void SetMoneyAmount(int amount) {
        MoneyAmount = amount;
        _text.text = MoneyAmount.ToString();
    }
    
    
    private void OnTriggerEnter(Collider collider) {
        _particleSystem.Play(true);
        if (collider.TryGetComponent(out PlayerBank bank)) {
            bank.AddMoney(MoneyAmount);
            // Debug.Log("Начисление бабла!");
        }
       
    }
    

}
