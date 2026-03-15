using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RangUnit : MonoBehaviour {
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private Image _img;
    [field: SerializeField] public RectTransform Rt { get; private set; }

    private float XValue => Rt.anchoredPosition.x;
    public float XInside { get; private set; }
    public long Money { get; private set; }

    [Inject] private NumberFormatter _formatter;


    public void SetData(long money, Sprite img, float xInside) {
        Money =  money;
        _moneyText.text = _formatter.ValuteFormatter(Money);
        _img.sprite = img;
        Debug.Log($"xvalue = {XValue}, Rt.localPosition = {Rt.localPosition}, Rt.position = {Rt.position}, xInside = {xInside}");
        XInside =  xInside;
    }
    
}