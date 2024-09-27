using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class Spawner<T> : MonoBehaviour where T: MonoBehaviour, ISpawnable<T>
{
	[SerializeField] private T _prefab;
	[SerializeField] private float _delay = 3f;
	[SerializeField] private float _randomSpreadX = 1.1f;
	[SerializeField] private float _randomSpreadZ = 1.1f;
	[SerializeField] private int _startAmount = 1;
	[SerializeField] private int _maxAmount = 15;
	[SerializeField] private bool _isAutoSpawn = false;

	private Pool<T> _pool;
	private int _activeCount = 0;
	private int _spawnsCount = 0;
	private WaitForSeconds _waitSpawn;
	
	public event Action<int, int, int> CounterChanged;
	
	private void Awake()
	{
		_pool = new Pool<T>(_prefab, transform, transform, _startAmount);
		_waitSpawn = new WaitForSeconds(_delay);
	}

	private void Start()
	{
		CounterChanged?.Invoke(_pool.EntitiesCount, _activeCount, _spawnsCount);
		
		if (_isAutoSpawn)
			StartCoroutine(RandomSpawning());
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(transform.position, new Vector3(_randomSpreadX, 2, _randomSpreadZ));
	}

	public bool TrySpawn(out T spawnedObject)
	{
		if (_maxAmount != _activeCount)
		{
			spawnedObject = _pool.Get();

			spawnedObject.Destroying += OnSpawnedDestroy;
			spawnedObject.gameObject.SetActive(true);
			
			_activeCount++;
			_spawnsCount++;
			CounterChanged?.Invoke(_pool.EntitiesCount, _activeCount, _spawnsCount);
			
			return true;
		}
		
		spawnedObject = null;
		return false;
	}
	
	protected virtual void SpawnAtRandom()
	{
		if (TrySpawn(out T spawnedObject))
		{
			spawnedObject.transform.position = GetRandomSpawnPosition();	
		}
	}

	protected Vector3 GetRandomSpawnPosition()
	{
		float overlapRaius = 0.5f;
		float modifier = 2;
		float resultSpreadX = _randomSpreadX / modifier;
		float resultSpreadZ = _randomSpreadZ / modifier;
		Vector3 spawnPosition;
		
		do
		{
			float spawnPositionX = Random.Range(transform.position.x - resultSpreadX,
				transform.position.x + resultSpreadX);
			float spawnPositionZ = Random.Range(transform.position.z - resultSpreadZ,
				transform.position.z + resultSpreadZ);
			spawnPosition = new Vector3(spawnPositionX,
				transform.position.y,
				spawnPositionZ);
		} while (IsSameObjectAround(overlapRaius, spawnPosition));

		return spawnPosition;
	}

	protected virtual void OnSpawnedDestroy(T spawnableObject)
	{
		spawnableObject.Destroying -= OnSpawnedDestroy;
		spawnableObject.gameObject.SetActive(false);
		_pool.Release(spawnableObject);

		_activeCount--;
		CounterChanged?.Invoke(_pool.EntitiesCount, _activeCount, _spawnsCount);
	}

	private bool IsSameObjectAround(float radius, Vector3 center)
	{
		Collider[] colliders = Physics.OverlapSphere(center, radius);
		bool result = false;
		
		foreach (Collider collider in colliders)
		{
			if (collider.TryGetComponent(out T component))
			{
				result = true;
				break;
			}
		}

		return result;
	}
	
	private IEnumerator RandomSpawning()
	{
		while (enabled)
		{
			SpawnAtRandom();
			yield return _waitSpawn;
		}
	}
}

