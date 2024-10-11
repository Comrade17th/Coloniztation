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
        if(Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (_selectedBase == null)
            {
                if (hit.collider.TryGetComponent(out Base hitBase))
                {
                    _selectedBase = hitBase;
                    _flag = _selectedBase.Flag;
                    _flag.StartPlacing();
                    _selectedBase.StopUnitsGettingReady();
                }
            }
            else
            {
                if (_flag.CanPlant())
                {
                    _flag.Plant();
                    _selectedBase.StartUnitsGettingReady();
                    OnUnchooseBase();
                }
            }
        }
    }
}
