using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class TargetMission : IMission {
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _target;
    [SerializeField] private float _delta = 0.1f;
    [SerializeField] private string _text;
    [SerializeField] private bool _showArrow;
    
    [Inject] private Narrator _narrator; 
    [Inject] private LineToObjects _lineToObjects; 

    public async UniTask RunAsync() {
        // Можно допустим сказать временный текст
        if (!string.IsNullOrEmpty(_text)) {
            _narrator.SetTextWithNarattor(_text); 
        }

        if (_showArrow) {
            _lineToObjects.SetTargetTutorial(_target.position);
        }
        
        await UniTask.WaitWhile(() => Vector3.Distance(_player.position, _target.position) > _delta);
        
        if (_showArrow) {
            _lineToObjects.HideArrow();
        }

    }
}
