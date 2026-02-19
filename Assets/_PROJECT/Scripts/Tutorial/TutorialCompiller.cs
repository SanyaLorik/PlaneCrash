using System;
using System.Collections.Generic;
using Architecture_M;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class TutorialCompiller : MonoBehaviour {
    [SerializeReference, SubclassSelector] List<IMission> _missions;
    [SerializeReference] private int _idMissionToAllowFlight;
    [SerializeReference] private int _idMissionToCruiser;
    [SerializeReference] private int _idMissionToStartFlightCycle;
    [SerializeField] private GameObject _flightStopper;
    [SerializeField] private GameObject _multiplierBlock;
   
    private bool _isInjected;
    private IGameSave<GameSavePC> _gameSave;
    public bool TutorialPassed { get; private set; }

    [Inject] private Narrator _narrator; 
    [Inject] private DiContainer _diContainer;
    [Inject] private LineToObjects _lineToObjects;
    [Inject] private PlayerStateManager _stateManager;
    
    
    [Inject]
    private void Init(IGameSave<GameSavePC> gameSave) {
        _gameSave = gameSave;
        if (_gameSave.GetSave.TutorialPassed) {
            _flightStopper.DisactiveSelf();
            _multiplierBlock.DisactiveSelf();
            _lineToObjects.TutorialModeDisable();
            _narrator.ActiveCanvas(false);
            TutorialPassed = true;
            return;
        }
        foreach (var mission in _missions) {
            _diContainer.QueueForInject(mission);
        }
        _flightStopper.ActiveSelf();
        _multiplierBlock.ActiveSelf();
        _narrator.ActiveCanvas(true);
        
        
        
        _isInjected = true;
    }
    

    private void Start() {
        if (!_gameSave.GetSave.TutorialPassed) {
            StartTutorial().Forget();
        }
    }

    private async UniTaskVoid StartTutorial() {
        await UniTask.WaitWhile(() => !_isInjected);
        _lineToObjects.TutorialModeEnable();
        for (var i = 0; i < _missions.Count; i++) {
            Debug.Log(i);
            if (i == _idMissionToAllowFlight) {
                _flightStopper.DisactiveSelf();
            }
            else if (i < _idMissionToAllowFlight) {
                _flightStopper.ActiveSelf();
            }

            if (i == _idMissionToCruiser) {
                // Ждем результата полёта
                await UniTask.WaitWhile(() => _stateManager.CurrentState != PlayerState.Grounded && _stateManager.CurrentState != PlayerState.Cruisered);
                // Игрок упал = запускаем заново цикл стрелок и полёта
                Debug.LogWarning("Игрок упал ждем возвращения на спавн");
                if (_stateManager.CurrentState == PlayerState.Grounded) {
                    await UniTask.WaitWhile(() => _stateManager.CurrentState != PlayerState.Walking);
                    Debug.LogWarning("Игрок вернулся на спавн, все по новой");
                    i = _idMissionToStartFlightCycle-1;
                }
                else {
                    await _missions[i].RunAsync();
                }
            }
            else {
                await _missions[i].RunAsync();
            }
        }
        TutorialPassed = true;
        _narrator.HideNarrator();
        _narrator.ActiveCanvas(false);
        
        _lineToObjects.TutorialModeDisable();
        _gameSave.GetSave.TutorialPassed = true;
        _multiplierBlock.DisactiveSelf();
        _gameSave.Save();
    }
}
