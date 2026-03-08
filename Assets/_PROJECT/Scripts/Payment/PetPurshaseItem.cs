using System;
using Architecture_M;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
[Serializable]

public struct PetPurchaseData 
{
    public PetItemConfig Pet;
    public int count;
}

[Serializable]
public class PetPurshaseItem : PurshaseItem
{
    [SerializeField] private PetPurchaseData[] _petsInfo;
    
    [Inject] private PetsManager _petsManager;
    [Inject] private IGameSave<GameSavePC> _saver;
    
    public override void Receive() {
        BindReceiveAsync();
    }
    
    private async void BindReceiveAsync() 
    {
        await UniTask.WaitUntil(() => _petsManager != null && _saver != null);
        _petsInfo.ForEach(p => _petsManager.AddPet(p.Pet, p.count, false));
        _saver.Save();
    }

}