using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseController : MonoBehaviour
{
    private SelectedTile selectedTile;

    private void Awake()
    {
        selectedTile = null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var tileHit = GetClickedTile();
            if (!tileHit.HasValue)
            {
                return;
            }
            if (
                tileHit.Value.collider.gameObject.TryGetComponent<SelectedTile>(
                    out SelectedTile tile
                )
            )
            {
                if (selectedTile != null)
                {
                    selectedTile.Hide();
                }
                selectedTile = tile;
                selectedTile.Show();
            }
        }
    }

    public RaycastHit2D? GetClickedTile()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }
}
