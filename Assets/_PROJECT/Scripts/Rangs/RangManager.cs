
using System;
using System.Linq;
using UnityEngine;
using Zenject;

public class RangManager : IInitializable, IDisposable {
    public RangData CurrentRang { get; private set; }
    
    public event Action RangChanged;
    
    
    [Inject] PlayerBank _bank;
    [Inject] private RangConfig _config;
    
    
    public void Initialize() {
        CurrentRang = GetConfigByAmount(_bank.PlayerCapital);
        Debug.Log("текущий ранг: " + CurrentRang.Money);
        _bank.BankChanged += BankOnBankChanged; 
    }
    


    private void BankOnBankChanged(long amount) {
        RangData newRang = GetConfigByAmount(amount);
        if (newRang != CurrentRang) {
            CurrentRang = newRang;
            RangChanged?.Invoke();
            Debug.Log("ранг сменился на " + CurrentRang);
        }
    }
    
    
    
    public long GetCurrentRangePercentage(double percentage) {
        return (long)(percentage * CurrentRang.Money);
    }

    
    private RangData GetConfigByAmount(long playerAmount) {
        RangData rangInfo = _config.Rangs.FirstOrDefault(r => r.Money >= playerAmount)
                            ??
                            _config.Rangs.Last();
        return rangInfo;
    }
    
    
    
    public void Dispose() {
        _bank.BankChanged -= BankOnBankChanged; 
    }
}