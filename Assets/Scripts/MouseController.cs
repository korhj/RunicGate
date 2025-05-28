using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseController : MonoBehaviour
{
    public Vector3Int pos = new Vector3Int(0, 0, 0);
    Ray ray;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var tileHit = GetClickedTile();
            if (tileHit.HasValue)
            {
                Debug.Log(tileHit.Value.collider.gameObject);
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
