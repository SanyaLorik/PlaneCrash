using UnityEngine;
using Zenject;

public class BotInstaller: MonoInstaller {
    [SerializeField] private BotsManagerConfig _botsManagerConfig;
    
    public override void InstallBindings() {
        BindBotStateManager();
    }

    private void BindBotStateManager() {
        
        Container.BindInterfacesAndSelfTo<BotsMainManager>().AsSingle().NonLazy();

        Container.Bind<BotsManagerConfig>().FromScriptableObject(_botsManagerConfig).AsSingle().NonLazy();;
        
        
        Container.Bind<BotStateManager>()
            .FromComponentsInHierarchy()
            .AsTransient();
        
    }

}
