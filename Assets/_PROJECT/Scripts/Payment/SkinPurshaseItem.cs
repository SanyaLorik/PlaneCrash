using System;
using System.Collections.Generic;
using Architecture_M;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;


[Serializable]
public class SkinPurshaseItem : PurshaseItem
{
    [SerializeField] private SkinItemConfig[] _skinsInStore;
    
    [Inject] private PlayerSkinInventory _playerSkinInventory;
    [Inject] private List<SkinItemConfig> _skinConfigs;

    public override void Receive() 
    {
        BindReceiveAsync();
    }

    private async void BindReceiveAsync() 
    {
        await UniTask.WaitUntil(() => _playerSkinInventory != null && _bank != null);
        SavePurchasedStatus();
        foreach (var skin in _skinsInStore) 
        {
            if (_playerSkinInventory.SkinIsBought(skin.Id)) 
            {
                _bank.AddMoney(skin.Price);
                Debug.Log("Куплен уже имеющийся скин " + skin.Id);
            }
            else 
            {
                _playerSkinInventory.UnlockSkin(skin);
            }
        }
        _playerSkinInventory.EquipSkin(_skinsInStore[0]);
    }


}
