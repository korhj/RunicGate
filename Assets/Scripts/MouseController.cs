using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MouseController : MonoBehaviour
{
    [SerializeField]
    private LayerMask clickableTileMask;
    public event EventHandler<OnTileSelectedEventArgs> OnTileSelected;

    public class OnTileSelectedEventArgs : EventArgs
    {
        public SelectedTile targetSelectedTile;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D clicked = GetClickedCollider();
            if (clicked == null)
                return;

            if (clicked.TryGetComponent<SelectedTile>(out SelectedTile tile))
            {
                OnTileSelected?.Invoke(
                    this,
                    new OnTileSelectedEventArgs { targetSelectedTile = tile }
                );
            }
        }
    }

    public Collider2D GetClickedCollider()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return null;
        }

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector2 worldPoint2D = new(worldMousePos.x, worldMousePos.y);

        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPoint2D, clickableTileMask);

        if (colliders.Length > 0)
        {
            return colliders.OrderByDescending(c => c.transform.position.z).First();
        }

        return null;
    }
}
