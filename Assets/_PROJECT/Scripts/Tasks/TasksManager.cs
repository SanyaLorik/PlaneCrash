using System;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


[Serializable]
public enum TaskType {
    Distance,
    BoostCollect,
    MoneyCollect,
    MoneyBet,
}

[Serializable]
public class TaskInfo {
    public float FullValue;
    public float TaskMoney;
    
    public float ValueMultiplier;
    public float MoneyMultiplier;

    public TaskType TaskType;
}


public class TasksManager : MonoBehaviour {
    [Header("Набор заданий")]
    [SerializeField] private List<TaskInfo> _tasksInfo;
    
    [Header("Визуалы")]
    [SerializeField] private List<TaskVisual> _tasksVisual;

    [SerializeField] private TaskNotification _taskNotification;
    [SerializeField] private ParticleSystem _taskCompletePS;
    
    
    [SerializeField] private DelayedTrigger _delayedTrigger;
    [SerializeField] private Transform _getTasksRewardTrigger;
    
    
    // Инфа по заданию и росту
    private readonly Dictionary<TaskType, TaskInfo> _taskTypeToInfoDictionary = new ();
    private readonly Dictionary<TaskType, TaskVisual> _taskTypeToVisualDictionary = new ();
        

    // Стата игрока в данный момент 
    private float _playerDistance;
    private int _playerBoostsCollect;
    private float _playerMoneyCollect;
    private float _playerMoneyBet;
    
    private CancellationTokenSource _tokenSource;
    

    private PlayerMovement _playerMovement;
    private PlayerStateManager _playerStateManager;
    private PlayerBank _bank;
    
    [Inject] private Money2dSpawner _money2dSpawner;
    [Inject] private ZoneManager _zoneManager;
    [Inject] private NumberFormatter _formatter; 
    [Inject] private LocalizationDataPC _localization; 
    [Inject] private UpgradesCalculator _upgradesCalculator;
    [Inject] private LineToObjects _lineToObjects;


    [Inject]
    private void Init(PlayerStateManager playerStateManager, PlayerMovement playerMovement, PlayerBank bank) {
        _playerStateManager = playerStateManager;
        _playerMovement = playerMovement;
        _bank = bank;
        
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _playerMovement.SetBoost += UpdateBoostsCount;
        _bank.MoneyCollect += UpdateMoneyCollect;
    }

    private void Awake() {
        CreateTaskInfoDictionary();
        CreateTaskVisualDictionary();
    }


    private void Start() {
        TableInitialize();
        _delayedTrigger.SetUnvailable();
    }

    private void OnTriggerEnter(Collider collider) {
        if (!collider.TryGetComponent(out PlayerMovement _)) return;
        UpdateCompleteTasksByTrigger();
        
    }
    
    private void OnTriggerExit(Collider collider) {
        if (!collider.TryGetComponent(out PlayerMovement _)) return;
            _delayedTrigger.CancelTriggerAction();
        
    }

    public bool NeedToGetReward() {
        foreach (var taskInfo in _taskTypeToVisualDictionary) {
            if (taskInfo.Value.TaskIsComplete) {
                _delayedTrigger.SetAvailable();
                return true;
            }
        }
        _delayedTrigger.SetUnvailable();
        return false;
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            UpdateMoneyBet(_zoneManager.BetAmount);
            _tokenSource = new CancellationTokenSource();
            PlayerFlightAsync(_tokenSource.Token, _playerMovement.Transform).Forget();
        }

        if (state == PlayerState.Grounded || state == PlayerState.Cruisered) {
            _tokenSource?.Cancel();
            // Debug.Log($"Игрок перестал лететь, сейчас счет {_playerDistance}");
        }

        if (state == PlayerState.Walking) {
            CheckToNeedLine();
        }
        
    }
    
    
    // ========== Обработчики заданий ==========
    private async UniTask PlayerFlightAsync(CancellationToken token, Transform playerTransform) {
        float startDistance = _playerStateManager.CurrentPlayerDistance(); // откуда игрок начал лететь
        
        TaskInfo taskInfo = _taskTypeToInfoDictionary[TaskType.Distance];
        TaskVisual taskVisual = _taskTypeToVisualDictionary[TaskType.Distance];
        
        float distanceForReward = taskInfo.FullValue;
        bool distanceIsDone = false;
        Debug.Log($"Игроку пролететь: {distanceForReward} он стартанул в {startDistance}, сейчас счет {_playerDistance}");
        
        while (!token.IsCancellationRequested) {
            // Вдруг както игрока назад откинуло например ловушкой
            float delta = playerTransform.position.z - startDistance;
            if (delta > 0) {
                _playerDistance += delta;
            }
            startDistance = playerTransform.position.z;
            if (_playerDistance >= distanceForReward && !distanceIsDone) {
                distanceIsDone =  true;
                Debug.Log("Задание по дистанции выполнено!");
                ShowNotification(taskInfo);
                taskVisual.SetTaskCompleteVisual();
            }
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        if (!distanceIsDone) {
            taskVisual.UpdateTaskScoreVisual(_playerDistance, taskInfo.FullValue);
        }
    }


    private void UpdateMoneyCollect(int amount) {
        _playerMoneyCollect += amount;
        UpdateTaskProgress(TaskType.MoneyCollect, _playerMoneyCollect);
    }
    

    private void UpdateMoneyBet(float bet) {
        _playerMoneyBet += bet;
        UpdateTaskProgress(TaskType.MoneyBet, _playerMoneyBet);
    }

    private void UpdateBoostsCount() {
        _playerBoostsCollect += 1;
        UpdateTaskProgress(TaskType.BoostCollect, _playerBoostsCollect);
    }
    

    private void UpdateTaskProgress(TaskType type, float currentValue) {
        TaskInfo taskInfo = _taskTypeToInfoDictionary[type];
        TaskVisual taskVisual = _taskTypeToVisualDictionary[type];
        
        if (currentValue >= taskInfo.FullValue && !taskVisual.TaskIsComplete) {
            taskVisual.SetTaskCompleteVisual();
            ShowNotification(taskInfo);
        }
        else {
            taskVisual.UpdateTaskScoreVisual(currentValue, taskInfo.FullValue);
        }
    }

    

    private void UpdateCompleteTasksByTrigger() {
        foreach (var taskVisual in _taskTypeToVisualDictionary) {
            if (_taskTypeToVisualDictionary[taskVisual.Key].TaskIsComplete) {
                _delayedTrigger.DelayedTriggerAction(() => RefreshCompleteTask(taskVisual.Key));
            }
        }
       
    }



    private void RefreshCompleteTask(TaskType taskType) {
        // Обновляем данные
        
        TaskInfo taskInfo = _taskTypeToInfoDictionary[taskType];
        TaskVisual taskVisual = _taskTypeToVisualDictionary[taskType];

        _bank.AddMoney(taskInfo.TaskMoney * _upgradesCalculator.GetUpgradeMultiplierByLevel());
        _money2dSpawner.SpawnOneMoneyInPoint(transform.position);
        
        
        taskInfo.FullValue *= taskInfo.ValueMultiplier;
        taskInfo.TaskMoney *= taskInfo.MoneyMultiplier;
        
        
        float playerValue = PlayerValue(taskType);
        taskVisual.SetTaskVisual(taskInfo.TaskMoney, taskInfo.FullValue, playerValue);
        
        // Поидее оно сделается а потом провериться еще раз!
        if (playerValue >= taskInfo.FullValue) {
            taskVisual.SetTaskCompleteVisual();
        }
        _taskCompletePS.Play();
        CheckToNeedLine();
    }

    public void CheckToNeedLine() {
        _lineToObjects.SetTarget(NeedToGetReward() ? _getTasksRewardTrigger.position : Vector3.zero);
    }

    private void CreateTaskInfoDictionary() {
        foreach (var task in _tasksInfo) {
            if (_taskTypeToInfoDictionary.ContainsKey(task.TaskType)) {
                Debug.LogWarning($"Повтор ключя! {task.TaskType}");
            }
            _taskTypeToInfoDictionary[task.TaskType] = task;
        }
    }
    
    
    private void CreateTaskVisualDictionary() {
        foreach (var task in _tasksVisual) {
            if (_taskTypeToVisualDictionary.ContainsKey(task.TaskType)) {
                Debug.LogWarning($"Повтор ключя! {task.TaskType}");
            }
            _taskTypeToVisualDictionary[task.TaskType] = task;
            
        }
    }   
    
    
    private void TableInitialize() {
        foreach (var taskVisual in _taskTypeToVisualDictionary) {
            TaskInfo taskInfo = _taskTypeToInfoDictionary[taskVisual.Key];
            _taskTypeToVisualDictionary[taskVisual.Key].SetTaskLocalizationText();
            _taskTypeToVisualDictionary[taskVisual.Key].SetTaskVisual(taskInfo.TaskMoney,taskInfo.FullValue, PlayerValue(taskVisual.Key));
        }
    }
    
    
    // Т.к я решил поля со статами игрока хранить тут, то сделаем так
    private float PlayerValue(TaskType taskType) {
        switch (taskType) {
            case TaskType.Distance:
                return _playerDistance;
            case TaskType.BoostCollect:
                return _playerBoostsCollect;
            case TaskType.MoneyCollect:
                return _playerMoneyCollect;
            case TaskType.MoneyBet:
                return _playerMoneyBet;
            default: return -1;
        }
    }
    
    private void ShowNotification(TaskInfo taskInfo) {
        _taskNotification.ShowNotification("+"+ _formatter.ValuteFormatter(taskInfo.TaskMoney));
    }

    


    private void OnDestroy() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
}
