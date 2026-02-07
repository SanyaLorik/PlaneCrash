using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using IInitializable = Zenject.IInitializable;
using Random = UnityEngine.Random;

public class BotsMainManager : IInitializable, IDisposable {
    private readonly List<BotStateManager> _bots;
    private readonly PlayerStateManager _playerStateManager;
    private readonly BotsManagerConfig _config;

    
    private CancellationTokenSource _tokenSource;
    private bool _stopBotSpeaking;
    private List<BotStateManager> _speakingBots = new();
    

    [Inject]
    public BotsMainManager(List<BotStateManager> bots, PlayerStateManager playerStateManager, BotsManagerConfig config) {
        _bots = bots;
        _config = config;
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerOnChangeState;
        // Debug.Log("Bot count: " + _bots.Count);
    }

    
    public void Initialize() {
        _tokenSource = new CancellationTokenSource();
        BotSpeakCycleAsync(_tokenSource.Token).Forget();
    }

    private async UniTask BotSpeakCycleAsync(CancellationToken token) {
        await UniTask.Delay(1000, cancellationToken: token);
        while (!_stopBotSpeaking) {
            float timeToSpeak = Random.Range(_config.TimeToSpeak.From,  _config.TimeToSpeak.To);
            // Debug.Log("Speaking time" + timeToSpeak);
            await BotSpeakTimerAsync(timeToSpeak, token);
        }
    } 
    
    private async UniTask BotSpeakTimerAsync(float time, CancellationToken token) {
        SetBotsSpeak();
        float elpsedTime = 0;
        while (elpsedTime < time) {
            elpsedTime += Time.deltaTime;
            await  UniTask.Yield(token);
        }
        SetBotsStfu();
    }

    private void SetBotsSpeak() {
        int countSpeakBots = GetCountSpeakingBots();
        // Debug.Log("Говорящих ботов: " + countSpeakBots);
        List<int> speakingBotsNumbers = BotsNumbers(countSpeakBots);
        foreach (var bot in speakingBotsNumbers) {
            _speakingBots.Add(_bots[bot]);
            _bots[bot].SetBotSpeak();
        }
    }
    
    private void SetBotsStfu() {
        foreach (var bot in _speakingBots) {
            bot.SetBotStfu();
        }
        _speakingBots.Clear();
    }

    private int GetCountSpeakingBots() {
        int from = Mathf.Clamp(_config.CountSpeakingBotsPerTime.From, 0, _bots.Count);
        int to = Mathf.Clamp(_config.CountSpeakingBotsPerTime.To, 0, _bots.Count);
        
        return Random.Range(from, to);
    }
    
    private List<int> BotsNumbers(int count) {
        List<int> numbers = new List<int>();
        int iterations = 0;
        while (numbers.Count < count && iterations < 1000) {
            int nextNumber = Random.Range(0, _bots.Count);
            if (!numbers.Contains(nextNumber)) {
                numbers.Add(nextNumber);
                // Debug.Log("Номер выбранного бота: " + nextNumber);
            }
            iterations++;
        }
        // Debug.Log(iterations);
        return numbers;
    }
    
    
    


   
    
    private void PlayerOnChangeState(PlayerState state){
        if (state == PlayerState.Flight) {
            SetFlightRandomBot();
        }

        if (state == PlayerState.Walking) {
            CheckFloatingBots();
        }
    }

    private void SetFlightRandomBot() {
        int randomBot = GetRandomBot();
        // Debug.Log("Выбран бот: " + randomBot);
        _bots[randomBot].ChangeBotState(BotState.Flight);
    }
    

    private int GetRandomBot() {
        return Random.Range(0, _bots.Count);
    }

    private void CheckFloatingBots() {
        foreach (var bot in _bots) {
            if (bot.State == BotState.Flight) {
                bot.PlayerInSpawn();
            }
        }
    }


    public void Dispose() {
        _playerStateManager.ChangeState -= PlayerOnChangeState;
    }
}
