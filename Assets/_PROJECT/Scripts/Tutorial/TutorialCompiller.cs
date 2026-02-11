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
    [SerializeField] private GameObject _flightStopper;
   
    private bool _isInjected;
    private IGameSave<GameSavePC> _gameSave;
    
    
    [Inject] private Narrator _narrator; 
    [Inject] private DiContainer _diContainer;
    [Inject] private LineToObjects _lineToObjects;
    
    
    [Inject]
    private void Init(IGameSave<GameSavePC> gameSave) {
        _gameSave = gameSave;
        if (_gameSave.GetSave.TutorialPassed) {
            _flightStopper.DisactiveSelf();
            return;
        }
        foreach (var mission in _missions) {
            _diContainer.QueueForInject(mission);
        }
        _isInjected = true;
        _flightStopper.ActiveSelf();
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
            if (i  == _idMissionToAllowFlight) {
                _flightStopper.DisactiveSelf();
            }
            await _missions[i].RunAsync();
        }

        _narrator.HideNarrator();
        _lineToObjects.TutorialModeDisable();
        _gameSave.GetSave.TutorialPassed = true;
        _gameSave.Save();
    }
}
