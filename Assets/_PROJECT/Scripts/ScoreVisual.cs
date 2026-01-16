using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScoreVisual : MonoBehaviour {
    [SerializeField] private GameObject _canvas;
    
    [SerializeField] private TMP_Text _totalDistanceText;
    [SerializeField] private TMP_Text _currentDistanceText;
    [SerializeField] private Image _visualProgress;
    [SerializeField] private Transform _cruiser;
    
    [SerializeField] private RectTransform _pointer;
    [SerializeField] private TMP_Text _flightTime;


    [SerializeField] private float _startProgressX;
    [SerializeField] private float _endProgressX;
    
    private RectTransform _visualProgressRt;
    private PlayerStateManager _playerStateManager;
    
    [Inject]
    public void Init(PlayerStateManager playerStateManager) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += OnPlayerStateChange;
    }
    

    private void Start() {
        _visualProgressRt = _visualProgress.gameObject.GetComponent<RectTransform>();
        SetDefault();
        
    }

    private void OnPlayerStateChange(PlayerState state) {
        if (state == PlayerState.Flight) {
            _canvas.SetActive(true);
            FlightScoreLogic();
        }
        else if (state == PlayerState.Grounded) {
            if (_flightRoutine != null) {
                StopCoroutine(_flightRoutine);
            }
        }
        else if (state == PlayerState.Cruisered) {
            if (_flightRoutine != null) {
                StopCoroutine(_flightRoutine);
            }

            SetMaxProgress();
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
        float timer = 0f;
        while (_playerStateManager.CurrentState == PlayerState.Flight) {
            float progress = _playerStateManager.CurrentPlayerDistance / _cruiser.position.z;
            _visualProgress.fillAmount = progress;

            // Visual
            float newX = Mathf.Lerp(_startProgressX, _endProgressX, progress);
            Vector3 newPosition = _pointer.anchoredPosition;
            newPosition.x = newX;
            _pointer.anchoredPosition = newPosition;
            
            _currentDistanceText.text = $"{_playerStateManager.CurrentPlayerDistance:F2}m";
            _flightTime.text = $"Время полёта: {timer:F2}c";
            
            timer += Time.deltaTime;
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

    private void SetMaxProgress() {
        _visualProgress.fillAmount = 1f;
        float newX = _endProgressX;
        Vector3 newPosition = _pointer.anchoredPosition;
        newPosition.x = newX;
        _pointer.anchoredPosition = newPosition;
        _currentDistanceText.text = $"{Math.Round(_cruiser.position.z)}m";
    }


}
