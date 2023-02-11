using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture;

public class Cannon : InteractableBase
{
    
    [Header("Settings")]
    [SerializeField] float upperAngle = 45f;
    [SerializeField] float lowerAngle = -45f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float powerSpeed = 1f;
    [SerializeField] float cannonballPower = 50f;

    [Header("References")]
    [SerializeField] Transform powerBar;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawnPos;
    [SerializeField] Transform cannonPivot;

    private bool interacting = false;

    private int interactionCycle = 0;

    bool rotatingToUpperAngle = true;
    float currentRotation = 0f;

    bool poweringUpwards = true;
    float currentPower = 0f;

    IEnumerator aimAnimation;
    IEnumerator powerAnimation;

    private void Start()
    {
        interactionCycle = 0;
        rotatingToUpperAngle = true;
        currentPower = 0f;
        powerBar.localScale = new Vector2(0f, powerBar.localScale.y);

    }
    /// <summary>
    /// First interact is angle, second interact is power
    /// </summary>
    public override void Interact()
    {
        interacting = true;

        interactionCycle++;
        //CLICK TO AIM
        if (interactionCycle == 1)
        {
            aimAnimation = AngleSetStartAnimating();
            StartCoroutine(aimAnimation);
        }
        //CLICK TO POWER SET
        if(interactionCycle == 2)
        {
            StopCoroutine(aimAnimation);
            powerAnimation = PowerSetStartAnimating();
            StartCoroutine(powerAnimation);
            
        }
        //FIRE
        if(interactionCycle == 3)
        {
            StopCoroutine(powerAnimation);
            //FIRE
            FireCannon();

            currentPower = 0f;
            powerBar.localScale = new Vector2(0f, powerBar.localScale.y);
            poweringUpwards = true;
            interactionCycle = 0;
        }
    }

    /// <summary>
    /// Leaves cannon in its current state
    /// </summary>
    public override void LeaveInteract()
    {
        if (!interacting) { return; }
        interacting = false;

        if(interactionCycle == 1)
        {
            StopCoroutine(aimAnimation);
            interactionCycle--;
        }
        if(interactionCycle == 2)
        {
            StopCoroutine(powerAnimation);
            interactionCycle--;
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
                cannonPivot.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
                currentRotation += rotationSpeed * Time.deltaTime;

                if (currentRotation > upperAngle)
                {
                    rotatingToUpperAngle = false;
                    currentRotation = upperAngle;
                }
            }

            if (!rotatingToUpperAngle)
            {
                cannonPivot.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime, Space.Self);
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

    private IEnumerator PowerSetStartAnimating()
    {
        while (true)
        {
            if (poweringUpwards)
            {
                currentPower += powerSpeed * Time.deltaTime;
                powerBar.localScale = new Vector2(currentPower, powerBar.localScale.y);

                if (currentPower >= 0.99f)
                {
                    poweringUpwards = false;
                }
            }

            if (!poweringUpwards)
            {
                currentPower -= powerSpeed * Time.deltaTime;
                powerBar.localScale = new Vector2(currentPower, powerBar.localScale.y);

                if (currentPower <= 0.01f)
                {
                    poweringUpwards = true;
                }
            }
            yield return null;
        }
    }

    private void FireCannon()
    {
        GameObject projectileClone = Instantiate(projectile, projectileSpawnPos.position, projectileSpawnPos.rotation, null);
        projectileClone.SetActive(true);
        Rigidbody2D projectileRb = projectileClone.GetComponent<Rigidbody2D>();
        projectileRb.gravityScale = 1f;
        projectileRb.AddForce(projectileClone.transform.right * cannonballPower, ForceMode2D.Impulse);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.root.CompareTag("Player"))
        {
            LeaveInteract();
            PlayerController pCon;
            pCon = collision.transform.gameObject.GetComponent<PlayerController>();
            pCon.ClearCurrentInteraction();
        }
    }
}
