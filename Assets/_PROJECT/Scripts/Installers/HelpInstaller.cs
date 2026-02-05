using UnityEngine;
using Zenject;

public class HelpInstaller: MonoInstaller {
    
    
    public override void InstallBindings() {
        BindMoneyVisualLogic();
        SpawnBoost();
        ZonesBind();
        LevelBoundsBind();
        TrapsBoundsBind();

        TutorialBind();
    }

    private void BindMoneyVisualLogic() {
        Container.Bind<MoneyCube>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Money2dSpawner>().FromComponentInHierarchy().AsSingle();
    }
    
    private void SpawnBoost() {
        Container.Bind<BoostSpawner>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
    
    private void ZonesBind() {
        Container.Bind<ZoneManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
    
    private void LevelBoundsBind() {
        Container.Bind<LevelBounds>().FromComponentInHierarchy().AsSingle().NonLazy();
    } 
    
    
    private void TrapsBoundsBind() {
        Container.Bind<TrapPositionCalculator>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
    
    
    private void TutorialBind() {
        Container.Bind<Narrator>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
    
    
    

}