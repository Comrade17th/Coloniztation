using UnityEngine;
using DG.Tweening;
using System;

public class Radar : MonoBehaviour
{
    [SerializeField] private Vector3 _position = new Vector3(0, 360, 0);
    [SerializeField] private float _duration = 5;
    [SerializeField] private int _repeats = -1;
    [SerializeField] private LoopType _loopType = LoopType.Restart;
    [SerializeField] private RotateMode _rotateMode = RotateMode.FastBeyond360;

    public event Action<Resource> ResourceFinded;
   
    private void Start()
    {
        transform.DORotate(_position, _duration, _rotateMode)
            .SetLoops(_repeats, _loopType)
            .SetRelative()
            .SetEase(Ease.Linear);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Resource resource))
        {
            ResourceFinded?.Invoke(resource);
        }
    }
}
