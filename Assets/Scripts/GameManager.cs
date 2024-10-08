using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Conveyor conveyor;
    public static Desk desk;

    private Animation anim;

    [SerializeField] private bool toggleConveyor = false;
    private void Start()
    {
        if (gameObject.GetComponent<Animation>() == null)
        {
            anim = gameObject.AddComponent<Animation>();
        }
        else
        {
            anim = gameObject.GetComponent<Animation>();
        }
    }

    private void Update()
    {
        if (toggleConveyor)
        {
            ToggleConveyor();
            toggleConveyor = false;
        }
    }

    public static void ToggleConveyor()
    {
        conveyor.targetSpeed = conveyor.targetSpeed == 0 ? conveyor.speed : 0;
        conveyor.matTargetSpeed = conveyor.matTargetSpeed == 0 ? conveyor.matSpeed : 0;
    }
}
