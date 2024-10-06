using System;
using System.Collections;
using _Project.Sources;
using UnityEngine;

[RequireComponent(typeof(UnitMover))]

public class Unit : MonoBehaviour, ISpawnable<Unit>
{
    [SerializeField] private Transform _holdPoint;
    
    private IStorage _storage;
    private Transform _base;

    private UnitMover _mover;
    private Resource _resource;
    private Coroutine _coroutine;
    
    public event Action<Unit> Destroying = delegate{};
    
    public WorkStatuses WorkStatus { get; private set; }

    private void Awake()
    {
        _mover = GetComponent<UnitMover>();
        WorkStatus = WorkStatuses.Rest;
    }
    
    public void Init(IStorage storage, Transform baseTransform)
    {
        _storage = storage;
        _base = baseTransform;
    }
    
    public void OrderResource(Resource resource)
    {
        _resource = resource;
        Debug.Log($"Resource ordered, i go resource");
        _mover.GoTo(resource.transform); // some where here something goes wrond
        _mover.TargetReached += GoBase;
        WorkStatus = WorkStatuses.GoResource;
    }

    private void GoBase()
    {
        Debug.Log($"Resource reached");
        _mover.TargetReached -= GoBase;
        _mover.TargetReached += PutToStorage;
        Grab();
        Debug.Log($"i grabbed resource");
        _mover.GoTo(_base);
        Debug.Log($"I go base");
    }

    private void PutToStorage()
    {
        Debug.Log($"Base reached");
        _mover.TargetReached -= PutToStorage;
        _resource.transform.parent = null;
        _storage.Keep(_resource);
        WorkStatus = WorkStatuses.Rest;
        Debug.Log($"I rest");
    }
    
    private void Grab()
    {
        _resource.transform.parent = transform;
        _resource.transform.position = _holdPoint.position;
        WorkStatus = WorkStatuses.GoBase;
    }
}
