using UnityEngine;
using System;

public class Resource : MonoBehaviour, ISpawnable<Resource>
{
    public event Action<Resource> Destroying;
}
