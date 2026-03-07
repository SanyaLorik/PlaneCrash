using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RangPlatformUnit : MonoBehaviour {
    [SerializeField] private TMP_Text _moneyCountText;
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private TMP_Text _rangNameText;
    [SerializeField] private Image _rangImg;
    [SerializeField] private int _rangId;
    [SerializeField] private DelayedTrigger _trigger;

    [Inject] private NumberFormatter _formatter;
    [Inject] private LocalizationDataPC _localizationDataPC;
    [Inject] private RangConfig _config;
    [Inject] private PlayerBank _bank;
    
    private long _rewardMoney;
    private bool _moneyIsGet;

    
    private void OnTriggerEnter(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _) || _moneyIsGet) return;
        _trigger.DelayedTriggerAction(AddMoney);
    }

    private void OnTriggerExit(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _)) return;
        _trigger.CancelTriggerAction();
    }

    private void AddMoney() {
        if(_moneyIsGet) return;
        _moneyIsGet = true;
        _bank.AddMoney(_rewardMoney);
        _trigger.SetUnvailable();
    }
    

    private void Start() {
        SetData();
    }

    private RangData GetRangInfoById(int id) {
        foreach (var rang in _config.Rangs) {
            if (rang.Id == id) {
                return rang;
            }
        }
        return null;
    }

    private void SetData() {
        RangData rang = GetRangInfoById(_rangId);
        
        _moneyCountText.text = _formatter.ValuteFormatter(rang.RewardMoney);
        _rangImg.sprite = rang.Sprite;
        
        _moneyText.text = _localizationDataPC.Receive;
        _rangNameText.text = _localizationDataPC.GetRangName(_rangId);
        _rewardMoney = rang.RewardMoney;
    }
}