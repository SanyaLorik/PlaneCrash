using UnityEngine;
using Zenject;

public class HelpInstaller: MonoInstaller {
    
    
    public override void InstallBindings() {
        BindCube();
        SpawnBoost();
        ZonesBind();
        LevelBoundsBind();
    }

    private void BindCube() {
        Container.Bind<MoneyCube>().FromComponentInHierarchy().AsSingle();
    }
    
    private void SpawnBoost() {
        Container.Bind<BoostSpawner>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
    
    private void ZonesBind() {
        Container.Bind<ZoneManager>().FromComponentInHierarchy().AsSingle().NonLazy();;
    }
    
    private void LevelBoundsBind() {
        Container.Bind<LevelBounds>().FromComponentInHierarchy().AsSingle().NonLazy();;
    }
}