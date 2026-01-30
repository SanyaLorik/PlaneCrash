using System;
using System.Collections.Generic;
using UnityEngine;

public class TrapVisual : MonoBehaviour {
     
    [SerializeField] private List<ParticleSystem> _particleSystem;
    [SerializeField] private float _brightnessMultiply = 5f;

    public void GetEffect() {
        foreach (var effect in _particleSystem) {
            effect.Play();
        }
    }
    
    public void StopEffect() {
        foreach (var effect in _particleSystem) {
            effect.Stop();
        }
    }
}
