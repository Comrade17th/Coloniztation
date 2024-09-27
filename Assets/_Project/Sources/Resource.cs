using UnityEngine;
using System;

public class Resource : MonoBehaviour, ISpawnable<Resource>
{
    public event Action<Resource> Destroying;

    public int Value => _value;
    
    [SerializeField] private int _value = 1;

    public void Destroy()
    {
        Destroying?.Invoke(this);
    }
}
