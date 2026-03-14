using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using SanyaBeerExtension;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

[Serializable]
public class TargetMission : IMission {
    [SerializeField] private string _phraseId;
    [SerializeField] private Transform[] _targets;
    [SerializeField] private float _delta = 1f;
    [SerializeField] private float _secToStay;
    
    [SerializeField] private bool _showArrow;
    [SerializeField] private bool _allowHideNarrator;
    
    
    [Inject] private Narrator _narrator; 
    [Inject] private LineToObjects _lineToObjects; 
    [Inject] private PlayerMovement _player;

    private bool _narratorIsOver;
    
    public async UniTask RunAsync() {
        // Можно допустим сказать временный текст
        TimerText(_phraseId).Forget();


        foreach (var target in _targets) {
            if (_showArrow) {
                _lineToObjects.SetTargetTutorial(target.position);
            }

            bool isEnd = false;
            while (!isEnd) {
                await UniTask.WaitWhile(
                    () => Vector3.Distance(_player.transform.position, target.position) > _delta,
                    cancellationToken: _player.GetCancellationTokenOnDestroy()
                );

                float elapsedTime = 0f;
                while (Vector3.Distance(_player.transform.position, target.position) <= _delta && !isEnd) {
                    // Отсчет времени
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime >= _secToStay) {
                        isEnd = true;
                    }
                    await UniTask.Yield();
                }
            }
        }
        if (_showArrow) {
            _lineToObjects.HideArrow();
        }
        await UniTask.WaitWhile(() => !_narratorIsOver);
    }
    
    public async UniTask TimerText(string textId) {
        _narratorIsOver = false;
        float speakDuration = _narrator.SetTextWithNarattor(textId);
        await UniTask.WaitForSeconds(speakDuration);
        if (_allowHideNarrator) {
            _narrator.HideNarratorAnimation();
        }
        _narratorIsOver = true;
    }
}
