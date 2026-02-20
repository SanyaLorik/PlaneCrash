using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RangUnit : MonoBehaviour {
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private Image _img;
    [SerializeField] private RectTransform _rt;

    public void SetData(string money, Sprite img) {
        _moneyText.text = money;
        _img.sprite = img;
    }

    public void ChangeRectTransform(float rangXPos) {
        _rt.anchoredPosition = new Vector2(rangXPos, _rt.anchoredPosition.y);
    }
    
}
