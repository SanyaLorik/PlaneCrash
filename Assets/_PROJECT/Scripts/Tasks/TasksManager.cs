using System;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Extensions_Helpers;
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
    
    // Инфа по заданию и росту
    private readonly Dictionary<TaskType, TaskInfo> _taskTypeToInfoDictionary = new ();
    private readonly Dictionary<TaskType, TaskVisual> _taskTypeToVisualDictionary = new ();


    // Стата игрока в данный момент 
    private float _playerDistance;
    private int _playerBoostsCollect;
    private float _playerMoneyCollect;
    private float _playerMoneyBet;
    
    private CancellationTokenSource _tokenSource;
    private CancellationToken _token;
    

    private PlayerMovement _playerMovement;
    private PlayerStateManager _playerStateManager;
    private PlayerBank _bank;
    
    [Inject] private ZoneManager _zoneManager;

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
        FirstTaskCreate();
    }

    
    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out PlayerMovement player)) {
            UpdateCompleteTasks();
        }
    }

    public bool NeedToGetReward() {
        foreach (var taskInfo in _taskTypeToVisualDictionary) {
            if (taskInfo.Value.TaskIsComplete) {
                return true;
            }
        }
        return false;
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            UpdateMoneyBet(_zoneManager.CurrentBet);
            _token = UniTaskHelper.CreateNewToken(ref _tokenSource);
            PlayerFlightAsync(_token, _playerMovement.Transform).Forget();
        }

        if (state == PlayerState.Grounded || state == PlayerState.Cruisered) {
            _tokenSource?.Cancel();
            Debug.Log($"Игрок перестал лететь, сейчас счет {_playerDistance}");
        }
        
    }
    
    
    // ========== Обработчики заданий ==========
    private async UniTask PlayerFlightAsync(CancellationToken token, Transform playerTransform) {
        float startDistance = playerTransform.position.z; // откуда игрок начал лететь
        
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




    private void UpdateMoneyCollect(float amount) {
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

    

    private void UpdateCompleteTasks() {
        foreach (var taskVisual in _taskTypeToVisualDictionary) {
            if (_taskTypeToVisualDictionary[taskVisual.Key].TaskIsComplete) {
                RefreshCompleteTask(taskVisual.Key);
            }
        }
    }



    private void RefreshCompleteTask(TaskType taskType) {
        // Обновляем даные
        
        TaskInfo taskInfo = _taskTypeToInfoDictionary[taskType];
        TaskVisual taskVisual = _taskTypeToVisualDictionary[taskType];

        _bank.AddMoney(taskInfo.TaskMoney);
        
        taskInfo.FullValue *= taskInfo.ValueMultiplier;
        taskInfo.TaskMoney *= taskInfo.MoneyMultiplier;
        
        
        float playerValue = PlayerValue(taskType);
        taskVisual.SetTaskVisual(taskInfo.TaskMoney, taskInfo.FullValue, playerValue);
        
        // Поидее оно сделается а потом провериться еще раз!
        if (playerValue >= taskInfo.FullValue) {
            taskVisual.SetTaskCompleteVisual();
        }
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
    
    private void FirstTaskCreate() {
        foreach (var taskVisual in _taskTypeToVisualDictionary) {
            TaskInfo taskInfo = _taskTypeToInfoDictionary[taskVisual.Key];
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
        _taskNotification.ShowNotification("+"+ GameHelper.ValuteFormatter(taskInfo.TaskMoney));
    }

    


    private void OnDestroy() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
}
