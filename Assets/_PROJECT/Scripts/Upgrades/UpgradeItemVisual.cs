using TMPro;
using UnityEngine;
using Zenject;

public class UpgradeItemVisual : MonoBehaviour {
    [SerializeField] private TMP_Text _levelVisual;
    [SerializeField] private TMP_Text _xCurrentVisual;
    [SerializeField] private TMP_Text _xNextVisual;
    
    [SerializeField] private TMP_Text _priceVisual;
    
    
    [SerializeField] private TMP_Text _titleVisual;
    [SerializeField] private float _brightnessMultiply = 5f;
    
    [Inject] private NumberFormatter _formatter; 
    
    

    public void SetNameText(string text) {
        _titleVisual.text = text;
    }
    
    public void UpdateData(int level, float xCurrent, float xNext, double price, string mesure, bool needRound) {
        // Debug.Log($"level {level} xCurrent {xCurrent} xNext {xNext} price {price}");
        _levelVisual.text = level.ToString();
        _priceVisual.text = _formatter.ValuteFormatter(price);
        if (needRound) {
            _xCurrentVisual.text = ((int)xCurrent) + mesure;
            _xNextVisual.text = ((int)xNext) + mesure;
            return;
        }
        _xCurrentVisual.text = xCurrent.ToString("F2")  + mesure;
        _xNextVisual.text = xNext.ToString("F2")  + mesure;
    }

    public void SetRed() {
        _priceVisual.color = Color.red;
    }
    
    public void SetGreen() {
        _priceVisual.color = Color.white;
    }

}
