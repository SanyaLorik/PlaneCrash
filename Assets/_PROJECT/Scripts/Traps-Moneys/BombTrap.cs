using UnityEngine;

public class BombTrap : TrapAttack {
    [Header("Explosion settings")]
    [SerializeField] private float _radius = 6f;
    [SerializeField] private float _force = 90f;
    [SerializeField] private float _upwardModifier = 1.4f;

    protected override void Atack(Rigidbody rb) {
        Explode(rb);
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
