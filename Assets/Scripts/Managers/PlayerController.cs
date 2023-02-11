using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Managers;
using UnityEngine.Playables;

namespace Architecture
{
    [
        RequireComponent(typeof(Rigidbody2D)),
        SelectionBase
    ]
    public class PlayerController : MonoBehaviour
    {
        public enum FacingDirection
        {
            LEFT,
            RIGHT
        }

        public enum State
        {
            HOLDING_NOTHING,
            HOLDING_BUILDING_BLOCK,
            HOLDING_CANNON_BALL
        }

        #region REFERENCES
        [Header("Grounding")]
        [SerializeField] LayerMask groundLayer;
        [SerializeField] Transform groundCheck;

        #endregion

        #region ATTRIBUTES
        private List<InteractableBase> InteractablesInRange = new List<InteractableBase>();
        private Rigidbody2D rb;
        private FacingDirection facingDirection = FacingDirection.RIGHT;
        private BuildingSystem building;
        private State playerState = State.HOLDING_BUILDING_BLOCK;
        #endregion

        #region SETTINGS_VARIABLES

        private enum PlayerControlType { WASD, ARROWS };
        [Space(15)]
        [Header("Settings")]
        [SerializeField] PlayerControlType playerControlType = new PlayerControlType { };

        [Header("Player Variables")]
        [SerializeField] float speed;
        [SerializeField] float jumpingPower;
        private float horizontal;
        #endregion

        #region INITIALIZATION
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            building = BuildingSystem.GetReference();
            SubscribeToInputHandler();
        }
        /// <summary>
        /// Chooses whether to subscribe to WASD or ARROW events
        /// </summary>
        private void SubscribeToInputHandler()
        {
            if (playerControlType == PlayerControlType.WASD)
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

        private void ChangeState(State state)
        {
            switch (state)
            {
                case State.HOLDING_BUILDING_BLOCK:
                    break;
            }
        }

        private Vector3 GetBuildingOffset()
        {
            return new Vector3((facingDirection == FacingDirection.RIGHT) ? 1 : -1, -1, 0);
        }

        private void Update()
        {
            if (playerState == State.HOLDING_BUILDING_BLOCK)
            {
                building.UpdateGhostBlock(transform.position, (int)GetBuildingOffset().x, (int)GetBuildingOffset().y);
            }
        }

        private void FixedUpdate()
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }

        #region PLAYER_CONTROLS
        public void Move(InputAction.CallbackContext context)
        {
            horizontal = context.ReadValue<Vector2>().x;
            if (horizontal == 0) { }
            else if (horizontal < 0)
            {
                facingDirection = FacingDirection.LEFT;
            } else
            {
                facingDirection = FacingDirection.RIGHT;
            }
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
                switch (playerState)
                {
                    case State.HOLDING_NOTHING:
                        InteractWithNearestObject();
                        break;
                    case State.HOLDING_BUILDING_BLOCK:
                        building.PlaceBlock(BuildingSystem.BlockType.WOOD, transform.position + new Vector3(GetBuildingOffset().x * building.tileMapGrid.cellSize.x, GetBuildingOffset().y * building.tileMapGrid.cellSize.y, 0));
                        break;
                    case State.HOLDING_CANNON_BALL:
                        break;
                }
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
            if (InteractablesInRange.Count == 0)
            {
                return;
            }

            //calculate closest interactable
            float nearestDist = Mathf.Infinity;
            InteractableBase nearestObj = null;
            foreach (InteractableBase obj in InteractablesInRange)
            {
                float dist = Vector2.Distance(obj.transform.position, gameObject.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestObj = obj;
                }
            }

            //activate their interact() function
            nearestObj?.Interact();

        }

        #endregion
    }
}