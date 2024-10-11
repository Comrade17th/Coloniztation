using UnityEngine;
using System;

public class MouseInputService : MonoBehaviour
{
    private const int LeftMouseNumber = 0;
    private const int RightMouseNumber = 1;
    
    public event Action RMBClick = delegate { };
    public event Action LMBClick = delegate { };

    private void Update()
    {
        if (Input.GetMouseButtonDown(LeftMouseNumber))
        {
            LMBClick.Invoke();
        }
        
        if (Input.GetMouseButtonDown(RightMouseNumber))
        {
            RMBClick.Invoke();
        }
    }
}
