using System;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class TrapObject : MonoBehaviour {
    [Header("Explosion settings")]
    [SerializeField] private float _radius = 6f;
    [SerializeField] private float _force = 800f;
    [SerializeField] private float _upwardModifier = 1.5f;
    [SerializeField] private ParticleSystem _particleSystem;

    private Rigidbody _playerRb;

    
    
    private void OnTriggerEnter(Collider collider) {
        Debug.Log("OnTriggerEnter!");
        if (collider.TryGetComponent(out PlayerMovement movement)) {
            movement.SetPlayerIsBombed();
        }
        if (collider.TryGetComponent(out Rigidbody rb)) {
            _particleSystem.Play();
            Explode(collider.GetComponent<Rigidbody>());
        }
        // gameObject.DisactiveSelf();
    }
    
    
    private void Explode(Rigidbody rb) {
        rb.linearVelocity = Vector3.zero;
        rb.AddExplosionForce(
            _force,
            transform.position,
            _radius,
            _upwardModifier,
            ForceMode.Impulse
        );
    }


}
