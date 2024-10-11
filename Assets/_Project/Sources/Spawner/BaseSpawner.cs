using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseSpawner : Spawner<Base>
{
    [SerializeField] private ResourcesDataBase _database;
    [SerializeField] private Base _initialBase;

    private void Awake()
    {
        Assert.IsNotNull(_database);
        Assert.IsNotNull(_initialBase);
    }

    private void Start()
    {
        _initialBase.Flag.BaseBuilded += OnBaseBuilded;
    }

    private void OnEnable()
    {
        _initialBase.Flag.BaseBuilded += OnBaseBuilded;
    }

    private void OnDisable()
    {
        _initialBase.Flag.BaseBuilded -= OnBaseBuilded;
        // отписка?
    }

    private void OnBaseBuilded(Flag flag)
    {
        Base spawnedBase = Spawn();
        Debug.Log($"{spawnedBase == null}");
        // spawnedBase.Init(_database);
        // spawnedBase.transform.position = flag.transform.position;
        // flag.gameObject.SetActive(false);
    }
}
