using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScoreVisual : MonoBehaviour {
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TMP_Text _flightTime;
    [SerializeField] private TMP_Text _totalDistanceText;
    
    [SerializeField] private TMP_Text _currentDistanceText;
    
    [SerializeField] private Image _progressBar;
    
    [SerializeField] private RectTransform _progressBarRt;
    [SerializeField] private RectTransform _pointer;
    
    private PlayerStateManager _playerStateManager;
    
    private float _startProgressX;
    private float _endProgressX;
    private float _pointY;
    
    [Inject] private ZoneManager _zoneManager;
    
    [Inject]
    public void Init(PlayerStateManager playerStateManager) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += OnPlayerStateChange;
    }
    

    private void Start() {
        SetDefault();
        CalculateBounds();
        _pointer.anchoredPosition = new Vector2(_startProgressX, _pointY);
    }

    private void CalculateBounds() {
        float barWidth = _progressBarRt.rect.width;
        float barHeight = _progressBarRt.rect.height;
        float pointerHeight = _pointer.rect.height;
        
        _startProgressX = -barWidth * 0.5f;
        _endProgressX = barWidth * 0.5f;
        _pointY = (-barHeight-pointerHeight) * 0.5f;
    }
    

    private void OnPlayerStateChange(PlayerState state) {
        if (state == PlayerState.Flight) {
            _canvas.SetActive(true);
            _cruiserDistanceZ = _zoneManager.DistanceToCruise;
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
    private float _cruiserDistanceZ;
    
    private void FlightScoreLogic() {
        _totalDistanceText.text = _cruiserDistanceZ + "m";
        _flightRoutine = StartCoroutine(ShowDistanceRoutine());
    }

    private IEnumerator ShowDistanceRoutine() {
        float timer = 0f;
        while (_playerStateManager.CurrentState == PlayerState.Flight) {
            float progress = _playerStateManager.CurrentPlayerDistance / _cruiserDistanceZ;
            _progressBar.fillAmount = progress;

            // Visual
            float newX = Mathf.Lerp(_startProgressX, _endProgressX, progress);
            Vector3 newPosition = _pointer.anchoredPosition;
            newPosition.x = newX;
            _pointer.anchoredPosition = newPosition;
            
            _currentDistanceText.text = $"{_playerStateManager.CurrentPlayerDistance:F2}m";
            // _flightTime.text = $"Время полёта: {timer:F2}c";
            
            timer += Time.deltaTime;
            yield return null; 
        }
    }

    private void SetDefault() {
        float newX = _startProgressX;
        Vector3 newPosition = _pointer.anchoredPosition;
        newPosition.x = newX;
        // Или можно убывает типо 
        _progressBar.fillAmount = 0;
        _canvas.SetActive(false);
    }

    private void SetMaxProgress() {
        _progressBar.fillAmount = 1f;
        float newX = _endProgressX;
        Vector3 newPosition = _pointer.anchoredPosition;
        newPosition.x = newX;
        _pointer.anchoredPosition = newPosition;
        _currentDistanceText.text = $"{Math.Round(_cruiserDistanceZ)}m";
    }


}
