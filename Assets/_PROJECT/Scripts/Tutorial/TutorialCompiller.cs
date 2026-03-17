using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Architecture_M;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class TutorialCompiller : MonoBehaviour {
    [SerializeReference, SubclassSelector] List<IMission> _missions;
    
    [Header("Для теста с какого индекса начать")]
    [SerializeReference] private int index = 0;
    
    [Header("Миссия на которой разрешается лететь")]
    [SerializeReference] private int _idMissionToAllowFlight;
    
    [Header("Миссия на которой начинается цикл полета")]
    [SerializeReference] private int _idMissionToStartCycle;
    [Header("Ой вы упали...")]
    [SerializeReference] private int _idMissionIfFall;
    [Header("Легенда долетел..")]
    [SerializeReference] private int _idMissionIfCruisered;
    
    [Header("Миссия после которой ждем закрытия канваса")]
    [SerializeReference] private int _idMissionPetOpen;
    
    [Header("Миссия после которой врубаем канвасы")]
    [SerializeReference] private int _idMissionShowCanvases;
    
    [Header("Скрытие элементов во время тутора")]
    [SerializeReference] private GameObject[] _canvasesToHide;
    
    [SerializeField] private GameObject _flightStopper;
    [SerializeField] private GameObject _multiplierBlock;
   
    private bool _petAdd; 
    private bool _isInjected;
    
    public bool TutorialPassed => _gameSave.GetSave.TutorialPassed;
    public event Action<int> TutorialStepChanged;
    public event Action TutorialIsOver;
    

    [Inject] private IGameSave<GameSavePC> _gameSave;
    [Inject] private DiContainer _diContainer;
    [Inject] private PlayerStateManager _stateManager;
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    [Inject] private PetOpenView _petOpenView;


    [Inject]
    private void Init() {
        if (!TutorialPassed) {
            InjectMissions();
        }
    }

    private void Awake() {
        if (TutorialPassed) {
            PrepareTutorial(false);
            TutorialIsOver?.Invoke();
        }
        else {
            PrepareTutorial(true);
            StartTutorial().Forget();
        }
    }

    private void PrepareTutorial(bool tutorialStarting) {
        _flightStopper.SetActive(tutorialStarting);
        _multiplierBlock.SetActive(tutorialStarting);
        SetCanvasesState(!tutorialStarting);
        if (tutorialStarting) {
            _interstitialDelaying.DisableTimer();
        }
    }

    private void InitCloseTutorial() {
        _interstitialDelaying.EnableTimer();
        _gameSave.GetSave.TutorialPassed = true;
        _gameSave.Save();
        _petOpenView.ClosePetOpen -= OnClosePetManager;

        SetCanvasesState(true);
        _flightStopper.SetActive(false);
        _multiplierBlock.SetActive(false);
        TutorialIsOver?.Invoke();
    }


    private void InjectMissions() {
        foreach (var mission in _missions) {
            _diContainer.QueueForInject(mission);
        }
        _isInjected = true;
    }

    private void OnEnable() {
        if (!TutorialPassed) {
            _petOpenView.ClosePetOpen += OnClosePetManager;
        }
    }

    private void OnClosePetManager() {
        Debug.Log("OnClosePetManager");
        if(TutorialPassed) return;
        Debug.Log("index = " + index);
        if (index == _idMissionPetOpen) {
            _petAdd = true;
        }
    }




    private async UniTaskVoid StartTutorial() {
        await UniTask.WaitWhile(() => !_isInjected);
        Debug.Log("StartTutorial");
        for (int i = index; i < _missions.Count; i++) {
            TutorialStepChanged?.Invoke(i);
            _interstitialDelaying.DisableTimer();
            index = i;
            FlightAllow(i);

            // Цикл полёта
            if (i == _idMissionToAllowFlight+1) {
                i = await FlightCycle();
            }
            else if (i != _idMissionPetOpen) {
                await _missions[i].RunAsync();
            }
            
            // открытие пета
            if (i == _idMissionPetOpen) {
                Debug.Log("Миссия с петом");
                await _missions[i].RunAsync();
                await UniTask.WaitWhile( () => !_petAdd);
            }

            if (i == _idMissionShowCanvases) {
                SetCanvasesState(true);
            }
            
        }
        InitCloseTutorial();
    }

    private async Task<int> FlightCycle() {
        int i;
        // Ждем результата полёта
        await UniTask.WaitWhile(() => 
            _stateManager.CurrentState != PlayerState.Grounded && 
            _stateManager.CurrentState != PlayerState.Cruisered
        );
        // Игрок упал = запускаем заново цикл стрелок и полёта
        Debug.LogWarning("Игрок упал ждем возвращения на спавн");
        if (_stateManager.CurrentState == PlayerState.Grounded) {
            await _missions[_idMissionIfFall].RunAsync();
            await UniTask.WaitWhile(() => _stateManager.CurrentState != PlayerState.Walking);
            Debug.LogWarning("Игрок вернулся на спавн, все по новой");
            // -1 т.к новая итерация i инкрементит
            i = _idMissionToStartCycle-1;
        }
        else {
            // Прошел, лега
            i = _idMissionIfCruisered - 1;
        }

        return i;
    }

    private void FlightAllow(int i) {
        if (i == _idMissionToAllowFlight) {
            _flightStopper.DisactiveSelf();
        }
        else if (i < _idMissionToAllowFlight) {
            _flightStopper.ActiveSelf();
        }
    }




    private void SetCanvasesState(bool state) {
        foreach (var canvas in _canvasesToHide) {
            canvas.SetActive(state);
        }
    }
}

