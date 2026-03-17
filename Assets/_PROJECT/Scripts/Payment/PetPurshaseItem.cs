using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public override void Receive() 
    {
        BindReceiveAsync();
    }
    
    private async void BindReceiveAsync() 
    {
        await UniTask.WaitUntil(() => _petsManager != null && _saver != null);
        SavePurchasedStatus();
        foreach (var purchasePet in _petsInfo) 
        {
            if (_saver.GetSave.Pets.Any(p => p.Id == purchasePet.Pet.Id)) 
            {
                Debug.Log($"Питомец {purchasePet.Pet.Id} уже имеется");
                _bank.AddMoney(purchasePet.Pet.PriceIfBought);
            }
            else 
            {
                _petsManager.AddPet(purchasePet.Pet, purchasePet.count, false);
            }
        }
        _saver.Save();
    }

}