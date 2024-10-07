using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZone : Item
{

    [Header("Drop Zone")]
    [SerializeField] private Vector2 size = new Vector2(1, 1);
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);
    [SerializeField] private bool drawGizmos = true;

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + offset, new Vector3(size.x, 0.1f, size.y));
        }
    }

    public void AddChild(Item item, RaycastHit hit)
    {
        float scale = item.transform.localScale.x;
        item.transform.SetParent(transform);
        item.transform.localScale = Vector3.one * scale;
        item.transform.position = new Vector3(
            Mathf.Clamp(hit.point.x, transform.position.x + offset.x - size.x / 2, transform.position.z + offset.x + size.x / 2),
            offset.y + item.transform.localScale.y + .05f,
            Mathf.Clamp(hit.point.z, transform.position.z + offset.z - size.y / 2, transform.position.z + offset.z + size.y / 2)
        );
        item.transform.rotation = Quaternion.identity;

        switch (name)
        {
            case "Desk":
                item.state = State.OnDesk;
                break;
            case "Conveyor":
                item.state = State.OnConveyor;
                break;
        }
    }
}
