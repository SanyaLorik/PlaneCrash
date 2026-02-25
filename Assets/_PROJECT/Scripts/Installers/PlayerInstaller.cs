using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller {
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private UpgradeConfig _upgradesConfig;
    [SerializeField] private SkinItemConfig[] _skinItemConfigs;
    
    
    public override void InstallBindings() {
        BindPlayer();
        BindPlayerStats();
        
        BindUpgrades();
    }

    private void BindPlayer() {
        Container.Bind<PlayerConfig>().FromInstance(_playerConfig).AsSingle().NonLazy();
        Container.Bind<PlayerBank>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayerMovement>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayerStateManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<LineToObjects>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<TasksManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayerVisual>().FromComponentInHierarchy().AsSingle().NonLazy();
        BindSkins();
    }

    private void BindPlayerStats() {
        Container.BindInterfacesAndSelfTo<PlayerStats>().AsSingle().NonLazy();
    }
    
    private void BindSkins() {
        Container.Bind<PlayerSkinWear>().FromComponentInHierarchy().AsSingle().NonLazy();
        
        Container.Bind<SkinItemConfig[]>()
            .FromInstance(_skinItemConfigs)
            .AsSingle();
    }
    
    private void BindUpgrades() {
        Container.Bind<UpgradesCalculator>().AsSingle().NonLazy();
        Container.Bind<UpgradeConfig>().FromInstance(_upgradesConfig).AsSingle().NonLazy();

    }
    

    
    

}
