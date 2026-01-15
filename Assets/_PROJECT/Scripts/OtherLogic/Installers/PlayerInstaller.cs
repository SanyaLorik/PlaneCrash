using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller {
    [SerializeField] private PlayerConfig _config;
    [SerializeField] private PlayerBank _bank;
    [SerializeField] private PlayerMovement _movement;
    
    
    public override void InstallBindings() {
        BindPlayerConfig();
        BindPlayerBank();
        BindPlayerMovement();
    }

    private void BindPlayerConfig() {
        Container.Bind<PlayerConfig>().FromInstance(_config);
    }
    
    private void BindPlayerBank() {
        Container.Bind<PlayerBank>().FromInstance(_bank).AsSingle().NonLazy();
    }
    
    private void BindPlayerMovement() {
        Container.Bind<PlayerMovement>().FromInstance(_movement).AsSingle().NonLazy();
    }
}
