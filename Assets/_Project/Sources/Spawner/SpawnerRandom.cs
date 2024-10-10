using System.Collections;
using UnityEngine;

namespace _Project.Sources.Spawner
{
    public class SpawnerRandom<T>: Spawner<T> where T: MonoBehaviour, ISpawnable<T>
    {
        [SerializeField] private float _delay = 3f;
        [SerializeField] private float _randomSpreadX = 1.1f;
        [SerializeField] private float _randomSpreadZ = 1.1f;
        [SerializeField] private bool _isAutoSpawn = false;
        [SerializeField] private int _maxAmount = 15;
        
        private WaitForSeconds _waitSpawn;
        private Coroutine _coroutine;

        protected override void Awake()
        {
            base.Awake();
            _waitSpawn = new WaitForSeconds(_delay);
        }

        protected override void Start()
        {
            if (_isAutoSpawn)
            {
                _coroutine = StartCoroutine(RandomSpawning());
            }
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
                spawnedObject = Spawn();
                return true;
            }
		
            spawnedObject = null;
            return false;
        }
        
        private void SpawnAtRandom()
        {
            if (TrySpawn(out T spawnedObject))
            {
                spawnedObject.transform.position = GetRandomSpawnPosition();	
            }
        }
        
        private IEnumerator RandomSpawning()
        {
            while (enabled)
            {
                SpawnAtRandom();
                yield return _waitSpawn;
            }
        }
        
        private Vector3 GetRandomSpawnPosition()
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
    }
}