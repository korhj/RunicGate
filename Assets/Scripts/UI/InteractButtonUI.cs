using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractButtonUI : MonoBehaviour
{
    public event EventHandler OnInteractButtonPressed;

    [SerializeField]
    InterfaceDataSO interfaceDataSO;

    [SerializeField]
    Sprite mimicUISprite;

    [SerializeField]
    Sprite timedButtonUISprite;

    [SerializeField]
    Sprite noTargetUISprite;

    [SerializeField]
    Image image;

    [SerializeField]
    Button button;

    void Awake()
    {
        button.onClick.AddListener(() => OnInteractButtonPressed?.Invoke(this, EventArgs.Empty));
        SetInteractable(interfaceDataSO.targetObject);
    }

    private void OnEnable()
    {
        interfaceDataSO.onTargetChanged.AddListener(SetInteractable);
    }

    private void OnDisable()
    {
        interfaceDataSO.onTargetChanged.RemoveListener(SetInteractable);
    }

    private void SetSprite(IPlayerInteractable target)
    {
        if (target == null && interfaceDataSO.playerHasMimic)
        {
            image.sprite = mimicUISprite;
            return;
        }
        if (target == null)
        {
            image.sprite = noTargetUISprite;
            return;
        }
        if (target is CursedMimic)
        {
            image.sprite = mimicUISprite;
            return;
        }
        if (target is TimedButton)
        {
            image.sprite = timedButtonUISprite;
        }
    }

    private void SetInteractable(GameObject obj)
    {
        if (obj == null && !interfaceDataSO.playerHasMimic)
        {
            SetSprite(null);
            button.interactable = false;
            return;
        }
        if (obj == null && interfaceDataSO.playerHasMimic)
        {
            SetSprite(null);
            button.interactable = true;
            return;
        }
        if (obj.TryGetComponent<IPlayerInteractable>(out IPlayerInteractable target))
        {
            SetSprite(target);
            button.interactable = true;
            return;
        }
        SetSprite(null);
        button.interactable = false;
    }
}
