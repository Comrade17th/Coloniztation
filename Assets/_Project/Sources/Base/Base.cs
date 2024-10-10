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
        Debug.Log($"Order res");
        
        if (_unitGarage.TryGetRestUnit(out Unit unit))
        {
            Debug.Log($"Free units");
            IEnumerable<Resource> scannedResources = _radar.Scan();
            IEnumerable<Resource> filteredResources = _database.GetFreeResources(scannedResources);
            
            if(filteredResources.Any() == false)
                return;

            IEnumerable<Resource> sortedResources = filteredResources.OrderBy(resource =>
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
