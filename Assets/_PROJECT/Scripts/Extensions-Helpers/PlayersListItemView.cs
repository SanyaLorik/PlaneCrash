using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[Serializable]
public class PlayersListItemView : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _playerPlace;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private Image _playerBackgroundColor;


    public long MoneyAmount;
    [Inject] private NumberFormatter _formatter;
    
    
    
    public void SetPlayerListData(int place, string playerName, string value) {
         _playerPlace.text = _formatter.ValuteFormatterInteger(place);
         _playerNameText.text = playerName;
         _valueText.text = value;
    }

    public void SetColor(Color color) {
        _playerBackgroundColor.color = color;
    }
}