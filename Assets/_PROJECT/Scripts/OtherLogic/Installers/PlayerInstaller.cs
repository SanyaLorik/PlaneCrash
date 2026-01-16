using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller {
    [SerializeField] private PlayerConfig _config;
    [SerializeField] private PlayerBank _bank;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerStateManager _stateManager;
    [SerializeField] private BoostSpawner _boostSpawner;
    
    
    public override void InstallBindings() {
        BindPlayer();
        SpawnBoost();
    }

    private void BindPlayer() {
        Container.Bind<PlayerConfig>().FromInstance(_config).AsSingle();;
        Container.Bind<PlayerBank>().FromInstance(_bank).AsSingle().NonLazy();
        Container.Bind<PlayerMovement>().FromInstance(_movement).AsSingle().NonLazy();
        Container.Bind<PlayerStateManager>().FromInstance(_stateManager).AsSingle().NonLazy();
    }
    
    private void SpawnBoost() {
        Container.Bind<BoostSpawner>().FromInstance(_boostSpawner).AsSingle().NonLazy();
    }
    
    

}
