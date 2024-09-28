using System;
using UnityEngine;

public class BotMover : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;

    private Transform _targetTransform;

    public event Action ArrivedDestination = delegate { };

    private void Start()
    {
        enabled = false;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards
            (transform.position, _targetTransform.position, _speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _targetTransform)
        {
            ArrivedDestination.Invoke();
        }
    }

    private void OnDisable()
    {
        ArrivedDestination = delegate { };
    }

    public void ReachDestination(Transform target)
    {
        _targetTransform = target;
        transform.LookAt(target);
        enabled = true;
    }
}
