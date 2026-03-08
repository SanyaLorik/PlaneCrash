using System;
using Architecture_M;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class UpgradePurshaseItem : PurshaseItem
{
    [SerializeField] private UpgradeType _upgradeType;
    [SerializeField] private int _newLevels;
    
    [Inject] private UpgradeManager _upgradeManager;
    [Inject] private IGameSave<GameSavePC> _gameSave;
    

    public override void Receive() 
    {
        BindReceiveAsync();
    }

    private async void BindReceiveAsync() 
    {
        await UniTask.WaitUntil(() => _upgradeManager != null && _gameSave != null);
        _upgradeManager.AddNewUpgrade(_upgradeType, _newLevels);
        _gameSave.Save();
    }
}
