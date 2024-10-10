using UnityEngine;
using System.Collections.Generic;

public class Radar : MonoBehaviour
{
    [SerializeField] private float _radius = 10f;
    [SerializeField] private LayerMask _resourceMask;
    
    public IEnumerable<Resource> Scan()
    {
        List<Resource> resources = new List<Resource>();

        foreach (var collider in Physics.OverlapSphere(transform.position, _radius, _resourceMask))
        {
            if (collider.TryGetComponent(out Resource cristal))
                resources.Add(cristal);
        }

        return resources;
    }
}
