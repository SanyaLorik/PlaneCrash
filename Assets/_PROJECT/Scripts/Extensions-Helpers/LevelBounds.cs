using System;
using UnityEngine;
using Zenject;

public class  LevelBounds : MonoBehaviour {
    
    // Потом наверное лучше добавить Collider вместо Renderer
    [SerializeField] private Renderer _floor;
    [SerializeField] private Renderer _leftWall;
    [SerializeField] private Renderer _rightWall;
    [SerializeField] private Renderer _cruiser;

    


    public Vector3 RecalculateCruiserY() {
       float cruiserY = _cruiser.bounds.max.y;
       CruiserPosition = new Vector3(_cruiser.transform.position.x, cruiserY, _cruiser.transform.position.z);
       return CruiserPosition;
    }


    public float MinimumY { get;private set; }
    public float LeftX { get;private set; }
    public float RightX { get;private set; }
    public Vector3 CruiserPosition { get;private set; }
    
    private void Awake() {
        MinimumY = _floor.bounds.max.y;
        LeftX = _leftWall.bounds.max.x;
        RightX = _rightWall.bounds.min.x;
        RecalculateCruiserY();
    }



    public Vector3 ClampPosition(Vector3 pos) {
        pos.x = Mathf.Clamp(pos.x, LeftX, RightX);
        pos.y = Mathf.Max(pos.y, MinimumY);
        return pos;
    }
}
