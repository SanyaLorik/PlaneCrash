using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using Zenject;

public class DualLegParticles : MonoBehaviour {
    public ParticleSystem _ps1; // одна система
    private ParticleSystem.EmissionModule _emission;

    [Inject] private PlayerMovement _playerMovement;

    private void Awake() {
        _emission = _ps1.emission;
        StartCoroutine(StartSystem());
    }

    private IEnumerator StartSystem() {
        StartRunning();
        yield return null;
        StopRunning();
    }
    

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
        _emission.enabled = true; 
       
    }

    private void StopRunning() {
        _emission.enabled = false; 
    }


   
}
