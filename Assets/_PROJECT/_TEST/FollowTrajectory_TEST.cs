using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;

public class FollowTrajectory_TEST : MonoBehaviour
{
    public Transform _player;
    public Transform[] _points;
    public AnimationCurve _trjectory;
    public float _height;
    public float _duration;

    private void Start()
    {
        Follow();
    }

    private async UniTask Follow()
    {
        for (int i = 0; i < _points.Length - 1; i++)
        {
            Vector3 initial = _points[i].position;
            Vector3 final = _points[i + 1].position;

            float expandedTime = 0;

            do
            {
                float normalizedTime = expandedTime / _duration;

                float height = _trjectory.Evaluate(normalizedTime) * _height;
                float y = Mathf.Lerp(initial.y, final.y, normalizedTime) + height;
                float z = Mathf.Lerp(initial.z, final.z, normalizedTime);

                _player.position = _player.position
                    .SetY(y)
                    .SetZ(z);

                expandedTime += Time.deltaTime;
                await UniTask.Yield();
            }
            while (expandedTime < _duration);
        }
    }
}
