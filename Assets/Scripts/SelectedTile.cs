using UnityEngine;

public class SelectedTile : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    public int G;
    public int H;
    public int F
    {
        get { return G + H; }
    }
    public int cost = 1;
    public Vector3Int tilePos;
    public SelectedTile previousTile;

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

    public void ResetPathfinding()
    {
        Hide();
        G = 0;
        H = 0;
        previousTile = null;
    }
}
