using System.Collections;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;

public class NewMoneyView : MonoBehaviour {
    [SerializeField] private TMP_Text _newMoneyText;
    [SerializeField] private Color _plusColor = Color.green;
    [SerializeField] private Color _minusColor = Color.crimson;

    
    public void PlusMoney(string money) {
        if (!gameObject.activeSelf) {
            gameObject.ActiveSelf();
        }
        _newMoneyText.text = "+" + money;
        _newMoneyText.color = _plusColor;
    }
    
    public void MinusMoney(string money) {
        if (!gameObject.activeSelf) {
            gameObject.ActiveSelf();
        }
        _newMoneyText.text = "-" + money;
        _newMoneyText.color = _minusColor;
    }

    
}
