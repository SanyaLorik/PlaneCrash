using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Lift : MonoBehaviour {
    [SerializeField] private Renderer _rend;
    [SerializeField] private Renderer _transformRend;
    [SerializeField] private Transform _newPlayerParent;
    
    private Vector3 _startPos;
    private Vector3 _endPos;
    private CancellationTokenSource _tokenSource;

    private void Start() {
        _newPlayerParent.localPosition = Vector3.zero;
        _startPos =  _newPlayerParent.position;
        _transformRend = transform.GetComponent<Renderer>();
    }
    
    

    
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement playerMovement)) {
            collider.transform.SetParent(_newPlayerParent, true);
            playerMovement.gameObject.GetComponent<Rigidbody>().useGravity = false;
            float targetTop = _rend.bounds.max.y + 5f;           // куда хотим приехать
            
            float liftBottom = _transformRend.bounds.min.y;  // где сейчас низ лифта
            float delta = targetTop - liftBottom;          // сколько реально надо ехать вверх

            _endPos = _startPos + Vector3.up * delta;
            
            ReadyLiftWork();
            LiftUp(_tokenSource.Token).Forget();
        }    
    }

    private void ReadyLiftWork() {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
        _tokenSource = new CancellationTokenSource();
    }
    
    
    private void OnTriggerExit(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement playerMovement)) {
            playerMovement.gameObject.GetComponent<Rigidbody>().useGravity = true;
            collider.transform.SetParent(null);
            ReadyLiftWork();
            LiftDown(_tokenSource.Token).Forget();
            Debug.Log(_startPos);
            
        }    
    }

    

    private async UniTask LiftUp(CancellationToken token) {
        await UniTask.Delay(700, cancellationToken: token);
        
        float duration = 2f;
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            _newPlayerParent.position = Vector3.Lerp(_startPos, _endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            await UniTask.Yield(token);
        }
    }
    
    private async UniTask LiftDown(CancellationToken token) {
        
        float duration = 2f;
        float elapsedTime = 0f;
        _endPos = _newPlayerParent.position;
        while (elapsedTime < duration) {
            _newPlayerParent.position = Vector3.Lerp(_endPos, _startPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            await UniTask.Yield(token);
        }
    }
    
    
}
