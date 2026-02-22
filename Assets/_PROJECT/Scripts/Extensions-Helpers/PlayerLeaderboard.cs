using System;
using NaughtyAttributes;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;


public class PlayerLeaderboard : MonoBehaviour {
    [field: SerializeField] public bool IsTopPlayerLeaderboard { get; private set; }
    [SerializeField, ShowIf(nameof(IsTopPlayerLeaderboard))] private int _zeroMoneyIndex;
   
    
    
    [SerializeField] private PlayersListItemView[] _playerLists;
    [SerializeField] private PairedValue<long> _valueDiapasone;
    [SerializeField] private AnimationCurve _curve;

    
    [Header("Цвета")]
    [SerializeField] private Color[] _colors;
    [SerializeField, ShowIf(nameof(IsTopPlayerLeaderboard))] private Color _playerColor;
    
    
    [SerializeField] private int _maxCharsName;
    [SerializeField] private int _maxDigitCount;
    
    
    
    [Inject] private NumberFormatter _formatter;
    [Inject] private PlayerBank _playerBank;
    [Inject] private LocalizationDataPC _localization;
    private bool _playerInTable;
    
    
    private string[] _ruMaleFirst;
    private string[] _ruFemaleFirst;
    private string[] _ruMaleLast;
    private string[] _ruFemaleLast;
    private string[] _enMaleFirst;
    private string[] _enFemaleFirst;
    private string[] _enLast;

    private void Awake() {
        GetFilesStrings();
    }

    

    private void OnEnable() {
        _playerBank.BankChanged += PlayerBankOnBankChanged;
    }

    private void PlayerBankOnBankChanged(long obj) {
        // IN DEV...
    }


    private void Start() {
        PlayersInit();
    }
    
    private void PlayersInit() {
        _playerInTable = false;
        for (var i = 0; i < _playerLists.Length; i++) {
            long money = (long)Mathf.Lerp(
                _valueDiapasone.From,
                _valueDiapasone.To,
                _curve.Evaluate((float)(i + 1) / _playerLists.Length)
            );
            string name = GetRandomName();
            // Нашли игрока
            if (_playerBank.PlayerCapital >= money && !_playerInTable && IsTopPlayerLeaderboard) {
                _playerLists[i].SetColor(_playerColor);
                money = _playerBank.PlayerCapital;
                name = _localization.You;
                _playerInTable = true;
            }
            else if (i < _colors.Length) {
                _playerLists[i].SetColor(_colors[i]);
            }
            _playerLists[i].MoneyAmount = money;
            
            
            string valuePretty = _formatter.ValuteFormatterInteger(money);
            _playerLists[i].SetPlayerListData(
                i + 1,
                name,
                valuePretty
            );
        }

        if (_playerInTable || !IsTopPlayerLeaderboard) return;
        // IN DEV ADD FAKE INDEX...
        
        _playerLists[^1].SetColor(_playerColor);
        float percent = (float) _playerBank.PlayerCapital / _playerLists[^1].MoneyAmount;
        Debug.Log($"{_playerLists[^1].MoneyAmount} / {_playerBank.PlayerCapital}");
        int index = (int)Mathf.Lerp(_zeroMoneyIndex, _playerLists.Length-1, percent);
        _playerLists[^1].SetPlayerListData(index, _localization.You, _formatter.ValuteFormatterInteger(_playerBank.PlayerCapital));
    }

    
    private void GetFilesStrings() {
        _ruMaleFirst = LoadFile("PlayerNames/ru_male_first");
        _ruMaleLast = LoadFile("PlayerNames/ru_male_last");
        _ruFemaleFirst = LoadFile("PlayerNames/ru_female_first");
        _ruFemaleLast = LoadFile("PlayerNames/ru_female_last");
        _enMaleFirst = LoadFile("PlayerNames/en_male_first");
        _enFemaleFirst = LoadFile("PlayerNames/en_female_first");
        _enLast = LoadFile("PlayerNames/en_last");
    }
    
    private string[] LoadFile(string path) {
        TextAsset asset = Resources.Load<TextAsset>(path);

        return asset.text
            .Replace("\r", "")          // убираем carriage return
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);
    }
    
    
    private string GetRandomName() {
        bool rusPlayer = Random.value > 0.5;
        bool male = Random.value > 0.5;
        
        string name;
        if (rusPlayer &&  male) {
            name = GetRandomLocalName(_ruMaleFirst, _ruMaleLast);
        }
        else if (rusPlayer && !male) {
            name = GetRandomLocalName(_ruFemaleFirst, _ruFemaleLast);
        }
        else if (!rusPlayer && male) {
            name = GetRandomLocalName(_enMaleFirst, _enLast);
        }
        else {
            name = GetRandomLocalName(_enFemaleFirst, _enLast);
        }
        
        
        if (name.Length > _maxCharsName) {
            name = AddNumbersInName(name);
        }
        return name;
    }

    private string GetRandomLocalName(string[] firstName, string[] lastName) {
        return firstName[Random.Range(0, firstName.Length)] + 
               " " +  
               lastName[Random.Range(0, lastName.Length)];
    }

    private string AddNumbersInName(string name) {
        string[] names = name.Split(' ');
        string newName = names[0];
        if (Random.value > 0.5) newName = names[1];
        
        if (newName.Length > _maxCharsName) {
            return newName.Substring(0, _maxCharsName);
        }

        // Свободное пространство под символы
        int freePlaces = _maxCharsName - newName.Length;
        freePlaces = Mathf.Clamp(freePlaces, 0, _maxDigitCount);
        

        // Добавляем случайные цифры
        string suffix = RandomDigits(0, freePlaces);

        return newName + suffix;
    }

    private string RandomDigits(int minLength, int maxLength) {
        int length = Random.Range(minLength, maxLength + 1);
        string result = "";
        for (int i = 0; i < length; i++)
            result += Random.Range(0, 10).ToString();
        return result;
    }

  
}
