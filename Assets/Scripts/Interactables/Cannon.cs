using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : InteractableBase
{

    private int interactionCycle = 0;
    public float upperAngle = 45f;
    public float lowerAngle = -45f;
    public float rotationSpeed = 10f;

    bool rotatingToUpperAngle = true;

    private void Start()
    {
        interactionCycle = 0;
        rotatingToUpperAngle = true;
    }
    /// <summary>
    /// First interact is angle, second interact is power
    /// </summary>
    public override void Interact()
    {
        if(interactionCycle == 0)
        {
            AngleSetStartAnimating();
        }
        if(interactionCycle == 1)
        {
            AngleSet();
        }

        interactionCycle++;
    }

    /// <summary>
    /// Bounces between -45 and 45 degrees
    /// </summary>
    private void AngleSetStartAnimating()
    {
        StartCoroutine(AngleSetStartAnimating_C());
    }

    private IEnumerator AngleSetStartAnimating_C()
    {
        while (true)
        {
            print(transform.rotation.z * Mathf.Rad2Deg);
            if (rotatingToUpperAngle)
            {
                print("rotating UP");
                Vector3 rot = transform.rotation.eulerAngles;
                rot.z += rotationSpeed * Time.deltaTime;
                print(rot.z);

                if (rot.z >= upperAngle)
                {
                    rotatingToUpperAngle = false;
                }
                transform.rotation.SetEulerAngles(rot);

            }

            if (!rotatingToUpperAngle)
            {
                print("rotating DOWN");
                Vector3 rot = transform.rotation.eulerAngles;
                rot.z += rotationSpeed * Time.deltaTime - 1f; 

                if (rot.z <= lowerAngle)
                {
                    rotatingToUpperAngle = true;
                }
                transform.rotation.SetEulerAngles(rot);
            }

            yield return null;
        }
    }

    //Sets the cannon in place
    private void AngleSet()
    {
        StopCoroutine(AngleSetStartAnimating_C());
    }
}
