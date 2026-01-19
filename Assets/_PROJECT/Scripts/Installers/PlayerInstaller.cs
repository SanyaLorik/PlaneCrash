using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller {
    [SerializeField] private PlayerConfig _config;
    [SerializeField] private PlayerBank _bank;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerStateManager _stateManager;
    [SerializeField] private BoostSpawner _boostSpawner;
    [SerializeField] private LineToBoosts _lineToBoosts;
    
    
    public override void InstallBindings() {
        BindPlayer();
        SpawnBoost();
    }

    private void BindPlayer() {
        Container.Bind<PlayerConfig>().FromInstance(_config).AsSingle().NonLazy();
        Container.Bind<PlayerBank>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayerMovement>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayerStateManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<LineToBoosts>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
    
    private void SpawnBoost() {
        Container.Bind<BoostSpawner>().FromInstance(_boostSpawner).AsSingle().NonLazy();
    }
    
    

}
