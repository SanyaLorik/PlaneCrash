using System;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class LineToObjects : MonoBehaviour {
    [SerializeField] private int _countTimesShowLine;
    [SerializeField] private Transform _getTasksRewardTrigger;
    [SerializeField] private Transform _posForBoost; // -2.76
    [SerializeField] private Transform _posForSpawn; // - 3.33

    
    private Vector3 _target;
    private bool _tutorialStarted;
    
    
    
    private LineRenderer _lineRenderer;
    private PlayerStateManager _playerStateManager;
    private PlayerMovement _player;
    private TasksManager _tasksManager;
    private ZoneManager _zoneManager;

    
    

    [Inject]
    private void Init(PlayerMovement player, PlayerStateManager playerStateManager, TasksManager tasksManager, ZoneManager zoneManager) {
        _player = player;
        _player.SetBoost += PlayerOnSetBoost;
        _playerStateManager  = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _tasksManager = tasksManager;
        _zoneManager = zoneManager;
        _zoneManager.ChooseBet += ZoneManagerStep;
        _zoneManager.ChooseMultiplier += ZoneManagerStep;
    }
    
    
    private void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
        gameObject.DisactiveSelf();
    }

    // Метод для изменения цели
    public void SetTarget(Vector3 newTarget) {
        if (_tutorialStarted && !_arrowInBoost) {
            return;
        }
        _target = newTarget;
        _lineRenderer.enabled = (_target != Vector3.zero);
        if (_target != Vector3.zero) {
            gameObject.ActiveSelf();
        }
        else {
            gameObject.DisactiveSelf();
            
        }
    }



    public void TutorialModeEnable() {
        _tutorialStarted = true;
    }
    
    public void TutorialModeDisable() {
        _tutorialStarted = false;
        SetTarget(Vector3.zero);
    }
    
    public void SetTargetTutorial(Vector3 newTarget) {
        _target = newTarget;
        _lineRenderer.enabled = (_target != Vector3.zero);
        gameObject.ActiveSelf();
    }

    public void HideArrow() {
        _target = Vector3.zero;
        gameObject.DisactiveSelf();
    }
    
    private void ZoneManagerStep(float obj) {
        if(_tutorialStarted) return;
        _target = Vector3.zero;
        gameObject.DisactiveSelf();
    }

    
    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Walking) {
            if (_arrowInBoost) {
                _arrowInBoost = false;
                SetTarget(Vector3.zero);
            }
            gameObject.ActiveSelf();
            SetSpawnPose();
            if (_tasksManager.NeedToGetReward()) {
                SetTarget(_getTasksRewardTrigger.position);
            }
            else if(!_tutorialStarted) {
                gameObject.DisactiveSelf();
            }
        }

        if (state == PlayerState.Cruisered || state == PlayerState.Grounded) {
            SetTarget(Vector3.zero);
        }
    }

    private int _currentShowLine;
    private void PlayerOnSetBoost() {
        if (_currentShowLine == _countTimesShowLine) {
            SetTarget(Vector3.zero);
            gameObject.DisactiveSelf();
            return;
        }
        _arrowInBoost = true;
        _currentShowLine++;
        SetTarget(_player.TargetPos);
        SetBoosterPose();
        if (!gameObject.activeSelf) {
            gameObject.ActiveSelf();
        }
        
    }

    private bool _arrowInBoost;
    private void Update() {
        if (_target != Vector3.zero) {
            // Обновляем позиции линии
            _lineRenderer.SetPosition(0, transform.position); // от игрока
            _lineRenderer.SetPosition(1, _target); // до цели
            if (_player.transform.position.z > _target.z && _arrowInBoost) {
                Debug.Log("Офаем стрелку");
                _target = Vector3.zero;
            }
        }
    }
    


    private void SetBoosterPose() {
        transform.localPosition = _posForBoost.localPosition;
    }
    
    private void SetSpawnPose() {
        transform.localPosition = _posForSpawn.localPosition;
    }
       
    
}
