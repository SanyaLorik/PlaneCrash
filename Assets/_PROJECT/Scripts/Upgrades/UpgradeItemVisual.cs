using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemVisual : MonoBehaviour {
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _levelVisual;
    [SerializeField] private TMP_Text _xCurrentVisual;
    [SerializeField] private TMP_Text _xNextVisual;
    [SerializeField] private TMP_Text _priceVisual;
    
    
    [SerializeField] private float _brightnessMultiply = 5f;

    private Material _triggerObjectMat;

    private void Awake() {
        _triggerObjectMat = GetComponent<Renderer>().material;
        _triggerObjectMat.EnableKeyword("_EMISSION");
    }


    public void UpdateData(int level, float xCurrent, float xNext, float price) {
        Debug.Log($"level {level} xCurrent {xCurrent} xNext {xNext} price {price}");
        _levelVisual.text = level.ToString();
        _xCurrentVisual.text = xCurrent.ToString();
        _xNextVisual.text = xNext.ToString();
        _priceVisual.text = price + "$";
    }

    public void SetRed() {
        _triggerObjectMat.color = Color.red;
        Color emission = Color.red * _brightnessMultiply; // множитель яркости
        _triggerObjectMat.SetColor(EmissionColor, emission);
        // Debug.Log("Установка красным");
    }
    
    public void SetGreen() {
        _triggerObjectMat.color = Color.green;
        Color emission = Color.green * _brightnessMultiply; // множитель яркости
        _triggerObjectMat.SetColor(EmissionColor, emission);
        // Debug.Log("Установка зеленым");
    }

}
