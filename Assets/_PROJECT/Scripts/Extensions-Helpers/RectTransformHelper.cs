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
        Vector2 newPointerPos = new Vector2(xEnd * percent + offset, pointer.anchoredPosition.y);
        pointer.anchoredPosition = newPointerPos;
    }

    public float CalculateXEnd(RectTransform parent) => parent.rect.width;

    

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
    
    

    private float GetXPoseByPercent(float percent, float xEnd, RectTransform parent) {
        if (xEnd < 0) {
            Canvas.ForceUpdateCanvases();
            xEnd = parent.rect.width;
        }
        return -xEnd * (1f - percent);
    }
}
