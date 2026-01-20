using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RangUnit : MonoBehaviour {
    [SerializeField] private TMP_Text _title;
    [SerializeField] private Image _img;

    public void SetData(string title, Sprite img) {
        _title.text = title;
        _img.sprite = img;
    }
    
}
