using UnityEngine;

public class RectTransformHelper {
    
    public void SetFillAmountWithPointer(RectTransform img,  RectTransform parent, RectTransform pointer, float percent, float offset = 0) {
        float xEnd = CalculateXEnd(parent);
        percent = Mathf.Clamp01(percent);
        img.offsetMax = new Vector2(GetXPoseByPercent(percent, xEnd, parent), 0);
        SetPointer(pointer, percent, xEnd, offset);
    }
    
    public void SetFillAmount(RectTransform img,  RectTransform parent, float percent) {
        percent = Mathf.Clamp01(percent);
        float xEnd = parent.rect.width;
        img.offsetMax = new Vector2(GetXPoseByPercent(percent, xEnd, parent), 0);
    }
    
    public void SetPointer(RectTransform pointer, float percent, float xEnd, float offset = 0) {
        percent = Mathf.Clamp01(percent);
        Vector2 newPointerPos = new Vector2(xEnd * percent + offset, pointer.anchoredPosition.y);
        pointer.anchoredPosition = newPointerPos;
    }

    public float CalculateXEnd(RectTransform parent) => parent.rect.width;

    public float Calculate1PeaceWidth(RectTransform parent, float peaceCount) 
        => parent.rect.width/peaceCount;

    
    public float GetYBottomScreen(RectTransform container, RectTransform pointer) {
        Canvas.ForceUpdateCanvases();
       // return -container.parent.GetComponent<RectTransform>().rect.height / 2 - container.rect.height / 2;
       Vector3 worldPos = pointer.position;
       // Конвертируем в локальные координаты родителя container
       Vector3 localPos = container.parent.InverseTransformPoint(worldPos);
       // Вычисляем смещение для pivot контейнера
       localPos.y -= container.rect.height * (1 - container.pivot.y);
       return localPos.y;
    } 
    
    
    public float GetYUnderScreen(RectTransform container, RectTransform pointer) {
        Canvas.ForceUpdateCanvases();
       // return -container.parent.GetComponent<RectTransform>().rect.height / 2 - container.rect.height / 2;
       Vector3 worldPos = pointer.position;
       // Конвертируем в локальные координаты родителя container
       Vector3 localPos = container.parent.InverseTransformPoint(worldPos);
       // Вычисляем смещение для pivot контейнера
       localPos.y += container.rect.height * (1 - container.pivot.y);
       return localPos.y;
    } 
    
    public Vector2 ClampByScreenVector(float padding, Vector2 point) {
        point.x = Mathf.Clamp(point.x, padding, Screen.width - padding);
        point.y = Mathf.Clamp(point.y, padding, Screen.height - padding);
        return point;
    }

    private float GetXPoseByPercent(float percent, float xEnd, RectTransform parent) {
        if (xEnd < 0) {
            Canvas.ForceUpdateCanvases();
            xEnd = parent.rect.width;
        }
        return -xEnd * (1f - percent);
    }
    
    private Vector2 GetRandomPointInCircle(float radius) {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        // Корень — чтобы точки были равномерно, а не кучей в центре
        float r = Mathf.Sqrt(Random.value) * radius;

        float x = Mathf.Cos(angle) * r;
        float y = Mathf.Sin(angle) * r;

        return new Vector2(x, y);
    }
    
    
    
    public Vector2 GetPointAroundPoint(float radius, Vector3 playerPosition) {
        Vector2 offset = GetRandomPointInCircle(radius);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(playerPosition);
        Vector2 point = new Vector2(offset.x + screenPos.x, offset.y + screenPos.y);

        float padding = 100f; // чтобы текст не упирался в край

        point.x = Mathf.Clamp(point.x, padding, Screen.width - padding);
        point.y = Mathf.Clamp(point.y, padding, Screen.height - padding);

        return point;
    }
    
    
    
}
