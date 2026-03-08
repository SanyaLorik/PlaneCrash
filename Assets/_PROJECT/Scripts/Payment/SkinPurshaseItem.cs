using System;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;


[Serializable]
public class SkinPurshaseItem : PurshaseItem
{
    [SerializeField] private SkinItemConfig[] _skins;
    
    [Inject] private PlayerSkinInventory _playerSkinInventory;
    
    
    public override void Receive() 
    {
        BindReceiveAsync();
    }

    private async void BindReceiveAsync() 
    {
        await UniTask.WaitUntil(() => _playerSkinInventory != null);
        _skins.ForEach(s => _playerSkinInventory.UnlockSkin(s));
        _playerSkinInventory.EquipSkin(_skins[^1]);
    }
}
