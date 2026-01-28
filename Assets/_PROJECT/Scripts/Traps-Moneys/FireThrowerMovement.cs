using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FireThrowerMovement : TrapMovement {
    [SerializeField] private TrapVisual _visual;
    
    
    
    
    private Collider _collider;
    public override void StartMove() {
        _pivot.localPosition = Vector3.zero;
        _pivot.position += _xOffsetVector;
        
        if (transform.position.x > 0) {
            Debug.Log("Поворот кулака бьет типо в другую сторону");
            _pivot.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        _collider = GetComponent<Collider>();
        StartCoroutine(FlameCycle());
    }


    private IEnumerator FlameCycle() {
        float waitTime = Random.Range(_durationDiapasone.From, _durationDiapasone.To);
        while (true) {
            _visual.GetEffect();
            yield return new WaitForSeconds(waitTime/3);
            _collider.enabled = true;
           
            yield return new WaitForSeconds(waitTime);
            
            _visual.StopEffect();
            _collider.enabled = false;
            yield return new WaitForSeconds(waitTime/3);
        }
    }
}
