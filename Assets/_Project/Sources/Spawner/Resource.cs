using UnityEngine;
using System;

public class Resource : MonoBehaviour, ISpawnable<Resource>
{
    public event Action<Resource> Destroying;

    [field: SerializeField] public int Value { get; private set; }

    public void Destroy()
    {
        Destroying?.Invoke(this);
    }
}
