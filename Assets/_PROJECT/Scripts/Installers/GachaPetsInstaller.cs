using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GachaPetsInstaller: MonoInstaller {
    [SerializeField] private PetStatusColorConfig  _petStatusColorConfig;
    [SerializeField] private List<PetItemConfig> _petConfigs;
    
    public override void InstallBindings() {
        Container.Bind<PetOpenView>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PetsManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PetStatusColorConfig>().FromScriptableObject(_petStatusColorConfig).AsSingle().NonLazy();
        BindPetItems();
    }

    private void BindPetItems() {
        Container.Bind<List<PetItemConfig>>()
            .FromInstance(_petConfigs)
            .AsSingle();
    }

}