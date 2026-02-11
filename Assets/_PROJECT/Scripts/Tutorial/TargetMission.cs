using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

[Serializable]
public class TargetMission : IMission {
    [SerializeField] private int _phraseId;
    [SerializeField] private Transform _target;
    [SerializeField] private float _delta = 1f;
    
    [SerializeField] private bool _showArrow;
    
    [SerializeField] private float _textDuration;
    


    
    
    [Inject] private Narrator _narrator; 
    [Inject] private LineToObjects _lineToObjects; 
    [Inject] private PlayerMovement _player; 
    [Inject] private LocalizationDataPC _localization; 

    
    
    public async UniTask RunAsync() {
        // Можно допустим сказать временный текст
        TimerText(_localization.GetPhrase(_phraseId)).Forget();

        if (_showArrow) {
            _lineToObjects.SetTargetTutorial(_target.position);
        }
        
        
        await UniTask.WaitWhile(
            () => Vector3.Distance(_player.transform.position, _target.position) > _delta,
            cancellationToken: _player.GetCancellationTokenOnDestroy()
        );
        
        if (_showArrow) {
            _lineToObjects.HideArrow();
        }

    }
    
    public async UniTask TimerText(string text) {
        _narrator.SetTextWithNarattor(text, 3f);
        await UniTask.WaitForSeconds(_textDuration);
        _narrator.HideNarrator(); 
       
        
    }
}
