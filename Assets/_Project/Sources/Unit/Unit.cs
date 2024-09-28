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
        _mover.GoTo(resource.transform);
        _mover.TargetReached += GoBase;
        WorkStatus = WorkStatuses.GoResource;
    }

    private void GoBase()
    {
        _mover.TargetReached -= GoBase;
        _mover.TargetReached += PutToStorage;
        Grab();
        _mover.GoTo(_base);
    }

    // private IEnumerator CollectingResource()
    // {
    //     yield return MovingTo(_resourceTransform.position);
    //     
    //     Grab(_resource);
    //     LaunchCoroutine(GoingBase());
    // }
    //
    // private IEnumerator GoingBase()
    // {
    //     yield return MovingTo(_basePosition);
    //     PutToStorage();
    // }

    private void PutToStorage()
    {
        _mover.TargetReached -= PutToStorage;
        _resource.transform.parent = null;
        _storage.Keep(_resource);
        WorkStatus = WorkStatuses.Rest;
    }
    
    private void Grab()
    {
        _resource.transform.parent = transform;
        _resource.transform.position = _holdPoint.position;
        WorkStatus = WorkStatuses.GoBase;
    }
}
