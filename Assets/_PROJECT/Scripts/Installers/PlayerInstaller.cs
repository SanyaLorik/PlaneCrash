using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller {
    [SerializeField] private PlayerConfig _config;
    
    
    public override void InstallBindings() {
        BindPlayer();
        BindPlayerStats();
    }

    private void BindPlayer() {
        Container.Bind<PlayerConfig>().FromInstance(_config).AsSingle().NonLazy();
        Container.Bind<PlayerBank>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayerMovement>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayerStateManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<LineToObjects>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<TasksManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        
        
    }

    private void BindPlayerStats() {
        Container.BindInterfacesAndSelfTo<PlayerStats>().AsSingle().NonLazy();
    }
    

    
    

}
