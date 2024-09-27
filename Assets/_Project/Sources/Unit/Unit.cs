using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Sources;
using UnityEngine;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour, ISpawnable<Unit>
{
    public event Action<Unit> Destroying;

    [SerializeField] private float _speed;
    [SerializeField] private Transform _holdPoint;
    
    private IStorage _storage;
    private Vector3 _basePosition;
    
    private Resource _resource;
    private Transform _resourceTransform;
    private Coroutine _coroutine;
    
    public WorkStatuses WorkStatus { get; private set; }

    private void Awake()
    {
        WorkStatus = WorkStatuses.Rest;
    }
    
    public void Init(IStorage storage, Vector3 basePosition)
    {
        _storage = storage;
        _basePosition = basePosition;
    }
    
    public void OrderResource(Resource resource)
    {
        _resource = resource;
        _resourceTransform = resource.GetComponent<Transform>();
        WorkStatus = WorkStatuses.GoResource;
        
        LaunchCoroutine(CollectingResource());
    }

    private IEnumerator CollectingResource()
    {
        yield return MovingTo(_resourceTransform.position);
        
        Grab(_resource);
        LaunchCoroutine(GoingBase());
    }
    
    private IEnumerator GoingBase()
    {
        yield return MovingTo(_basePosition);
        PutToStorage();
    }

    private IEnumerator MovingTo(Vector3 position)
    {
        while (transform.position != position)
        {
            FollowTarget(position);
            yield return Time.deltaTime;
        }
    }

    private void LaunchCoroutine(IEnumerator routine)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        
        _coroutine = StartCoroutine(routine);
    }

    private void PutToStorage()
    {
        _storage.Keep(_resource);
        WorkStatus = WorkStatuses.Rest;
    }
    
    private void Grab(Resource resource)
    {
        resource.transform.parent = transform;
        resource.transform.position = _holdPoint.position;
        WorkStatus = WorkStatuses.GoBase;
    }

    private void FollowTarget(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            position, 
            _speed * Time.deltaTime);
    }
}
