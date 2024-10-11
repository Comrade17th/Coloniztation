using System.Collections;
using UnityEngine;
using System;

public class UnitMover : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;

    private Transform _target;
    private Coroutine _coroutine;
    private float _epsilent = 0.1f;

    public event Action TargetReached = delegate { };

    private void OnDisable()
    {
        TargetReached = delegate { };
    }

    public void GoTo(Transform target)
    {
        _target = target;
        
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        
        _coroutine = StartCoroutine(MovingTo(_target.position));
    }

    private IEnumerator MovingTo(Vector3 position)
    {
        while ((transform.position - position).magnitude >= _epsilent)
        {
            FollowTarget(position);
            yield return Time.deltaTime;
        }
        
        TargetReached.Invoke();
    }
    
    private void FollowTarget(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            position, 
            _speed * Time.deltaTime);
    }
}
