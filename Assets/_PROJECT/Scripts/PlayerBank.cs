using System;
using TMPro;
using UnityEngine;

public class PlayerBank : MonoBehaviour {
    [SerializeField] private float _playerCapital;
    [SerializeField] private TMP_Text _playerCapitalVisual;
    
    public float PlayerCapital { get => _playerCapital; }

    private void Start() {
        BetVisual.Instantiate.PlayerBetVisual.text = "Банк игрока: " + _playerCapital;
    }
}
