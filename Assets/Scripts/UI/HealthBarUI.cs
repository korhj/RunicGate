using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField]
    InterfaceDataSO interfaceDataSO;

    [SerializeField]
    Image image;

    void Start()
    {
        SetFillAmount(1f);
    }

    private void OnEnable()
    {
        interfaceDataSO.onHealthChanged.AddListener(SetFillAmount);
    }

    private void OnDisable()
    {
        interfaceDataSO.onHealthChanged.RemoveListener(SetFillAmount);
    }

    private void SetFillAmount(float percent)
    {
        image.fillAmount = percent;
    }
}
