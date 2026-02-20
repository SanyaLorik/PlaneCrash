using System;
using System.Collections.Generic;
using System.Linq;
using Architecture_M;
using UnityEngine;using UnityEngine.InputSystem.iOS;
using Zenject;





public abstract class UpgradeBase : MonoBehaviour {
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private DelayedTrigger _delayedTrigger;
    [SerializeField] protected UpgradeType UpgradeType;
    [SerializeField] protected UpgradeItemVisual _visual;
    
    
    
    protected UpgradeInfo UpgradeInfo;
    
    
    protected int _level = 1;
    protected double _currentPrice;
    protected IPlayerStatsWritable _playerStats;
    protected PlayerBank _bank;
    
    
    [Inject] protected UpgradeConfig _config;
    [Inject] protected UpgradesCalculator _upgradesCalculator;
    [Inject] protected PlayerVisual _playerVisual;
    [Inject] protected IGameSave<GameSavePC> _gameSave;
    [Inject] protected LocalizationDataPC _localization;
    
    
    [Inject]
    public void Init(PlayerBank bank, IPlayerStatsWritable playerStats) {
        _playerStats = playerStats;
        _bank = bank;
        _bank.BankChanged += BankOnBankChanged;
    }

    protected void Start() {
        LoadLevel();
    }



    private void BankOnBankChanged(long playerCapital) {
        CheckColor();
    }

    protected void CheckColor() {
        if (_bank.CanBuy(_currentPrice)) {
            _visual.SetGreen();
            return;
        }
        _visual.SetRed();
    }

    private void OnTriggerEnter(Collider collider) {
        if (!collider.TryGetComponent(out PlayerMovement _)) return;

        _delayedTrigger.DelayedTriggerAction(TryBuy); 
    }

    
    private void OnTriggerExit(Collider other) {
        if (!other.TryGetComponent(out PlayerMovement _))
            return;
        _delayedTrigger.CancelTriggerAction();
    }
    
    private void TryBuy() {
        if (_bank.CanBuy(_currentPrice)) {
            ApplyUpgrade();
            _particleSystem.Play();
            _level = _gameSave.GetSave.AddNewUpgrade(UpgradeInfo.Id);
            _playerVisual.SetBought();
            _gameSave.Save();
        }
        else {
            Debug.LogWarning("Не хватает срэдств(");
        }
    }




    protected abstract void ApplyUpgrade();
    protected abstract void UpdateVisual();
    protected abstract void LoadLevel();

}
