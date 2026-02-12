using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BotMonolog : MonoBehaviour {
    [SerializeField] private GameObject _monolog;
    [SerializeField] private TMP_Text _monologText;
    
 
    [Inject] private LocalizationDataPC _localization; 
    
    private void Awake() {
        _monolog.DisactiveSelf();
    }


    public void SaySomething() {
        _monolog.ActiveSelf();
        _monologText.text = _localization.BotsPhrases[Random.Range(0, _localization.BotsPhrases.Length)];
    }

    public void Stfu() {
        _monolog.DisactiveSelf();
    }
    
}
