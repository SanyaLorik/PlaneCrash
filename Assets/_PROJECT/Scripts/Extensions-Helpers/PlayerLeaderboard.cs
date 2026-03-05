using NaughtyAttributes;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;


public class PlayerLeaderboard : MonoBehaviour {
    [field: SerializeField] public bool IsTopPlayerLeaderboard { get; private set; }
    [SerializeField, ShowIf(nameof(IsTopPlayerLeaderboard))] private int _zeroMoneyIndex;
    
    [SerializeField] private PlayersListItemView[] _playerLists;
    [SerializeField] private PairedValue<long> _valueDiapasone;
    [SerializeField] private AnimationCurve _curve;

    
    [Header("Цвета")]
    [SerializeField] private Color[] _colors;
    [SerializeField, ShowIf(nameof(IsTopPlayerLeaderboard))] private Color _playerColor;
    
    
   
    
    
    
    [Inject] private NumberFormatter _formatter;
    [Inject] private PlayerBank _playerBank;
    [Inject] private LocalizationDataPC _localization;
    [Inject] private NicknameRandomizer _nicknameRandomizer;
    private bool _playerInTable;
    

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
            string name = _nicknameRandomizer.GetRandomName();
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
        _playerLists[^1].SetColor(_playerColor);
        float percent = (float) _playerBank.PlayerCapital / _playerLists[^1].MoneyAmount;
        int index = (int)Mathf.Lerp(_zeroMoneyIndex, _playerLists.Length-1, percent);
        _playerLists[^1].SetPlayerListData(index, _localization.You, _formatter.ValuteFormatterInteger(_playerBank.PlayerCapital));
    }

    

  
}
