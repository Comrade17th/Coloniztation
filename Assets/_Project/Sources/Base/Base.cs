using System.Collections;
using System.Collections.Generic;
using _Project.Sources;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Linq;
using UnityEngine.Serialization;

public class Base : MonoBehaviour, ISpawnable<Base>
{
    [SerializeField] private ResourcesDataBase _database;
    [SerializeField] private UnitGarage _unitGarage; 
    [SerializeField] private Radar _radar;
    [SerializeField] private float _orderDelay = 0.5f;
    
    private int _storedResources = 0;
    private int _unitCreatePrice = 3;
    private int _baseCreatePrice = 5;
    private bool _isBaseBuilding = false;

    private Coroutine _coroutine;
    private WaitForSeconds _waitOrder;

    [field: SerializeField] public Flag Flag; 

    public event Action<int> StoredResourcesChanged;
    public event Action<Base> Destroying;

    private void Awake()
    {
        _waitOrder = new WaitForSeconds(_orderDelay);
        Assert.IsNotNull(_radar);
    }

    private void Start()
    {
        _coroutine = StartCoroutine(Working());
    }

    public void StartUnitsGettingReady()
    {
        _isBaseBuilding = true;
    }
    
    public void StopUnitsGettingReady()
    {
        _isBaseBuilding = false;
    }

    public void AcceptUnit(Unit unit) => _unitGarage.AcceptUnit(unit);
    
    private void OrderResource()
    {
        if (_unitGarage.TryGetRestUnit(out Unit unit))
        {
            IEnumerable<Resource> scannedResources = _database.GetFreeResources(_radar.Scan());
            
            if(scannedResources.Any() == false)
                return;

            IEnumerable<Resource> sortedResources = scannedResources.OrderBy(resource =>
                (resource.transform.position - transform.position).sqrMagnitude);

            Resource resource = sortedResources.First();
            _database.ReserveResource(resource);
            unit.OrderResource(resource);
            unit.ResourceDelivered += OnResourceDelivered;
        }
    }

    private void OnResourceDelivered(Resource resource, Unit unit)
    {
        unit.ResourceDelivered -= OnResourceDelivered;
        _database.RemoveReservation(resource);
        
        _storedResources += (resource.Value);
        StoredResourcesChanged?.Invoke(_storedResources);
        resource.Destroy();

        TryCreateNewUnit();
    }

    private bool TryCreateNewUnit()
    {
        if (_isBaseBuilding == false && _storedResources >= _unitCreatePrice)
        {
            _storedResources -= _unitCreatePrice;
            StoredResourcesChanged.Invoke(_storedResources);
            _unitGarage.CreateUnit();
            return true;
        }

        return false;
    }

    private void CreateBase()
    {
        if (_isBaseBuilding && _storedResources >= _baseCreatePrice)
        {
            if (_unitGarage.TryGetRestUnit(out Unit unit))
            {
                _storedResources -= _baseCreatePrice;
                unit.BuildBase(Flag);
            }
        }
    }

    private IEnumerator Working()
    {
        while (true)
        {
            CreateBase();
            OrderResource();
            yield return _waitOrder;
        }
    }
}
