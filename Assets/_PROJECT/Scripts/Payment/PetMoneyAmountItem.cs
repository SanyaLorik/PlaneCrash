using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class MoneyAmountItem : PurshaseItem
{
    [SerializeField] private long _moneyAmount;
    
    [Inject] private PlayerBank _bank;
    
    public override void Receive() {
        BindReceiveAsync();
    }
    
    private async void BindReceiveAsync() 
    {
        await UniTask.WaitUntil(() => _bank != null);
        _bank.AddMoney(_moneyAmount);
    }

}