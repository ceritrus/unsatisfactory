using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveillanceCam : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Transform target;

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = target.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }


}
