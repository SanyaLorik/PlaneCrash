using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FireThrowerMovement : TrapMovement {
    [SerializeField] private TrapVisual _visual;
    
    
    
    
    private Collider _collider;
    private Coroutine _flameCoroutine;
    public override void StartMove() {
        _pivot.localPosition = Vector3.zero;
        _pivot.position += _xOffsetVector;
        
        if (Mathf.Approximately(transform.position.x, _levelBounds.RightX)) {
            _pivot.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else {
            _pivot.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        _collider = GetComponent<Collider>();
        ResetTrap();
        _flameCoroutine = StartCoroutine(FlameCycle());
    }

    public override void ResetTrap() {
        if (_flameCoroutine != null) {
            StopCoroutine(_flameCoroutine);
            _flameCoroutine = null;
        }
    }


    private IEnumerator FlameCycle() {
        float waitTime = Random.Range(_durationDiapasone.From, _durationDiapasone.To);
        while (true) {
            _visual.GetEffect();
            yield return new WaitForSeconds(waitTime/5);
            _collider.enabled = true;
           
            yield return new WaitForSeconds(waitTime);
            
            _visual.StopEffect();
            _collider.enabled = false;
            yield return new WaitForSeconds(waitTime/3);
        }
    }
}
