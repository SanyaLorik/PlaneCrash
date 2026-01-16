using System;
using UnityEngine;
using Zenject;

public class LineToBoosts : MonoBehaviour {
    [SerializeField] private int _countTimesShowLine;
    
    private Vector3 _target;
    
    
    private LineRenderer _lineRenderer;
    private PlayerStateManager _playerStateManager;
    private PlayerMovement _player;
    



    [Inject]
    public void Construct(PlayerMovement player, PlayerStateManager playerStateManager) {
        _player = player;
        _player.SetBoost += PlayerOnSetBoost;
        _playerStateManager  = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Walking) {
            _target = Vector3.zero;
        }
    }

    private int _currentShowLine;
    private void PlayerOnSetBoost() {
        if (_currentShowLine == _countTimesShowLine) {
            SetTarget(Vector3.zero);
        }
        if (_currentShowLine < _countTimesShowLine) {
            _currentShowLine++;
            SetTarget(_player.TargetPos);
        }
        
    }

    private void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
    }
    
    
    
    void Update() {
        if (_target != Vector3.zero) {
            // Обновляем позиции линии
            _lineRenderer.SetPosition(0, transform.position); // от игрока
            _lineRenderer.SetPosition(1, _target); // до цели
            if (_player.transform.position.z > _target.z) {
                _target = Vector3.zero;
            }
        }
    }
    
    // Метод для изменения цели
    public void SetTarget(Vector3 newTarget) {
        _target = newTarget;
        _lineRenderer.enabled = (_target != Vector3.zero);
    }

    
}
