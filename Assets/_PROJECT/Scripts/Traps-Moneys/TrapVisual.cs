using UnityEngine;

public class TrapVisual : MonoBehaviour {
     
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private float _brightnessMultiply = 5f;

    public void GetEffect() {
        if (_particleSystem!=null) {
            _particleSystem.Play();
        }
    }
    
    public void StopEffect() {
        if (_particleSystem!=null) {
            _particleSystem.Stop();
        }
    }
}
