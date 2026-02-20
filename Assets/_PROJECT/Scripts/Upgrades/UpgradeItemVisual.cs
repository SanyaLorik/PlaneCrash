using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UpgradeItemVisual : MonoBehaviour {
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    [SerializeField] private TMP_Text _levelVisual;
    [SerializeField] private TMP_Text _xCurrentVisual;
    [SerializeField] private TMP_Text _xNextVisual;
    
    [SerializeField] private TMP_Text _priceVisual;
    
    
    [SerializeField] private TMP_Text _titleVisual;
    [SerializeField] private float _brightnessMultiply = 5f;

    private Material _triggerObjectMat;

    
    [Inject] private NumberFormatter _formatter; 
    
    
    private void Awake() {
        _triggerObjectMat = GetComponent<Renderer>().material;
        _triggerObjectMat.EnableKeyword("_EMISSION");
    }

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
        // _triggerObjectMat.color = Color.red;
        // Color emission = Color.red * _brightnessMultiply; // множитель яркости
        // _triggerObjectMat.SetColor(EmissionColor, emission);
        // // Debug.Log("Установка красным");
        _priceVisual.color = Color.red;
    }
    
    public void SetGreen() {
        // _triggerObjectMat.color = Color.green;
        // Color emission = Color.green * _brightnessMultiply; // множитель яркости
        // _triggerObjectMat.SetColor(EmissionColor, emission);
        // // Debug.Log("Установка зеленым");
        _priceVisual.color = Color.white;
    }

}
