using System;
using UnityEngine;

public class  LevelBounds : MonoBehaviour {
    
    // Потом наверное лучше добавить Collider вместо Renderer
    [SerializeField] private Renderer _floor;
    [SerializeField] private Renderer _leftWall;
    [SerializeField] private Renderer _rightWall;

    
    public float MinimumY { get;private set; }
    public float LeftX { get;private set; }
    public float RightX { get;private set; }
    
    private void Awake() {
        MinimumY = _floor.bounds.max.y + 0.5f;
        LeftX = _leftWall.bounds.max.x;
        RightX = _rightWall.bounds.min.x;
        Debug.Log($"RightX = {RightX}, LeftX = {LeftX}");
        Debug.Log($"MinimumY = {MinimumY}");
    }



    public Vector3 ClampPosition(Vector3 pos) {
        pos.x = Mathf.Clamp(pos.x, LeftX, RightX);
        pos.y = Mathf.Max(pos.y, MinimumY);
        return pos;
    }
}
