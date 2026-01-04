using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreVisual : MonoBehaviour {
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private Transform _player;
    [SerializeField] private GameObject _canvas;
    
    [SerializeField] private TMP_Text _totalDistanceText;
    [SerializeField] private TMP_Text _currentDistanceText;
    [SerializeField] private Image _visualProgress;
    [SerializeField] private Transform _cruiser;
    
    [SerializeField] private RectTransform _pointer;
    
    


    private float _totalDistance;
    private float _startPointZ;
    private RectTransform _visualProgressRt;
    [SerializeField] private float _startProgressX;
    [SerializeField] private float _endProgressX;
    private void Start() {
        _visualProgressRt = _visualProgress.gameObject.GetComponent<RectTransform>();
        StartFlight();
    }

    public void StartFlight() {
        _totalDistanceText.text = _cruiser.position.z + "m";
        
        
        _startPointZ = _player.position.z;
        _totalDistance = _cruiser.position.z;
        Debug.Log("Границы прогресса: " + _startProgressX + " " + _endProgressX);
        
        StartCoroutine(ShowDistanceRoutine());
    }

    private IEnumerator ShowDistanceRoutine() {
        while (!_groundChecker.Grounded) {
            float _currentDistance = _player.position.z - _startPointZ; // чтоб начало в 0
            float progress = _currentDistance / _totalDistance;
            _visualProgress.fillAmount = progress;

            float newX = Mathf.Lerp(_startProgressX, _endProgressX, progress);
            Vector3 newPosition = _pointer.anchoredPosition;
            newPosition.x = newX;
            _pointer.anchoredPosition = newPosition;
            
            _currentDistanceText.text = $"{_currentDistance:F2}m";
            yield return null; 
        }
    }


}
