using System;
using Architecture_M;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class TextMission : IMission {
    [SerializeField] private string _phraseId;
    [SerializeField] private bool _allowHideNarrator;
    
    // [SerializeField] private float _duration;
    
    [Inject] private Narrator _narrator; 


    public async UniTask RunAsync() {
        float speakDuration = _narrator.SetTextWithNarattor(_phraseId);
        await UniTask.WaitForSeconds(Math.Max(speakDuration, speakDuration));
        if (_allowHideNarrator) {
            _narrator.HideNarrator();
        }
    }
}
