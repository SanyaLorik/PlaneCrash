using System;
using Architecture_M;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class TextMission : IMission {
    [SerializeField] private string _phraseId;
    [SerializeField] private float _duration;
    
    [Inject] private Narrator _narrator; 
    [Inject] private LocalizationDataPC _localization; 
    [Inject] private ILanguageProvider _languageProvider; 


    public async UniTask RunAsync() {
        _narrator.SetTextWithNarattor(_localization.GetPhrase(_phraseId), 3f);
        await UniTask.WaitForSeconds(_duration);
        _narrator.HideNarrator();
    }
}
