using System;
using System.Collections;
using _Project.Sources;
using UnityEngine;

[RequireComponent(typeof(UnitMover))]

public class Unit : MonoBehaviour, ISpawnable<Unit>
{
    [SerializeField] private Transform _holdPoint;
    
    private Transform _base;
    private UnitMover _mover;
    private Resource _resource;
    private Coroutine _coroutine;
    
    public event Action<Unit> Destroying = delegate{};
    public event Action<Unit> BaseBuilded = delegate {};

    public event Action<Resource, Unit> ResourceDelivered = delegate { }; 
    
    public WorkStatuses WorkStatus { get; private set; }

    private void Awake()
    {
        _mover = GetComponent<UnitMover>();
        WorkStatus = WorkStatuses.Rest;
    }
    
    public void Init(Transform baseTransform)
    {
        _base = baseTransform;
    }

    public void BuildBase(Flag flag)
    {
        _mover.GoTo(flag.transform);
    }
    
    public void OrderResource(Resource resource)
    {
        _resource = resource;
        _mover.enabled = true;
        _mover.GoTo(resource.transform);
        _mover.TargetReached += GoBase;
        WorkStatus = WorkStatuses.GoResource;
    }

    private void GoBase()
    {
        _mover.TargetReached -= GoBase;
        _mover.TargetReached += DeliverResource;
        Grab();
        _mover.GoTo(_base);
    }

    private void DeliverResource()
    {
        _mover.TargetReached -= DeliverResource;
        _resource.transform.parent = null;
        ResourceDelivered.Invoke(_resource, this);
        WorkStatus = WorkStatuses.Rest;
    }
    
    private void Grab()
    {
        _resource.transform.parent = transform;
        _resource.transform.position = _holdPoint.position;
        WorkStatus = WorkStatuses.GoBase;
    }
}
