using System;
using UnityEngine;
using Zenject;

public enum BotState {
    Flight,
    Wandering
}


public class BotStateManager : MonoBehaviour {
    private BotFlight _botFlight;
    private BotWander _botWander;
    
    private IBotBehaviour _currentBotBehaviour;
        
    public BotState State { get; private set; }
    
    
    private void Awake() {
        _botFlight = GetComponent<BotFlight>();
        _botWander = GetComponent<BotWander>();

        _botFlight.EndFlight += BotFlightOnEndFlight;
        _currentBotBehaviour = _botWander;
    }
 

    public void ChangeBotState(BotState newState) {
        _currentBotBehaviour?.Exit();
        
        State = newState;
        _currentBotBehaviour = State switch {
            BotState.Flight => _botFlight,
            BotState.Wandering => _botWander,
            _ => _currentBotBehaviour
        };

        Debug.Log(_currentBotBehaviour);
        _currentBotBehaviour?.Enter();
    }
    
    private void BotFlightOnEndFlight() {
        ChangeBotState(BotState.Wandering);
    }

    public void PlayerInSpawn() {
        _botFlight.GoToFall();
    }
    
}
