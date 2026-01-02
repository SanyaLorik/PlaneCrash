using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreVisual : MonoBehaviour {
    [SerializeField] private TMP_Text _scoreText;
    private PlayerMovement _playerMovement;

    private void Start() {
        _playerMovement = GetComponent<PlayerMovement>();
        StartCoroutine(WriteTextRoutine());
    }


    private IEnumerator WriteTextRoutine() {
        while (!_playerMovement.Grounded) {
            _scoreText.text = $"{transform.position.magnitude:F2} м.";
            yield return null; 
        }
        _scoreText.text = "У вас от падения переломался позвоночник и вы теперь парализованы";
    }


}
