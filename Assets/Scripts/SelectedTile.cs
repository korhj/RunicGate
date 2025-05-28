using UnityEngine;

public class SelectedTile : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    public void Start()
    {
        Hide();
    }

    public void Show()
    {
        spriteRenderer.enabled = true;
    }

    public void Hide()
    {
        spriteRenderer.enabled = false;
    }
}
