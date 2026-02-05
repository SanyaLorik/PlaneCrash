using System;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using Zenject;

public class DualLegParticles : MonoBehaviour {
    public ParticleSystem _ps1; // одна система

    [Inject] private PlayerMovement _playerMovement;


    private bool _needPlay = false;
    private void Update() {
        Vector2 move = _playerMovement.MoveInput;

        if (move == Vector2.zero && _needPlay) {
            _needPlay = false;
            StopRunning();
        }
        else if (move != Vector2.zero && !_needPlay) {
            _needPlay = true;
            StartRunning();
        }
    }


    private void StartRunning() {
        var emission1 = _ps1.emission;
        emission1.enabled = true; 
       
    }

    private void StopRunning() {
        var emission1 = _ps1.emission;
        emission1.enabled = false; 
        
    }


   
}
