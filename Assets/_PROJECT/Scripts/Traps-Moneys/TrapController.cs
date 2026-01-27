using System;
using UnityEngine;
using UnityEngine.Scripting;
using Zenject;


public class TrapController  : MonoBehaviour {
    [SerializeField] private TrapMovement _trapMovement;
    [SerializeField] private TrapAttack _trapAttack;
    public TrapRole TrapRole;

    
    
    
    [Inject]
    public void Init(BoostSpawner boostSpawner, LevelBounds levelBounds) {
        _trapAttack.Init(boostSpawner);
        _trapMovement.Init(levelBounds);
    }
    

    public void StartMoveTrap() {
        _trapMovement.StartMove();
    }
    
}
