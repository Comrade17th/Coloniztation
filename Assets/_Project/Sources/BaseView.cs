using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseView : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private TextMeshProUGUI _textTMP;

    private void OnEnable()
    {
        _base.StoredResourcesChanged += WriteCount;
    }

    private void OnDisable()
    {
        _base.StoredResourcesChanged -= WriteCount;
    }

    private void WriteCount(int count)
    {
        _textTMP.text = $"{count}";
    }
}
