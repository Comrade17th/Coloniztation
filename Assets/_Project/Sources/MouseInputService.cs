using UnityEngine;
using System;

public class MouseInputService : MonoBehaviour
{
    public event Action RMBClick = delegate { };
    public event Action LMBClick = delegate { };

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LMBClick.Invoke();
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            RMBClick.Invoke();
        }
    }
}
