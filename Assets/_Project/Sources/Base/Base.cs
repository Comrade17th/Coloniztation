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
    [SerializeField] private List<Unit> _units = new(); // unit garage

    [SerializeField] private int _startUnitsCount = 3;
    [SerializeField] private float _orderDelay = 0.5f;
    
    private HashSet<Resource> _resources = new(); // => database res
    private int _storedResources = 0;
    private int _unitCreatePrice = 3;

    private Coroutine _coroutine;
    private WaitForSeconds _waitOrder;

    [field: SerializeField] public Flag Flag; 

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
        if(resource.Value <= 0)
            return;

        _storedResources += (resource.Value);
        StoredResourcesChanged?.Invoke(_storedResources);
        resource.Destroy();
    }

    private void TryCreateUnit()
    {
        if (_storedResources >= _unitCreatePrice)
        {
            CreateUnits(1);
        }
    }

    private void CreateUnits(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _units.Add(_unitSpawner.Spawn());
        }
    }

    private void CreateUnit()
    {
        
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
