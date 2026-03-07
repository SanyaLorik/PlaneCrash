using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RangUnit : MonoBehaviour {
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private Image _img;
    [field: SerializeField] public RectTransform _rt { get; private set; }

    public float XValue { get; private set; }
    public long Money { get; private set; }

    [Inject] private NumberFormatter _formatter;


    public void SetData(long money, Sprite img, float xValue) {
        Money =  money;
        _moneyText.text = _formatter.ValuteFormatter(Money);
        _img.sprite = img;
        XValue = xValue;
    }
    
}