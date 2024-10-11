using System.Collections.Generic;
using UnityEngine;

public class UnitGarage : MonoBehaviour
{
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private int _startUnitsCount = 3;
    [SerializeField] private bool _isInitialBase;
    
    private List<Unit> _units = new();

    private void Start()
    {
        if (_isInitialBase)
        {
            CreateUnits(_startUnitsCount);
        }
        else
        {
            CreateUnit();
        }
    }

    // public void AcceptUnit(Unit unit)
    // {
    //     unit.Init(transform);
    //     _units.Add(unit);
    // }

    public void Init(UnitSpawner unitSpawner)
    {
        _unitSpawner = unitSpawner;
    }
    
    public void CreateUnit()
    {
        Unit unit = _unitSpawner.Spawn();
        unit.transform.position = transform.position;
        unit.Init(transform);
        _units.Add(unit);
    }
    
    public bool TryGetRestUnit(out Unit result)
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
    
    private void CreateUnits(int count)
    {
        for (int i = 0; i < count; i++)
        {
           CreateUnit();
        }
    }

    public void RemoveUnit(Unit unit)
    {
        _units.Remove(unit);
    }
}
