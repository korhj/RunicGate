using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloatEvent : UnityEvent<float> { }

[System.Serializable]
public class IntEvent : UnityEvent<int> { }

[System.Serializable]
public class BoolEvent : UnityEvent<bool> { }

[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject> { }

[CreateAssetMenu(menuName = "SO/Interface Data")]
public class InterfaceDataSO : ScriptableObject
{
    [Header("Data")]
    public float playerHealthPercent;
    public int runicGateCount;
    public GameObject targetObject;
    public bool playerHasObject;

    [Header("Events")]
    public FloatEvent onHealthChanged;
    public IntEvent onRunicGateCountChanged;
    public GameObjectEvent onTargetChanged;
    public BoolEvent onPickUpOrDrop;

    public void SetPlayerHealthPercent(float value)
    {
        if (playerHealthPercent != value)
        {
            playerHealthPercent = value;
            onHealthChanged.Invoke(playerHealthPercent);
        }
    }

    public void SetRunicGateCount(int value)
    {
        if (runicGateCount != value)
        {
            runicGateCount = value;
            onRunicGateCountChanged.Invoke(runicGateCount);
        }
    }

    public void SetTargetObject(GameObject target)
    {
        if (targetObject != target)
        {
            targetObject = target;
            onTargetChanged.Invoke(targetObject);
        }
    }

    public void SetPlayerHasObject(bool hasObject)
    {
        if (playerHasObject != hasObject)
        {
            playerHasObject = hasObject;
            onPickUpOrDrop.Invoke(playerHasObject);
        }
    }
}
