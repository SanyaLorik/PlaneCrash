using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class TutorialCompiller : MonoBehaviour {
    [SerializeReference, SubclassSelector] List<IMission> _missions;
   
   
    private bool _isInjected = false;
    
    [Inject] private Narrator _narrator; 
    [Inject] private DiContainer _diContainer;
    [Inject] private LineToObjects _lineToObjects;

    [Inject]
    private void Init() {
        foreach (var mission in _missions) {
            _diContainer.QueueForInject(mission);
        }
        _isInjected = true; 
    }
    

    private void Start() {
       StartTutorial().Forget();
   }

   private async UniTaskVoid StartTutorial() {
       await UniTask.WaitWhile(() => !_isInjected);
       _lineToObjects.TutorialModeEnable();
       foreach (var mission in _missions) {
           await mission.RunAsync();
       }
       _narrator.HideNarrator();
       _lineToObjects.TutorialModeDisable();

   }
}
