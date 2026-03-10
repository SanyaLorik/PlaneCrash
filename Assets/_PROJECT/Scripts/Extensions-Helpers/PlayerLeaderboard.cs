using Architecture_M;
using NaughtyAttributes;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;


public class PlayerLeaderboard : MonoBehaviour {
    [field: SerializeField] public bool IsTopPlayerLeaderboard { get; private set; }
    [SerializeField, ShowIf(nameof(IsTopPlayerLeaderboard))] private int _zeroMoneyIndex;
    [SerializeField, ShowIf(nameof(IsTopPlayerLeaderboard))] private PairedValue<long> _valueDiapasone;
    
    [SerializeField] private PlayersListItemView[] _playerLists;
    [SerializeField] private AnimationCurve _curve;

    
    [Header("Цвета")]
    [SerializeField] private Color[] _colors;
    [SerializeField, ShowIf(nameof(IsTopPlayerLeaderboard))] private Color _playerColor;
    
    
    private bool _playerInTable;
    

    [Inject] private NumberFormatter _formatter;
    [Inject] private PlayerBank _playerBank;
    [Inject] private LocalizationDataPC _localization;
    [Inject] private NicknameRandomizer _nicknameRandomizer;
    private void OnEnable() {
        _playerBank.BankChanged += PlayerBankOnBankChanged;
    }

    private void PlayerBankOnBankChanged(long obj) {
        // IN DEV...
    }


    private void Start() {
        if (IsTopPlayerLeaderboard) {
            TopPlayersInit();
        }
        else {
            PlayersTopDonaterInit();
        }
    }
    
   
    private void TopPlayersInit() {
        _playerInTable = false;
        for (var i = 0; i < _playerLists.Length; i++) {
            long money = (long)Mathf.Lerp(
                _valueDiapasone.From,
                _valueDiapasone.To,
                _curve.Evaluate((float)(i + 1) / _playerLists.Length)
            );
            string name = _nicknameRandomizer.GetRandomName();
            // Нашли игрока
            if (_playerBank.PlayerCapital >= money && !_playerInTable) {
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

        if (_playerInTable) return;
        _playerLists[^1].SetColor(_playerColor);
        float percent = (float) _playerBank.PlayerCapital / _playerLists[^1].MoneyAmount;
        int index = (int)Mathf.Lerp(_zeroMoneyIndex, _playerLists.Length-1, percent);
        _playerLists[^1].SetPlayerListData(index, _localization.You, _formatter.ValuteFormatterInteger(_playerBank.PlayerCapital));
    }
    
    private void PlayersTopDonaterInit() {
        for (var i = 0; i < _playerLists.Length; i++) {
            if (i < _colors.Length) {
                _playerLists[i].SetColor(_colors[i]);
            }
            string name = _nicknameRandomizer.GetRandomName();
            _playerLists[i].SetPlayerListData(
                i + 1,
                name,
                string.Empty
            );
        }
    }

    

  
}
