using UnityEngine;
using Zenject;

public class HelpInstaller: MonoInstaller {
    
    
    public override void InstallBindings() {
        BindCube();
    }

    private void BindCube() {
        Container.Bind<MoneyCube>().FromComponentInHierarchy().AsSingle();
    }

}