using System;
using System.Collections.Generic;
using System.Linq;
using Architecture_M;
using UnityEngine;
using Zenject;

public abstract class UpgradeBase : MonoBehaviour {
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] protected int _id;
    [SerializeField] private DelayedTrigger _delayedTrigger;
    
    
    
    protected UpgradeInfo UpgradeInfo;
    
    
    protected int _level = 1;
    protected float _currentPrice;
    protected UpgradeItemVisual _visual;
    protected IPlayerStatsWritable _playerStats;
    protected PlayerBank _bank;
    
    
    [Inject] protected UpgradeConfig _config;
    [Inject] protected UpgradesCalculator _upgradesCalculator;
    [Inject] private PlayerVisual _playerVsual;
    [Inject] IGameSave<GameSavePC> _gameSave;
    
    
    
    [Inject]
    public void Init(PlayerBank bank, IPlayerStatsWritable playerStats) {
        _playerStats = playerStats;
        _bank = bank;
        _bank.BankChanged += BankOnBankChanged;
    }

    protected void Awake() {
        _visual = GetComponent<UpgradeItemVisual>();
        bool exist = _gameSave.GetSave.Upgrades.Any(upgrade => upgrade.Id == _id);
        if (!exist) {
            _gameSave.GetSave.Upgrades.Add(new UpgradeData {
                Id = _id,
                Level = 1,
            });
            _gameSave.Save();
        }
        else {
            _level = _gameSave.GetSave.Upgrades.First(upgrade => upgrade.Id == _id).Level;
        }
        LoadLevel();
    }



    private void BankOnBankChanged(float playerCapital) {
        CheckColor();
    }

    protected void CheckColor() {
        if (_bank.CanBuy(_currentPrice)) {
            _visual.SetGreen();
            _particleSystem.Play();
            
            return;
        }
        _visual.SetRed();
        _particleSystem.Stop();
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
            var upgrade = _gameSave.GetSave.Upgrades.First(upgrade => upgrade.Id == _id);
            upgrade.Level = _level;
            _playerVsual.SetBought();
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
