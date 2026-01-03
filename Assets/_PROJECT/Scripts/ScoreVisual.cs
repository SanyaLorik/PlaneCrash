using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreVisual : MonoBehaviour {
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private GroundChecker _groundChecker;

    private void Start() {
        StartCoroutine(WriteTextRoutine());
    }

    private IEnumerator WriteTextRoutine() {
        while (!_groundChecker.Grounded) {
            _scoreText.text = $"{transform.position.magnitude:F2} м.";
            yield return null; 
        }
        _scoreText.text = "У вас от падения переломался позвоночник и вы теперь парализованы";
    }


}
