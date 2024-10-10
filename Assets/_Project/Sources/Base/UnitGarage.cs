using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGarage : MonoBehaviour
{
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private int _startUnitsCount = 3;
    
    private List<Unit> _units = new();

    private void Start()
    {
        CreateUnits(_startUnitsCount);
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
}
