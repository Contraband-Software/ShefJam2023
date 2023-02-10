using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Managers;

public class PlayerController : MonoBehaviour
{
    #region REFERENCES
    [Header("Player Component References")]
    [SerializeField] Rigidbody2D rb;

    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;

    #endregion

    #region ATTRIBUTES
    private List<InteractableBase> InteractablesInRange = new List<InteractableBase>();
    #endregion

    #region SETTINGS_VARIABLES

    private enum PlayerControlType { WASD, ARROWS };
    [Space][Space][Space]
    [Header(" - SETTINGS -")]
    [SerializeField] PlayerControlType playerControlType = new PlayerControlType { };
    [Header("Player Variables")]
    [SerializeField] float speed;
    [SerializeField] float jumpingPower;
    private float horizontal;
    #endregion

    #region INITIALIZATION
    private void Start()
    {
        SubscribeToInputHandler();
    }
    /// <summary>
    /// Chooses whether to subscribe to WASD or ARROW events
    /// </summary>
    private void SubscribeToInputHandler()
    {
        if(playerControlType == PlayerControlType.WASD)
        {
            SubscribeWASDEvents();
        }
        else
        {
            SubscribeARROWSEvents();
        }
    }
    /// <summary>
    /// Subscribes its controls to the WASD Events of the InputHandler
    /// </summary>
    private void SubscribeWASDEvents()
    {
        InputHandler.Instance.WASD_MoveEvent.AddListener(Move);
        InputHandler.Instance.WASD_JumpEvent.AddListener(Jump);
        InputHandler.Instance.WASD_InteractEvent.AddListener(Interact);
    }
    /// <summary>
    /// Subscribes its controls to the ARROWS Events of the InputHandler
    /// </summary>
    private void SubscribeARROWSEvents()
    {
        InputHandler.Instance.ARROWS_MoveEvent.AddListener(Move);
        InputHandler.Instance.ARROWS_JumpEvent.AddListener(Jump);
        InputHandler.Instance.ARROWS_InteractEvent.AddListener(Interact);
    }
    #endregion

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    #region PLAYER_CONTROLS
    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            InteractWithNearestObject();
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);   
    }
    #endregion

    #region INTERACTION
    /// <summary>
    /// On Entering an objects range collider, add it to the interactable objects in range
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root.CompareTag("InteractableObject"))
        {
            InteractablesInRange.Add(collision.transform.root.GetComponent<InteractableBase>());
        }
    }
    /// <summary>
    /// On Exiting an objects range collider, remove it from interactable objects in range
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.root.CompareTag("InteractableObject"))
        {
            InteractablesInRange.Remove(collision.transform.root.GetComponent<InteractableBase>());
        }
    }

    /// <summary>
    /// Calculates the object that is nearest to the player and interacts with it
    /// </summary>
    private void InteractWithNearestObject()
    {
        //dont try interact if none in range
        if(InteractablesInRange.Count == 0)
        {
            return;
        }

        //calculate closest interactable
        float nearestDist = Mathf.Infinity;
        InteractableBase nearestObj = null;
        foreach(InteractableBase obj in InteractablesInRange)
        {
            float dist = Vector2.Distance(obj.transform.position, gameObject.transform.position);
            if(dist < nearestDist)
            {
                nearestDist = dist;
                nearestObj = obj;
            }
        }

        //activate their interact() function
        nearestObj.Interact();

    }

    #endregion
}
