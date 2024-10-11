using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Linq;

public class Base : MonoBehaviour, ISpawnable<Base>
{
    [SerializeField] private ResourcesDataBase _database;
    [SerializeField] private UnitGarage _unitGarage; 
    [SerializeField] private Radar _radar;
    [SerializeField] private float _orderDelay = 0.5f;
    
    private int _storedResources;
    private int _unitCreatePrice = 3;
    private int _baseCreatePrice = 5;
    private int _unitsNeedToNewBase = 1;
    private bool _isPreparingToBuild;

    private Coroutine _coroutine;
    private WaitForSeconds _waitOrder;
    
    public event Action<int> StoredResourcesChanged;
    public event Action<Base> Destroying;

    [field: SerializeField] public Flag Flag { get; private set; }

    private bool IsEnoughUnits => _unitGarage.Count > _unitsNeedToNewBase;
    
    private void Awake()
    {
        _waitOrder = new WaitForSeconds(_orderDelay);
        Assert.IsNotNull(_radar);
    }

    private void Start()
    {
        _coroutine = StartCoroutine(Working());
    }

    public void Init(ResourcesDataBase database, UnitSpawner unitSpawner)
    {
        _unitGarage.Init(unitSpawner);
        _database = database;
    }

    public void StartUnitsGettingReady()
    {
        _isPreparingToBuild = true;
    }
    
    public void StopUnitsGettingReady()
    {
        _isPreparingToBuild = false;
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

        if(_isPreparingToBuild == false || IsEnoughUnits == false)
            TryCreateNewUnit();
    }

    private bool TryCreateNewUnit()
    {
        if (_storedResources >= _unitCreatePrice) // _isBaseBuilding == false
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
        if ( _unitGarage.Count > _unitsNeedToNewBase && 
            _isPreparingToBuild && 
             _storedResources >= _baseCreatePrice)
        {
            if (_unitGarage.TryGetRestUnit(out Unit unit))
            {
                _storedResources -= _baseCreatePrice;
                StoredResourcesChanged.Invoke(_storedResources);
                unit.BuildBase(Flag);
                unit.FlagResourceDelivered += OnUnitFlagResourceDelivered;
            }
        }
    }

    private void OnUnitFlagResourceDelivered(Unit unit)
    {
        unit.FlagResourceDelivered -= OnUnitFlagResourceDelivered;
        unit.ResourceDelivered -= OnResourceDelivered;
        unit.Destroy();
        _unitGarage.RemoveUnit(unit);
        StopUnitsGettingReady();
    }
}
