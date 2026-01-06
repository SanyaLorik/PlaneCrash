using System;
using UnityEngine;

public class GroundChecker : MonoBehaviour {
    [SerializeField] LayerMask _groundMask;
    [SerializeField] private float _distanceCheck = 0.1f;
    public bool Grounded { get; private set; }
    
    public event Action OnGrounded;
    private void Update() {
        CheckGround();
    }


    private void CheckGround() {
        Vector3 origin = transform.position;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit,  _distanceCheck, _groundMask)) {
            OnGrounded?.Invoke();
            Debug.Log("Упали");
            Grounded = true;
        }
    }
}
