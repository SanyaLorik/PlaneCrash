using System;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class LineToObjects : MonoBehaviour {
    [SerializeField] private int _countTimesShowLine;
    [SerializeField] private Transform _posForBoost; // -2.76
    [SerializeField] private Transform _posForSpawn; // - 3.33
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _speed = 1f;
    
    private Vector3 _target;
    private bool _tutorialStarted;
    private float _offset;
    
    
    private PlayerStateManager _playerStateManager;
    private PlayerMovement _player;
    private ZoneManager _zoneManager;
    private bool _arrowInBoost;

    [Inject] private TasksManager _tasksManager;

 

    [Inject]
    private void Init(PlayerMovement player, PlayerStateManager playerStateManager, ZoneManager zoneManager) {
        _player = player;
        _player.SetBoost += PlayerOnSetBoost;
        _playerStateManager  = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _zoneManager = zoneManager;
        _zoneManager.ChooseMultiplier += ChooseMultiplier;
    }
    
    
    private void Awake() {
        SetSpawnPose();
    }
    
    private void Update() {
        if (_target != Vector3.zero) {
            // Обновляем позиции линии
            _lineRenderer.SetPosition(0, transform.position); // от игрока
            _lineRenderer.SetPosition(1, _target); // до цели
            if (_player.transform.position.z > _target.z && _arrowInBoost) {
                ForceHideArrow();
            }
            _offset += Time.deltaTime * _speed;
            _lineRenderer.material.mainTextureOffset = new Vector2(_offset, 0);
        }
    }
    
    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Walking) {
            if (_arrowInBoost) {
                _arrowInBoost = false;
            }
            if (!_tasksManager.NeedToGetReward()) {
                HideArrow();
            }
            SetSpawnPose();
        }

        else if (state == PlayerState.Cruisered || state == PlayerState.Grounded) {
            HideArrow();
        }
    }

    // Метод для изменения цели
    public void SetTarget(Vector3 newTarget) {
        Debug.Log("SetTarget: " + newTarget);
        if (_tutorialStarted && !_arrowInBoost) {
            return;
        }
        _target = newTarget;
        _lineRenderer.enabled = (_target != Vector3.zero);
        if (_target != Vector3.zero) {
            gameObject.ActiveSelf();
        }
        else {
            HideArrow();
        }
    }


    public void TutorialModeEnable() {
        _tutorialStarted = true;
    }
    
    public void TutorialModeDisable() {
        _tutorialStarted = false;
        if (!_tasksManager.NeedToGetReward()) {
            HideArrow();
        }
    }
    
    public void SetTargetTutorial(Vector3 newTarget) {
        _target = newTarget;
        _lineRenderer.enabled = (_target != Vector3.zero);
        gameObject.ActiveSelf();
        ResetOffset();
    }

    public void HideArrow() {
        if(_tutorialStarted) return;
        Debug.Log("HideArrow");
        _target = Vector3.zero;
        gameObject.DisactiveSelf();
        ResetOffset();
    }
    
    public void ForceHideArrow() {
        Debug.Log("ForceHideArrow");
        _target = Vector3.zero;
        gameObject.DisactiveSelf();
        ResetOffset();
    }

    private void ResetOffset() {
        _offset = 0f; // сброс
        _lineRenderer.material.mainTextureOffset = Vector2.zero;
    }


    private void ChooseMultiplier(float obj) {
        if(_tutorialStarted) return;
        HideArrow();
    }

    private int _currentShowLine;
    private void PlayerOnSetBoost() {
        Debug.Log("PlayerOnSetBoost");
        if (_currentShowLine == _countTimesShowLine) {
            HideArrow();
            return;
        }
        _arrowInBoost = true;
        _currentShowLine++;
        Debug.LogWarning("установка игроку буста " + _player.TargetPos);
        SetTarget(_player.TargetPos);
        SetBoosterPose();
    }
    


    private void SetBoosterPose() {
        transform.localPosition = _posForBoost.localPosition;
    }
    
    private void SetSpawnPose() {
        transform.localPosition = _posForSpawn.localPosition;
    }
       
    
}
