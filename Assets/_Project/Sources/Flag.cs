using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Flag : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Collider _collider;
    
    private float _spawnRadius = 3f;

    public event Action<Flag> BaseBuilded = delegate { };

    public bool CanPlant()
    {
        return Physics.OverlapSphere(transform.position, _spawnRadius, _layerMask).Length == 0;
    }

    public void StartPlacing()
    {
        _collider.enabled = false;
        gameObject.SetActive(true);
    }

    public void UnSelect()
    {
        _collider.enabled = true;
        gameObject.SetActive(false);
    }

    public void Plant()
    {
        _collider.enabled = true;
    }

    public void StartBuildingBase()
    {
        BaseBuilded.Invoke(this);
    }
}
