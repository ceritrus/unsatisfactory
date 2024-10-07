using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCam : MonoBehaviour
{
    public Vector2 sensitivity = new Vector2(2, 2);
    public float maxAngle = 80;

    public Transform orientation;

    public Vector2 rotation = Vector2.zero;

    [Header("Interactions")]
    [SerializeField] private LayerMask interactiveLayer = 1 << 8;
    [SerializeField] private Image crosshair;
    [SerializeField] private float range = 3;

    [Header("Inventory")]
    [SerializeField] private Transform inventory;
    [SerializeField] private GameObject hand = null;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private Item lastItem;

    // Update is called once per frame
    void Update()
    {
        rotation.y += ((Input.GetAxisRaw("Mouse X") + Input.GetAxis("Mouse X")) * 0.5f) * sensitivity.y * Time.fixedDeltaTime;
        rotation.x -= ((Input.GetAxisRaw("Mouse Y") + Input.GetAxis("Mouse Y")) * 0.5f) * sensitivity.x * Time.fixedDeltaTime;
        rotation.x = Mathf.Clamp(rotation.x, -maxAngle, maxAngle);

        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        orientation.localRotation = Quaternion.Euler(0, rotation.y, 0);

        RaycastHit hit;
        if (lastItem != null && (lastItem.type == Item.Type.Pickupable || lastItem.type == Item.Type.Usable))
            lastItem.Highlight(false);
        if (Physics.Raycast(transform.position, transform.forward, out hit, range, interactiveLayer))
        {
            lastItem = hit.collider.GetComponent<Item>();
            if (lastItem != null && (lastItem.type == Item.Type.Pickupable || lastItem.type == Item.Type.Usable))
                lastItem.Highlight(true);
            crosshair.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                switch (lastItem.type)
                {
                    case Item.Type.Pickupable:
                        if (hand)
                        {
                            hand.GetComponent<Item>().Shake();
                        }
                        else
                        {
                            lastItem.state = Item.State.Carried;
                            lastItem.Highlight(false);
                            hand = lastItem.gameObject;
                            float scale = hand.transform.localScale.x;
                            hand.transform.SetParent(inventory.transform);
                            hand.transform.localPosition = Vector3.zero;
                            hand.transform.localRotation = Quaternion.identity;
                            hand.transform.localScale = Vector3.one * scale;
                        }
                        break;
                    case Item.Type.DropZone:
                        if (lastItem.name == "Desk")
                            lastItem.GetComponent<Desk>().AddChild(hand.GetComponent<Item>(), hit);
                        else if (lastItem.name == "Conveyor")
                            lastItem.GetComponent<Conveyor>().AddChild(hand.GetComponent<Item>(), hit);
                        hand = null;
                        break;
                    case Item.Type.Usable:
                        lastItem.state = Item.State.Used;
                        break;
                }
                if (hit.collider.GetComponent<Item>() != null)
                {
                    Debug.Log("Interacted with " + hit.collider.name + ":" + hit.collider.GetComponent<Item>().state);
                }
            }
        }
        else
        {
            crosshair.gameObject.SetActive(false);
        }
    }
}
