using System;
using UnityEngine;
using Zenject;

public enum BotState {
    Wandering,
    Flight
}


public class BotStateManager : MonoBehaviour {
    private BotFlight _botFlight;
    private BotWander _botWander;
    private BotMonolog _botMonolog;
    
    private IBotBehaviour _currentBotBehaviour;

    public BotState State { get; private set; }
    
    
    private void Awake() {
        _botFlight = GetComponent<BotFlight>();
        _botWander = GetComponent<BotWander>();
        _botMonolog = GetComponent<BotMonolog>();

        _botFlight.EndFlight += BotFlightOnEndFlight;
        _currentBotBehaviour = _botWander;
        State = BotState.Wandering;
    }
 
    
    

    public void ChangeBotState(BotState newState) {
        _currentBotBehaviour?.Exit();
        
        State = newState;
        _currentBotBehaviour = State switch {
            BotState.Flight => _botFlight,
            BotState.Wandering => _botWander,
            _ => _currentBotBehaviour
        };

        // Debug.Log(_currentBotBehaviour);
        _currentBotBehaviour?.Enter();
    }
    
    public void PlayerInSpawn() {
        _botFlight.GoToFall();
    }

    public void SetBotSpeak() {
        _botMonolog.SaySomething();
    }

    public void SetBotStfu() {
        _botMonolog.Stfu();
    }
    
    
    
    private void BotFlightOnEndFlight() {
        ChangeBotState(BotState.Wandering);
    }


    private void OnDisable() {
        _botFlight.EndFlight -= BotFlightOnEndFlight;
    }
}
