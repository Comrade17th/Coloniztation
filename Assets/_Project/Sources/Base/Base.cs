using System.Collections;
using System.Collections.Generic;
using _Project.Sources;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Linq;
using UnityEngine.Serialization;

public class Base : MonoBehaviour
{
    [SerializeField] private ResourcesDataBase _database;
    [SerializeField] private UnitGarage _unitGarage; 
    [SerializeField] private Radar _radar;
    [SerializeField] private float _orderDelay = 0.5f;
    
    private int _storedResources = 0;
    private int _unitCreatePrice = 3;

    private Coroutine _coroutine;
    private WaitForSeconds _waitOrder;

    [field: SerializeField] public Flag Flag; 

    public event Action<int> StoredResourcesChanged;

    private void Awake()
    {
        _waitOrder = new WaitForSeconds(_orderDelay);
        Assert.IsNotNull(_radar);
    }

    private void Start()
    {
        _coroutine = StartCoroutine(OrderingResources());
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

        TryCreateNewUnit();
    }

    private bool TryCreateNewUnit()
    {
        if (_storedResources >= _unitCreatePrice)
        {
            _storedResources -= _unitCreatePrice;
            StoredResourcesChanged.Invoke(_storedResources);
            _unitGarage.CreateUnit();
            return true;
        }

        return false;
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
