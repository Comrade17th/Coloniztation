using UnityEngine;
using DG.Tweening;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Vector3 _position = new Vector3(0, 360, 0);
    [SerializeField] private float _duration = 5;
    [SerializeField] private int _repeats = -1;
    [SerializeField] private LoopType _loopType = LoopType.Restart;
    [SerializeField] private RotateMode _rotateMode = RotateMode.FastBeyond360;
    
    private void Start()
    {
        transform.DORotate(_position, _duration, _rotateMode)
            .SetLoops(_repeats, _loopType)
            .SetRelative()
            .SetEase(Ease.Linear);
    }
}
