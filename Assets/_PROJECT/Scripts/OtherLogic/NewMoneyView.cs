using SanyaBeerExtension;
using TMPro;
using UnityEngine;

public class NewMoneyView : MonoBehaviour {
    [SerializeField] private TMP_Text _newMoneyText;
    [SerializeField] private Color _plusColor = Color.green;
    [SerializeField] private Color _minusColor = Color.crimson;
    
    [field: SerializeField] public CanvasGroup Container { get; private set; }
    [field: SerializeField] public RectTransform RectTransform { get; private set; }


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
