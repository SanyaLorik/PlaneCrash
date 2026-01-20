using UnityEngine;
using Zenject;

public class RangInstaller: MonoInstaller {
    [SerializeField] private RangConfig _config;
    
    
    public override void InstallBindings() {
        BindConfig();
    }

    private void BindConfig() {
        Container.Bind<RangConfig>().FromInstance(_config).AsSingle().NonLazy();
    }

}