using System.Collections.Generic;
using Architecture_M;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class TutorialCompiller : MonoBehaviour {
    [SerializeReference, SubclassSelector] List<IMission> _missions;
    
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
    
    [Header("Для теста с какого индекса начать")]
    [SerializeReference] private int index = 0;
    
    
    [Header("Миссия после которой врубаем канвасы")]
    [SerializeReference] private int _idMissionShowCanvases;
    
    [Header("Скрытие элементов во время тутора")]
    [SerializeReference] private GameObject[] _canvasesToHide;
    
    
    
    [SerializeField] private GameObject _flightStopper;
    [SerializeField] private GameObject _multiplierBlock;
   
    private bool _petAdd; 
    private bool _isInjected;
    
    public bool TutorialPassed { get; private set; }

    [Inject] private IGameSave<GameSavePC> _gameSave;
    [Inject] private Narrator _narrator; 
    [Inject] private DiContainer _diContainer;
    [Inject] private LineToObjects _lineToObjects;
    [Inject] private PlayerStateManager _stateManager;
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    [Inject] private PetOpenView _petOpenView;



    [Inject]
    private void Init() {
        if (_gameSave.GetSave.TutorialPassed) {
            _flightStopper.DisactiveSelf();
            _multiplierBlock.DisactiveSelf();
            SetCanvasesState(true);
            _lineToObjects.TutorialModeDisable();
            _narrator.ActiveCanvas(false);
            TutorialPassed = true;
        }
        else {
            InjectMissions();
            _flightStopper.ActiveSelf();
            _multiplierBlock.ActiveSelf();
            SetCanvasesState(false);
            _narrator.ActiveCanvas(true);
            _isInjected = true;
        }
        
    }

    private void InjectMissions() {
        foreach (var mission in _missions) {
            _diContainer.QueueForInject(mission);
        }
    }

    private void OnEnable() {
        _petOpenView.ClosePetOpen += OnClosePetManager;
    }

    private void OnClosePetManager() {
        Debug.Log("OnClosePetManager");
        if(TutorialPassed) return;
        Debug.Log("index = " + index);
        if (index == _idMissionPetOpen) {
            _petAdd = true;
        }
    }


    private void Start() {
        if (!_gameSave.GetSave.TutorialPassed) {
            StartTutorial().Forget();
        }
    }



    private async UniTaskVoid StartTutorial() {
        await UniTask.WaitWhile(() => !_isInjected);
        _interstitialDelaying.DisableTimer();
        _lineToObjects.TutorialModeEnable();
        for (int i = index; i < _missions.Count; i++) {
            index = i;
            Debug.Log(i);
            if (i == _idMissionToAllowFlight) {
                _flightStopper.DisactiveSelf();
            }
            else if (i < _idMissionToAllowFlight) {
                _flightStopper.ActiveSelf();
            }

            // Цикл полёта
            if (i == _idMissionToAllowFlight+1) {
                // Ждем результата полёта
                await UniTask.WaitWhile(() => _stateManager.CurrentState != PlayerState.Grounded && _stateManager.CurrentState != PlayerState.Cruisered);
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
            }
            else {
                await _missions[i].RunAsync();
            }
            
            // открытие пета
            if (i == _idMissionPetOpen) {
                await _missions[i].RunAsync();
                await UniTask.WaitWhile( () => !_petAdd);
            }

            if (i == _idMissionShowCanvases) {
                SetCanvasesState(true);
            }
            
        }
        TutorialPassed = true;
        _narrator.HideNarrator();
        _narrator.ActiveCanvas(false);
        
        _lineToObjects.TutorialModeDisable();
        _gameSave.GetSave.TutorialPassed = true;
        _multiplierBlock.DisactiveSelf();
        _gameSave.Save();
        _interstitialDelaying.EnableTimer();
    }
    
    
    private void SetCanvasesState(bool state) {
        foreach (var canvas in _canvasesToHide) {
            canvas.SetActive(state);
        }
    }
}
