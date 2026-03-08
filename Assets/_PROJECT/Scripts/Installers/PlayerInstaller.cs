using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller {
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private UpgradeConfig _upgradesConfig;
    [SerializeField] private List<SkinItemConfig> _skinItemConfigs;
    [SerializeField] private SkinItemConfig  _defaultSkinConfig;
    
    
    public override void InstallBindings() {
        BindPlayer();
        BindPlayerStats();
        
        BindUpgrades();
    }

    private void BindPlayer() {
        Container.Bind<UpgradeManager>().FromComponentInHierarchy().AsSingle().NonLazy();
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
        
        Container.Bind<List<SkinItemConfig>>()
            .FromInstance(_skinItemConfigs)
            .AsSingle();
        
        // Skins
        Container.BindInterfacesAndSelfTo<PlayerSkinInventory>()
            .AsSingle()
            .WithArguments(_defaultSkinConfig)
            .NonLazy();
    }
    
    private void BindUpgrades() {
        Container.Bind<UpgradesCalculator>().AsSingle().NonLazy();
        Container.Bind<UpgradeConfig>().FromInstance(_upgradesConfig).AsSingle().NonLazy();

    }

}
