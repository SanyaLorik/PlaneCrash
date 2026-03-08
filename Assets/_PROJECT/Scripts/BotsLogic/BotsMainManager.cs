using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
using IInitializable = Zenject.IInitializable;
using Random = UnityEngine.Random;

public class BotsMainManager : IInitializable, IDisposable {
    private readonly List<BotStateManager> _bots;
    private readonly List<SkinItemConfig> _skins;
    private readonly PlayerStateManager _playerStateManager;
    private readonly BotsManagerConfig _config;

    
    private CancellationTokenSource _tokenSource;
    private bool _stopBotSpeaking;
    private List<BotStateManager> _speakingBots = new();
    

    public BotsMainManager(List<BotStateManager> bots, PlayerStateManager playerStateManager, BotsManagerConfig config, List<SkinItemConfig> skins) {
        _bots = bots;
        _skins = skins;
        _config = config;
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerOnChangeState;
        // Debug.Log("Bot count: " + _bots.Count);
    }

    
    public void Initialize() {
        _tokenSource = new CancellationTokenSource();
        BotSpeakCycleAsync(_tokenSource.Token).Forget();
        foreach (var bot in _bots) {
            bot.SetBotSkin(_skins.GetRandomElement());
            bot.InitAnimator();
        }
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
        List<int> speakingBotsNumbers = GetNewSpeakingBotsNumbers(countSpeakBots);
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
    
    private List<int> GetNewSpeakingBotsNumbers(int count)
    {
        List<int> available = new List<int>();

        for (int i = 0; i < _bots.Count; i++)
        {
            if (_bots[i].State != BotState.Flight)
                available.Add(i);
        }

        List<int> result = new List<int>();

        count = Mathf.Min(count, available.Count);

        for (int i = 0; i < count; i++)
        {
            int r = Random.Range(0, available.Count);
            result.Add(available[r]);
            available.RemoveAt(r);
        }

        return result;
    }
    
    
    private void PlayerOnChangeState(PlayerState state){
        if (state == PlayerState.Flight) {
            int index = SetFlightRandomBot();
            index = GetRandomIndexExcept(_bots.Count, index);
            if (Random.value < _config.ChanseToChangeSkin) {
                _bots[index].SetBotSkin(_skins.GetRandomElement());
            }
            if (Random.value < _config.ChanseToChangeNickname) {
                _bots[index].ChangeNickname();
            }
        }

        else if(state == PlayerState.Walking) {
            CheckFloatingBots();
        }
    }

    private int GetRandomIndexExcept(int count, int previous) {
        int random = Random.Range(0, count-1);

        if (random >= previous) {
            random++;
        }
        return random;
    }

    private int SetFlightRandomBot() {
        int randomBot = GetRandomBot();
        // Debug.Log("Выбран бот: " + randomBot);
        _bots[randomBot].ChangeBotState(BotState.Flight);
        return randomBot;
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
