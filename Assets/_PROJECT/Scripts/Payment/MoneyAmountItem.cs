using System;
using Architecture_M;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class MoneyAmountItem : PurshaseItem
{
    [SerializeField] private long _moneyAmount;
    
    public override void Receive() 
    {
        BindReceiveAsync();
    }
    
    private async void BindReceiveAsync() 
    {
        SavePurchasedStatus();
        await UniTask.WaitUntil(() => _bank != null);
        _bank.AddMoney(_moneyAmount);
    }

}