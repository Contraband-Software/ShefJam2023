using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : InteractableBase
{

    private int interactionCycle = 0;
    public float upperAngle = 45f;
    public float lowerAngle = -45f;
    public float rotationSpeed = 10f;

    private void Start()
    {
        interactionCycle = 0;
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
        //

        while (true)
        {
            bool rotatingToUpperAngle = true;

            print(transform.rotation.z * Mathf.Rad2Deg);
            if (rotatingToUpperAngle)
            {
                print("rotating UP");
                Quaternion rot = transform.rotation;
                rot.z += rotationSpeed * Mathf.Deg2Rad * Time.deltaTime;
                print(rot.z * Mathf.Rad2Deg);

                if (rot.z * Mathf.Rad2Deg >= upperAngle)
                {
                    rotatingToUpperAngle = false;
                }
                transform.rotation = rot;

            }

            if (!rotatingToUpperAngle)
            {
                print("rotating DOWN");
                Quaternion rot = transform.rotation;
                rot.z += rotationSpeed * Mathf.Deg2Rad * Time.deltaTime * -1f;

                if (rot.z * Mathf.Rad2Deg <= lowerAngle)
                {
                    rotatingToUpperAngle = true;
                }
                transform.rotation = rot;
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
