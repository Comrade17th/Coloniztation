using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private int _targetFrameRate = 30;
    private void Awake()
    {
        Application.targetFrameRate = _targetFrameRate;
    }
}
