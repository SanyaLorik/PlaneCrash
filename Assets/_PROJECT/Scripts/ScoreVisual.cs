using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreVisual : MonoBehaviour {
    [SerializeField] private PlayerStateManager _stateManager;
    [SerializeField] private GameObject _canvas;
    
    [SerializeField] private TMP_Text _totalDistanceText;
    [SerializeField] private TMP_Text _currentDistanceText;
    [SerializeField] private Image _visualProgress;
    [SerializeField] private Transform _cruiser;
    
    [SerializeField] private RectTransform _pointer;


    private RectTransform _visualProgressRt;
    [SerializeField] private float _startProgressX;
    [SerializeField] private float _endProgressX;

    private void Start() {
        _stateManager.OnChangeState += OnPlayerStateChange;
        _visualProgressRt = _visualProgress.gameObject.GetComponent<RectTransform>();
    }

    private void OnPlayerStateChange(PlayerState state) {
        if (state == PlayerState.Flight) {
            _canvas.SetActive(true);
            FlightScoreLogic();
        }
        else if (state == PlayerState.Grounded ||  state == PlayerState.Cruisered) {
            if (_flightRoutine != null) {
                StopCoroutine(_flightRoutine);
            }
        }
        else if(state == PlayerState.Walking) {
            SetDefault();
        }
    }
    
    
    private Coroutine _flightRoutine;
    public void FlightScoreLogic() {
        _totalDistanceText.text = _cruiser.position.z + "m";
        
        Debug.Log("Границы прогресса: " + _startProgressX + " " + _endProgressX);
        
        _flightRoutine = StartCoroutine(ShowDistanceRoutine());
    }

    private IEnumerator ShowDistanceRoutine() {
        while (_stateManager.CurrentState == PlayerState.Flight) {
            float progress = _stateManager.CurrentPlayerDistance / _cruiser.position.z;
            _visualProgress.fillAmount = progress;

            // Visual
            float newX = Mathf.Lerp(_startProgressX, _endProgressX, progress);
            Vector3 newPosition = _pointer.anchoredPosition;
            newPosition.x = newX;
            _pointer.anchoredPosition = newPosition;
            
            _currentDistanceText.text = $"{_stateManager.CurrentPlayerDistance:F2}m";
            yield return null; 
        }
    }

    private void SetDefault() {
        float newX = _startProgressX;
        Vector3 newPosition = _pointer.anchoredPosition;
        newPosition.x = newX;
        // Или можно убывает типо 
        _visualProgress.fillAmount = 0;
        _canvas.SetActive(false);
    }


}
