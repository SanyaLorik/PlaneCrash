using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BotsMainManager {
    private readonly List<BotStateManager> _bots;
    private readonly PlayerStateManager _player;


    [Inject]
    public BotsMainManager(List<BotStateManager> bots, PlayerStateManager player) {
        _bots = bots;
        Debug.Log("Bot count: " + _bots.Count);
        _player = player;
        _player.ChangeState += PlayerOnChangeState;
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
        Debug.Log("Выбран бот: " + randomBot);
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
}
