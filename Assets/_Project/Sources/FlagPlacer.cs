using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagPlacer : MonoBehaviour
{
    [SerializeField] private RayShooter _rayShooter;
    private Base _base;

    private void OnEnable()
    {
        _rayShooter.Clicked += PlaceFlag;
    }

    private void OnDisable()
    {
        _rayShooter.Clicked -= PlaceFlag;
    }

    private void PlaceFlag()
    {
        
    }
}
