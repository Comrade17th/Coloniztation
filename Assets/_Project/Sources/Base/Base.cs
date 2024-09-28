using System.Collections;
using System.Collections.Generic;
using _Project.Sources;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Linq;

public class Base : MonoBehaviour, IStorage
{
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private Radar _sonar;
    [SerializeField] private List<Unit> _units = new();

    [SerializeField] private int _startUnitsCount = 3;
    [SerializeField] private float _orderDelay = 0.5f;
    
    private HashSet<Resource> _resources = new();
    private int _storedResources = 0;

    private Coroutine _coroutine;
    private WaitForSeconds _waitOrder;

    public event Action<int> StoredResourcesChanged;

    private void Awake()
    {
        _waitOrder = new WaitForSeconds(_orderDelay);
        Assert.IsNotNull(_sonar);
    }

    private void OnEnable()
    {
        _sonar.ResourceFinded += WriteResource;
    }

    private void OnDisable()
    {
        _sonar.ResourceFinded -= WriteResource;
    }

    private void Start()
    {
        CreateUnits(_startUnitsCount);
        
        foreach (Unit unit in _units)
            InitUnit(unit);

        _coroutine = StartCoroutine(OrderingResources());
    }
    
    public void Keep(Resource resource)
    {
        Add(resource.Value);
        resource.transform.parent = transform;
        resource.Destroy();
    }

    private void CreateUnits(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (_unitSpawner.TrySpawn(out Unit unit))
            {
                _units.Add(unit);
            }
        }
    }
    
    private bool TryGetRestUnit(out Unit result)
    {
        foreach (Unit unit in _units)
        {
            if (unit.WorkStatus == WorkStatuses.Rest)
            {
                result = unit;
                return true;
            }
        }

        result = null;
        return false;
    }
    
    private void Add(int value)
    {
        if(value <= 0)
            return;

        _storedResources += value;
        StoredResourcesChanged?.Invoke(_storedResources);
    }

    private void InitUnit(Unit unit)
    {
        unit.Init(this, transform);
    }

    private void OrderResource()
    {
        if (_resources.Count > 0)
        {
            if (TryGetRestUnit(out Unit unit))
            {
                Resource resource = _resources.ElementAt(0);
                unit.OrderResource(resource);
                _resources.Remove(resource);
            }
        }
    }

    private void WriteResource(Resource resource)
    {
        Debug.Log($"Base added resource");
        _resources.Add(resource);
    }

    private IEnumerator OrderingResources()
    {
        while (true)
        {
            OrderResource();
            yield return _waitOrder;
        }
    }
    
}
