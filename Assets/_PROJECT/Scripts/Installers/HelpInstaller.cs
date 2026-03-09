using System;
using UnityEngine;
using Zenject;

[Serializable]
public class NicknameSettings {
    public int MaxCharsName;
    public int MaxDigitCount;
    [Range(0,1), SerializeField] public float ChanceToHaveNumber;
    [Range(0,1), SerializeField] public float ChanceToRusName;
    [Range(0,1), SerializeField] public float ChanceToMale;
}


public class HelpInstaller: MonoInstaller {
    [SerializeField] private NicknameSettings _nicknameSettings;
    
    public override void InstallBindings() {
        BindMoneyVisualLogic();
        SpawnBoost();
        ZonesBind();
        LevelBoundsBind();
        TrapsBoundsBind();
        TutorialBind();
        
        Container.Bind<ObjectPoolManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<NumberFormatter>().AsSingle();
        Container.Bind<RectTransformHelper>().AsSingle();
        Container.Bind<TrampolineManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<SoundManager>().FromComponentInHierarchy().AsSingle();
        
        BindSettings();
        BindNicknameRandomizer();
    }

    private void BindSettings() {
        Container.Bind<SettingsManager>().FromComponentInHierarchy().AsSingle();
    }

    private void BindNicknameRandomizer() {
        Container.BindInstance(_nicknameSettings);
        Container.BindInterfacesAndSelfTo<NicknameRandomizer>().AsSingle();
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
        Container.Bind<TutorialCompiller>().FromComponentInHierarchy().AsSingle();
    }
}