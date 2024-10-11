using UnityEngine;
using UnityEngine.Assertions;

public class BaseSpawner : Spawner<Base>
{
    [SerializeField] private ResourcesDataBase _database;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private Base _initialBase;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(_unitSpawner);
        Assert.IsNotNull(_database);
        Assert.IsNotNull(_initialBase);
    }

    private void OnEnable()
    {
        _initialBase.Flag.BaseBuilded += OnBaseBuilded;
    }

    private void OnDisable()
    {
        _initialBase.Flag.BaseBuilded -= OnBaseBuilded;
    }

    private void OnBaseBuilded(Flag flag)
    {
        Base spawnedBase = Spawn();
        spawnedBase.Flag.BaseBuilded += OnBaseBuilded;
        spawnedBase.Init(_database, _unitSpawner);
        spawnedBase.transform.position = flag.transform.position;
        flag.gameObject.SetActive(false);
    }
}
