
using System;
using System.Linq;
using UnityEngine;
using Zenject;

public class RangManager : IInitializable, IDisposable {
    public RangData NextRang { get; private set; }
    public RangData CurrentRang { get; private set; }
    
    public event Action RangChanged;
    
    
    [Inject] PlayerBank _bank;
    [Inject] private RangConfig _config;


    
    public void Initialize() {
        _bank.BankChanged += BankOnBankChanged;
        BankOnBankChanged(_bank.PlayerCapital);
        Debug.Log("Уже ранг: " + CurrentRang.Money);
    }
    


    private void BankOnBankChanged(long amount) {
        var nextRangIndex = GetNextRangIndex(amount);
        RangData newRang = _config.Rangs[nextRangIndex];
        if (newRang != NextRang) {
            NextRang = newRang;
            if (newRang == _config.Rangs[^1] || nextRangIndex == 0) {
                CurrentRang = NextRang;
            }
            else {
                CurrentRang = _config.Rangs[nextRangIndex-1];
            }
            RangChanged?.Invoke();
            Debug.Log("ранг сменился на " + NextRang);
        }
    }
    
    
    
    public long GetNextRangePercentage(double percentage) {
        return (long)(percentage * NextRang.Money);
    }

    
    private int GetNextRangIndex(long playerAmount) {
        int nextRangIndex = _config.Rangs.FindIndex(r => r.Money >= playerAmount);
        Debug.Log("Индекс next ранга равен " + nextRangIndex);
        return nextRangIndex == -1 ? _config.Rangs.Count-1 : nextRangIndex;
    }

    private int GetRangIndex(RangData rangInfo) {
        for (int i = 0; i < _config.Rangs.Count; i++) {
            if (_config.Rangs[i] == rangInfo) {
                return i;
            }
        }
        return _config.Rangs.Count-1;
    }
    
    
    
    public void Dispose() {
        _bank.BankChanged -= BankOnBankChanged; 
    }
}