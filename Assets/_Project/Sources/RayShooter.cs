using UnityEngine;
using System;

public class RayShooter : MonoBehaviour
{
    private KeyCode _leftMouseButton = KeyCode.Mouse0;
    private Camera _camera;
    
    public event Action Clicked = delegate {};

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_leftMouseButton))
        {
            OnLeftMouseButtonClick();
        }
    }

    private void OnLeftMouseButtonClick()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
				
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out IClickable clickable))
            {
                clickable.OnClick();
                Clicked.Invoke();
            }
        }
    }
}
