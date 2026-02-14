using UnityEngine;
using Zenject;

public class GachaPetsInstaller: MonoInstaller {
    [SerializeField] private PetStatusColorConfig  _petStatusColorConfig;
    
    
    public override void InstallBindings() {
        Container.Bind<PetOpenView>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PetsManager>().AsSingle().NonLazy();
        Container.Bind<PetStatusColorConfig>().FromScriptableObject(_petStatusColorConfig).AsSingle().NonLazy();
    }

}