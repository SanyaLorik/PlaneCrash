using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RangUnit : MonoBehaviour {
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private Image _img;
    [field: SerializeField] public RectTransform _rt { get; private set; }


    public void SetData(string money, Sprite img) {
        _moneyText.text = money;
        _img.sprite = img;
    }
    
}
