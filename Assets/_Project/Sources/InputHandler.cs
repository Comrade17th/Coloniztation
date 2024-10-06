using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private MouseInputService _mouseService;
    
    private Camera _camera;
    private Flag _flag;
    private Base _selectedBase;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _mouseService.LMBClick += OnLMBClicked;
        _mouseService.RMBClick += OnRMBClicked;
    }

    private void OnDisable()
    {
        _mouseService.LMBClick -= OnLMBClicked;
        _mouseService.RMBClick -= OnRMBClicked;
    }

    private void Update()
    {
        if (_selectedBase != null)
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                _flag.transform.position = hit.point;
            }
        }
    }

    private void OnUnchooseBase()
    {
        _flag = null;
        _selectedBase = null;
    }

    private void OnRMBClicked()
    {
        if (_flag != null)
        {
            _flag.gameObject.SetActive(false);
            OnUnchooseBase();    
        }
    }

    private void OnLMBClicked()
    {
        Debug.Log($"LMB");
        
        if(Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (_selectedBase == null)
            {
                Debug.Log($"*");
                if (hit.collider.TryGetComponent(out Base hitBase))
                {
                    Debug.Log($"Base got");
                    _selectedBase = hitBase;
                    _flag = _selectedBase.Flag;
                    _flag.StartPlacing();
                    // say to base stop getting ready units
                }
            }
            else
            {
                if (_flag.CanPlant())
                {
                    _flag.Plant();
                    OnUnchooseBase();
                    // say to base, to get ready units for new base building
                }
                    
            }
        }
    }
}
