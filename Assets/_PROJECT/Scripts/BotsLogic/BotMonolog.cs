using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotMonolog : MonoBehaviour {
    [SerializeField] private GameObject _monolog;
    [SerializeField] private TMP_Text _monologText;
    
    private string[] _phrases;
    private string _path = "BotsPhrasesTxt";
 
    private void Awake() {
        _monolog.DisactiveSelf();
        LoadPhrases();
    }

    private void LoadPhrases() {
        TextAsset textAsset = Resources.Load<TextAsset>(_path); // без расширения
        _phrases = textAsset.text.Split('\n');
        if (_phrases.Length < 0) {
            Debug.LogError("Phrase not found");
        }
    }
        

    public void SaySomething() {
        _monolog.ActiveSelf();
        _monologText.text = _phrases[Random.Range(0, _phrases.Length)];
    }

    public void Stfu() {
        _monolog.DisactiveSelf();
    }
    
}
