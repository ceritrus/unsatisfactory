using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public Vector2 sensitivity = new Vector2(2, 2);
    public float maxAngle = 80;

    public Transform orientation;

    public Vector2 rotation = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        rotation.y += ((Input.GetAxisRaw("Mouse X") + Input.GetAxis("Mouse X")) * 0.5f) * sensitivity.y * Time.fixedDeltaTime;
        rotation.x -= ((Input.GetAxisRaw("Mouse Y") + Input.GetAxis("Mouse Y")) * 0.5f) * sensitivity.x * Time.fixedDeltaTime;
        rotation.x = Mathf.Clamp(rotation.x, -maxAngle, maxAngle);

        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        orientation.localRotation = Quaternion.Euler(0, rotation.y, 0);
    }
}
