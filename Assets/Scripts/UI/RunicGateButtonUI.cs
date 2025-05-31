using System;
using UnityEngine;
using UnityEngine.UI;

public class RunicGateButtonUI : MonoBehaviour
{
    public event EventHandler OnRunicGateButtonPressed;

    [SerializeField]
    InterfaceDataSO interfaceDataSO;

    [SerializeField]
    Image image;

    [SerializeField]
    Sprite twoGatesUsed;

    [SerializeField]
    Sprite oneGatesUsed;

    [SerializeField]
    Sprite noGatesUsed;

    Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => OnRunicGateButtonPressed?.Invoke(this, EventArgs.Empty));
    }

    private void OnEnable()
    {
        interfaceDataSO.onRunicGateCountChanged.AddListener(SetSprite);
        interfaceDataSO.onTargetChanged.AddListener(SetInteractable);
    }

    private void OnDisable()
    {
        interfaceDataSO.onRunicGateCountChanged.RemoveListener(SetSprite);
        interfaceDataSO.onTargetChanged.RemoveListener(SetInteractable);
    }

    private void SetSprite(int numberOfGates)
    {
        if (numberOfGates == 0)
        {
            image.sprite = noGatesUsed;
            return;
        }
        if (numberOfGates == 1)
        {
            image.sprite = oneGatesUsed;
            return;
        }
        if (numberOfGates == 2)
        {
            image.sprite = twoGatesUsed;
            return;
        }
    }

    private void SetInteractable(GameObject obj)
    {
        if (obj == null)
        {
            button.interactable = true;
            return;
        }
        if (obj.TryGetComponent<RunicGate>(out _))
        {
            button.interactable = true;
            return;
        }
        button.interactable = false;
    }
}
