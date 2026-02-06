using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class TargetMission : IMission {
    [SerializeField] private Transform _target;
    [SerializeField] private float _delta = 1f;
    
    [SerializeField] private string _text;
    [SerializeField] private bool _showArrow;
    [SerializeField] private float _textDuration;
    [SerializeField] private bool _infiniteText;
    
    
    [Inject] private Narrator _narrator; 
    [Inject] private LineToObjects _lineToObjects; 
    [Inject] private PlayerMovement _player; 

    
    
    public async UniTask RunAsync() {
        // Можно допустим сказать временный текст
        TimerText().Forget();

        if (_showArrow) {
            _lineToObjects.SetTargetTutorial(_target.position);
        }
        
        await UniTask.WaitWhile(() => Vector3.Distance(_player.Transform.position, _target.position) > _delta);
        
        if (_showArrow) {
            _lineToObjects.HideArrow();
        }

    }
    
    public async UniTask TimerText() {
        _narrator.SetTextWithNarattor(_text, 3f);
        if (_infiniteText) {
            return;
        }
        await UniTask.WaitForSeconds(_textDuration);
        _narrator.HideNarrator(); 
       
        
    }
}
