using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPosition.position;
    }

    void OnValidate()
    {
        if (cameraPosition == null)
        {
            Debug.LogError("Camera Position is not set.");
        }
        else
        {
            transform.position = cameraPosition.position;
        }
    }
}
