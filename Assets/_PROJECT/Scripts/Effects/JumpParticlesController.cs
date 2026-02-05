using System;
using System.Collections;
using UnityEngine;

public class JumpParticlesController : MonoBehaviour  {
    [SerializeField] private ParticleSystem _flyPS;
    [SerializeField] private float _freezeDelay = 0.5f;
    [SerializeField] private float _freezeDuration = 0.2f;
    [SerializeField] private float _trailsEnabledTime = 0.05f;
    
    
    private ParticleSystem.Particle[] particles;
    private void Start() {
        EnableTrails(false);
    }

    private void EnableTrails(bool state) {
        ParticleSystem.TrailModule _trails = _flyPS.trails;
        _trails.enabled = state;
    }

    public void Play() {
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence() {
        // 1. Вылет
        _flyPS.Play();
        yield return new WaitForSeconds(_trailsEnabledTime);
        EnableTrails(true);
        yield return new WaitForSeconds(_freezeDelay-_trailsEnabledTime);
        FreezeParticles(_flyPS);
        EnableTrails(false);
    }
    
    
    private void FreezeParticles(ParticleSystem ps) {
        
        if (particles == null || particles.Length < ps.main.maxParticles)
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
        
        int count = ps.GetParticles(particles);

        for (int i = 0; i < count; i++) {
            particles[i].velocity = Vector3.zero;
        }

        ps.SetParticles(particles, count);
    }
    
}
