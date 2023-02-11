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
    float currentRotation = 0f;
    IEnumerator aimAnimation;

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
        interactionCycle++;
        if (interactionCycle == 1)
        {
            aimAnimation = AngleSetStartAnimating();
            StartCoroutine(aimAnimation);
        }
        if(interactionCycle == 2)
        {
            StopCoroutine(aimAnimation);
            AngleSet();
            interactionCycle = 0;
        }
    }

    /// <summary>
    /// Bounces between -45 and 45 degrees
    /// </summary>
    private IEnumerator AngleSetStartAnimating()
    {
        while (true)
        {
            if (rotatingToUpperAngle)
            {
                transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
                currentRotation += rotationSpeed * Time.deltaTime;

                if (currentRotation > upperAngle)
                {
                    rotatingToUpperAngle = false;
                    currentRotation = upperAngle;
                }
            }

            if (!rotatingToUpperAngle)
            {
                transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime, Space.Self);
                currentRotation -= rotationSpeed * Time.deltaTime;

                if (currentRotation < lowerAngle)
                {
                    rotatingToUpperAngle = true;
                    currentRotation = lowerAngle;
                }
            }
            yield return null;
        }
    }

    //Sets the cannon in place
    private void AngleSet()
    {
        
    }
}
