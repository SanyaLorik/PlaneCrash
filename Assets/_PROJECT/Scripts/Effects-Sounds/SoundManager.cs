using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class SoundManager : MonoBehaviour {
    // CORE
    [Header("Конфиги")]
    [SerializeField] private List<SoundConfig> soundConfigs;
    [Header("Тонкая настройка")]
    [SerializeField] private int _poolSize;
    [SerializeField] private float _fadeTime = 1f;
    [SerializeField] private float _stepTiming = 0.2f;
    [SerializeField] private GameObject _audioSourcesComponent;
    [Header("Background Music")]
    [SerializeField] private AudioSource _walkMusicSource;
    [SerializeField] private AudioSource _flyMusicSource;
    
    
    private Dictionary<SoundType, SoundConfig> _soundConfigDict = new ();
    private List<AudioSource> _sources = new();
    
    [Inject] private PlayerStateManager _stateManager;
    [Inject] private PlayerMovement _playerMovement;
    [Inject] private PlayerBank _bank;
    
    
    private void Awake() {
        foreach (var _sound in soundConfigs) {
            _soundConfigDict[_sound.SoundType] = _sound;
        }
       
        // создаём пул
        for (int i = 0; i < _poolSize; i++) {
            CreateNewAudioSource();
        }
        PlayMusic(_soundConfigDict[SoundType.MainBackground]);
    }

    private AudioSource CreateNewAudioSource() {
        AudioSource source = _audioSourcesComponent.AddComponent<AudioSource>();
        _sources.Add(source);
        return source;
    }


    private void OnEnable() {
        _stateManager.ChangeState += StateManagerOnChangeState;
        _playerMovement.JumpPressed += PlayerMovementOnJumpPressed;
        _playerMovement.DoubleJumpPressed += PlayerMovementOnJumpPressed;
        _playerMovement.RunningStateChanged += PlayerMovementOnRunningStateChanged;
        _playerMovement.Floored += PlayerMovementOnFloored;
        _playerMovement.SetBoost += PlayerMovementOnSetBoost;
        _bank.BankChanged += BankOnBankChanged;
    }

    private void BankOnBankChanged(long obj) {
       PlaySoundByType(SoundType.Money);
    }

    private void PlayerMovementOnSetBoost() {
        PlaySoundByType(SoundType.Boost);
    }


    private void OnDisable() {
        _stateManager.ChangeState -= StateManagerOnChangeState;
        _playerMovement.JumpPressed -= PlayerMovementOnJumpPressed;
        _playerMovement.DoubleJumpPressed -= PlayerMovementOnJumpPressed;
        _playerMovement.RunningStateChanged -= PlayerMovementOnRunningStateChanged;
        _playerMovement.Floored -= PlayerMovementOnFloored;
    }

    
    private void PlayerMovementOnFloored() {
        // Можно звук приземления
        _onAir = false;
        PlayerMovementOnRunningStateChanged(_playerMovement.IsRunning);
    }

    private CancellationTokenSource _cancellationTokenSource;
    private void PlayerMovementOnRunningStateChanged(bool isRunning) {
        _cancellationTokenSource?.Cancel();
        if (isRunning) {
            _cancellationTokenSource = new CancellationTokenSource();
            StepCycleAsync(_cancellationTokenSource.Token).Forget();
        }
    }

    private async UniTask StepCycleAsync(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            if (_onAir) {
                return;
            }
            PlaySoundByType(SoundType.Step);
            await UniTask.Delay(TimeSpan.FromSeconds(_stepTiming), cancellationToken: token);
        }
    }

    private bool _onAir;
    private void PlayerMovementOnJumpPressed() {
        PlaySoundByType(SoundType.Jump);
        _onAir = true;
    }

    private void PlaySoundByType(SoundType type) {
        if (!_soundConfigDict.TryGetValue(type, out var config)) {
            Debug.Log("Нет звука с типом " + type);
            return;
        }

        AudioClip clip = config.AudioClips.GetRandomElement();
        Debug.Log("Получен clip " + clip);
        AudioSource source = GetFreeSource();
        
        source.clip = clip;
        source.volume = config.Volume;
        source.pitch = UnityEngine.Random.Range(config.PitchDiapasone.From, config.PitchDiapasone.To);
        source.loop = config.Loop;
        source.outputAudioMixerGroup = config.MixerGroup;
        source.Play();
    }


    private AudioSource GetFreeSource() {
        foreach (var source in _sources) {
            if (!source.isPlaying)
                return source;
        }

        var newSource = CreateNewAudioSource();
        return newSource;
    }
    

    private void StateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.TrampolineJumping) return;
        
        if (state == PlayerState.Walking && _stateManager.BeforeState != PlayerState.TrampolineJumping) {
            // Проигрывание MainBackground
            _onAir = false;
            PlayMusic(_soundConfigDict[SoundType.MainBackground]);
        }
        else if (state == PlayerState.Flight) {
            // Проигрывание FlyBackground
            _onAir = true;
            PlayMusic(_soundConfigDict[SoundType.FlyBackground]);
        }
        else if (state == PlayerState.Grounded || state == PlayerState.Cruisered) {
            PlaySoundByType(SoundType.Win);
        }
        
    }
    


    private void PlayMusic(SoundConfig config) 
    {
        AudioSource targetSource = config.SoundType switch 
        {
            SoundType.MainBackground => _walkMusicSource,
            SoundType.FlyBackground => _flyMusicSource,
            _ => null
        };
        if (targetSource == null) return;

        // Если этот источник уже играет нужный клип - выходим
        if (targetSource.isPlaying && targetSource.clip == config.AudioClips[0])
            return;

        // Настраиваем источник
        targetSource.clip = config.AudioClips[0];
        targetSource.volume = 0f;
        targetSource.loop = config.Loop;
        targetSource.outputAudioMixerGroup = config.MixerGroup;
        targetSource.Play();

        // Плавно затухаем все другие источники (кроме target)
        foreach (var src in new[] { _walkMusicSource, _flyMusicSource })
        {
            if (src == targetSource) continue;

            if (src.isPlaying) 
            {
                src.DOFade(0f, _fadeTime).OnComplete(() =>
                {
                    src.Stop();
                    src.volume = 1f;
                });
            }
        }

        // Плавное появление нового трека
        targetSource.DOFade(config.Volume, _fadeTime);
    }



   
   
   
   
   
}
