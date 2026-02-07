using System;
using System.Collections.Generic;
using UnityEngine;

public class TrapVisual : MonoBehaviour {
     
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private float _brightnessMultiply = 5f;

    public void GetEffect() {
        _particleSystem.Play();
    }
    
    public void StopEffect() {
        _particleSystem.Stop();
    }
}
