using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class TextMission : IMission {
    [SerializeField] private string _text;
    [SerializeField] private float _duration;
    
    [Inject] private Narrator _narrator; 
    

    public async UniTask RunAsync() {
        _narrator.SetTextWithNarattor(_text);
        await UniTask.WaitForSeconds(_duration);
    }
}
