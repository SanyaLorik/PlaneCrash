using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TaskNotification : MonoBehaviour {
    [SerializeField] private TMP_Text _moneyText;
    
    
    [SerializeField] private RectTransform _panel;
    [SerializeField] private RectTransform _canvas;


    [SerializeField] private float _timeToShow;
    [SerializeField] private float _duration;
    
    
    

    private Vector2 _shownPos;
    private Vector2 _hiddenPos;
    private bool _notifIsShowed;
    
    private void Awake() {
        CachePositions();
        

        _panel.anchoredPosition = _hiddenPos; // сразу прячем
    }


    private Coroutine _notifCoroutine;
    public void ShowNotification(string money) {
        _moneyText.text = money;
        if (_notifCoroutine != null) {
            StopCoroutine(_notifCoroutine);
        }
        _notifCoroutine = StartCoroutine(NotificationRoutine());
    }

    private IEnumerator NotificationRoutine() {
        Show();
        yield return new WaitUntil(() => _notifIsShowed);
        yield return new WaitForSeconds(_timeToShow);
        Hide();
        _notifCoroutine = null;
    }
    
        
        
    private void Show() {
        _panel.DOAnchorPos(_shownPos, _duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => _notifIsShowed = true);
    }

    private void Hide() {
        _panel.DOAnchorPos(_hiddenPos, _duration)
            .SetEase(Ease.InBack)
            .OnComplete(() => _notifIsShowed = false);
        
    }
    
    private void CachePositions() {
        // Логика получения значения за экраном 
        _shownPos = _panel.anchoredPosition;
        float hideX = _canvas.rect.width / 2f + _panel.rect.width;
        _hiddenPos = new Vector2(hideX, _shownPos.y);
        
        _shownPos = _panel.anchoredPosition;
    }

    
    
}
