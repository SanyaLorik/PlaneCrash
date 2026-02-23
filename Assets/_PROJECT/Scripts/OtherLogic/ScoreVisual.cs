using System;
using System.Collections;
using Architecture_M;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class ScoreVisual : MonoBehaviour {
    [SerializeField] private GameObject _canvas;
    
    [SerializeField] private RectTransform _currentProgressBar;
    [SerializeField] private RectTransform _recordProgressBar;
    
    [SerializeField] private RectTransform _finishPointer;
    [SerializeField] private RectTransform _currentPointer;
    [SerializeField] private RectTransform _recordPointer;
    
    [SerializeField] private TextMeshProUGUI _currentDistanceText;
    [SerializeField] private TextMeshProUGUI _recordDistanceText;
    [SerializeField] private TextMeshProUGUI _finishDistanceText;
    [SerializeField] private float _pointerOffset;
    
    [Header("Настройка слайдеров")]
    [SerializeField] private RectTransform _parentRectTransform;
    
    
    
    [Inject] private PlayerStateManager _playerStateManager;
    [Inject] private ZoneManager _zoneManager;
    [Inject] private LocalizationDataPC _localization;
    [Inject] private IGameSave<GameSavePC> _saver;
    [Inject] private RectTransformHelper _fillAmounthMover;
    
    [Inject]
    public void OnEnable() {
        _playerStateManager.ChangeState += OnPlayerStateChange;
    }
    

    private void Start() {
        SetDefault();
    }
    

 
    private void OnPlayerStateChange(PlayerState state) {
        if (state == PlayerState.Flight) {
            _canvas.ActiveSelf();
            Debug.Log("OnPlayerStateChange вызывает FlightScoreLogic");
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
    private float _maxDistance;
    private void FlightScoreLogic() {
        Debug.Log("FlightScoreLogic");
        UpdateRecordText(_saver.GetSave.RecordDistance);
        
        _finishDistanceText.text = _zoneManager.DistanceToCruise +  _localization.Meters;
        _maxDistance = MathF.Max(_zoneManager.DistanceToCruise, _saver.GetSave.RecordDistance);
        if (Mathf.Approximately(_zoneManager.DistanceToCruise, _saver.GetSave.RecordDistance)) {
            _finishDistanceText.text = string.Empty;
        }
        Debug.Log(_zoneManager.DistanceToCruise + " " + _saver.GetSave.RecordDistance);


        // Move Finish
        float percent = _zoneManager.DistanceToCruise/_maxDistance;
        float xEnd = _fillAmounthMover.CalculateXEnd(_parentRectTransform);
        _fillAmounthMover.SetPointer(_finishPointer, percent, xEnd, _pointerOffset);
        
        percent = _saver.GetSave.RecordDistance / _maxDistance;
        _fillAmounthMover.SetFillAmountWithPointer(_recordProgressBar, _parentRectTransform, _recordPointer, percent);
       
        _flightRoutine = StartCoroutine(ShowDistanceRoutine());
    }

    private IEnumerator ShowDistanceRoutine() {
        while (_playerStateManager.CurrentState == PlayerState.Flight) {
            // Это процент полета но он не пойдет в SetFillAmount т.к там 100 процентов - конец
            float percent = _playerStateManager.CurrentPlayerDistance() / _maxDistance;
            _fillAmounthMover.SetFillAmountWithPointer(_currentProgressBar, _parentRectTransform, _currentPointer, percent);
            _currentDistanceText.text = $"{(int)_playerStateManager.CurrentPlayerDistance()}{_localization.Meters}";
            yield return null; 
        }

        if (_playerStateManager.CurrentPlayerDistance() > _saver.GetSave.RecordDistance) {
            _saver.GetSave.RecordDistance = (int)_playerStateManager.CurrentPlayerDistance();
            _saver.Save();
            Debug.Log("ShowDistanceRoutine вызывает UpdateRecordText");
            UpdateRecordText(_saver.GetSave.RecordDistance);
        }
        _canvas.DisactiveSelf();
        
    }

    private void UpdateRecordText(int distance) {
        if (_saver.GetSave.RecordDistance != 0 && !_recordProgressBar.gameObject.activeSelf) {
            _recordProgressBar.ActiveSelf();
            _recordPointer.ActiveSelf();
        }
        else if(_saver.GetSave.RecordDistance == 0) {
            _recordProgressBar.DisactiveSelf();
            _recordPointer.DisactiveSelf();
            return;
        }
        _recordDistanceText.text = distance + _localization.Meters;
    }
    


    private void SetDefault() {
        _fillAmounthMover.SetFillAmountWithPointer(_currentProgressBar, _parentRectTransform, _currentPointer, 0);
        _canvas.SetActive(false);
    }

    private void SetMaxProgress() {
        _fillAmounthMover.SetFillAmountWithPointer(_currentProgressBar, _parentRectTransform, _currentPointer, 1);
        _currentDistanceText.text = $"{Math.Round(_zoneManager.DistanceToCruise)}m";
    }

 
    

}
