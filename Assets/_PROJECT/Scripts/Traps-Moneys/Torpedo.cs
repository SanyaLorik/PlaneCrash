using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using SanyaBeerExtension;
using Unity.VisualScripting;
using UnityEngine;

public class Torpedo : MonoBehaviour {
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private GameObject modelForHide;
    
    [SerializeField] private float _radius = 6f;
    [SerializeField] private float _force = 20f;
    [SerializeField] private float _upwardModifier = 1.3f;
    
    
    public float Speed;
    private Vector3 _targetPoint;

    public void Launch(Vector3 targetPoint, float speed) {
        _targetPoint = targetPoint;
        _targetPoint.y += 100f; // просто пусть выше летит
        Speed = speed;
        StartCoroutine(TorpedaFlightAsync());
    }


    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out PlayerMovement playerMovement)) {
            _particle.Play();
            if(playerMovement.TryToKill()) {
                Debug.Log("Убили игрока!!");
                Explode(playerMovement.Rb);
                modelForHide.DisactiveSelf();
                StartCoroutine(DestroyRoutine(.3f));
            }
            else {
                StartCoroutine(DestroyRoutine(2f));
            }
        }
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


    private IEnumerator TorpedaFlightAsync() {
        while (true) {
            // Летаем по направлению вверх к цели
            Vector3 dir = (_targetPoint - transform.position).normalized;
            transform.position += dir * Speed * Time.deltaTime;

            // Проверка на попадание (по Y)
            if (Vector3.Distance(transform.position, _targetPoint) < 0.1f) {
                // Тут можно нанести урон игроку
                StartCoroutine(DestroyRoutine(3f));
            }
            yield return null;
        }
    }
    


    private IEnumerator DestroyRoutine(float time) {
        yield return new WaitForSeconds(time);
            Destroy(gameObject);
    }
}
