using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Managers;
using UnityEngine.Tilemaps;
using TMPro;

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

        public enum Motion
        {
            IDLE,
            MOVING,
            AIR
        }

        public enum State
        {
            HOLDING_NOTHING,
            HOLDING_BUILDING_BLOCK,
            HOLDING_CANNON_BALL,
            USING_STATION,
            USING_WHEEL
        }

        #region REFERENCES
        [Header("Grounding")]
        [SerializeField] LayerMask groundLayer;
        [SerializeField] Transform groundCheck;
        [SerializeField] BuildingSystem building;

        [Header("Animation")]
        [SerializeField] Animator animator;
        [SerializeField] SpriteRenderer spriteRend;

        [Header("Item Holding")]
        [SerializeField] Transform holdPoint;
        [SerializeField] TextMeshProUGUI blockText;
        #endregion

        #region ATTRIBUTES
        private List<InteractableBase> InteractablesInRange = new List<InteractableBase>();
        private Rigidbody2D rb;
        private FacingDirection facingDirection = FacingDirection.RIGHT;
        private Motion playerMotion = Motion.IDLE;
        private BuildingSystem.BlockType holdingBlock;
        private int blocksLeft = 0;
        [SerializeField] State playerState = State.HOLDING_BUILDING_BLOCK;
        private InteractableBase currentInteraction;
        private float horizontal;
        private float vertical;
        #endregion

        #region SETTINGS_VARIABLES

        [Space(15)]
        [Header("Settings")]
        [SerializeField] GameManager.PlayerIndex playerNumber = GameManager.PlayerIndex.ONE;
        public GameManager.PlayerIndex GetPlayerIndex()
        {
            return playerNumber;
        }

        [Header("Player Variables")]
        [SerializeField] float speed;
        [SerializeField] float jumpingPower;
        [SerializeField] int blocksPickUp = 5;
        #endregion

        #region UNITY
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            if (playerNumber == GameManager.PlayerIndex.ONE)
            {
                InputHandler.Instance.WASD_MoveEvent.AddListener(Move);
                InputHandler.Instance.WASD_JumpEvent.AddListener(Jump);
                InputHandler.Instance.WASD_InteractEvent.AddListener(Interact);
            }
            else if (playerNumber == GameManager.PlayerIndex.TWO)
            {
                InputHandler.Instance.ARROWS_MoveEvent.AddListener(Move);
                InputHandler.Instance.ARROWS_JumpEvent.AddListener(Jump);
                InputHandler.Instance.ARROWS_InteractEvent.AddListener(Interact);
            }
            else
            {
                throw new System.ArgumentException("Player index must be set to one or two.");
            }
        }

        private void Update()
        {
            //Platform logic
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Platform"), vertical < 0 || rb.velocity.y > 0);

            if (playerState == State.HOLDING_BUILDING_BLOCK)
            {
                building.UpdateGhostBlock(transform.position, (int)GetBuildingOffset().x, (int)GetBuildingOffset().y);
            }
            MotionStateSwitch();
            UpdateBlockDisplay();
            MoveAnimation();
        }

        private void FixedUpdate()
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y + ((vertical < 0) ? vertical : 0));
        }
        #endregion

        private void ChangeState(State state)
        {
            playerState = state;
            switch (state)
            {
                case State.HOLDING_BUILDING_BLOCK:
                    break;
            }
        }

        private Vector3 GetBuildingOffset()
        {
            int x = (facingDirection == FacingDirection.RIGHT) ? 1 : -1;
            return new Vector3(x, (building.CheckOccupancy(new Vector2(transform.position.x + x * building.tileMapGrid.cellSize.x, transform.position.y - building.tileMapGrid.cellSize.y))) ? 0 : -1);
        }

        #region PLAYER_CONTROLS
        public void Move(InputAction.CallbackContext context)
        {
            if (playerState != State.USING_WHEEL)
            {
                vertical = context.ReadValue<Vector2>().y;
            } else
            {
                vertical = 0;
            }

            horizontal = context.ReadValue<Vector2>().x;
            if (horizontal == 0) { IdleAnimation(); }
            else if (horizontal < 0)
            {
                facingDirection = FacingDirection.LEFT;
                spriteRend.flipX = true;
            }
            else
            {
                facingDirection = FacingDirection.RIGHT;
                spriteRend.flipX = false;
            }
            
            if (playerState != State.USING_WHEEL || (playerState == State.USING_WHEEL && vertical == 0 && horizontal != 0))
            {
                LeaveInteract();
            }
        }

        public void Jump(InputAction.CallbackContext context)
        {
            if (playerState != State.USING_WHEEL)
            {
                if (context.performed && IsGrounded())
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                    playerMotion = Motion.AIR;
                    IdleAnimation();
                }
                LeaveInteract();
            }
        }

        public void Interact(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InteractableBase station = GetNearestInteractableObject();
                if(station != null)
                {
                    currentInteraction = station;
                }

                switch (playerState)
                {
                    case State.HOLDING_NOTHING:
                    case State.USING_STATION:
                        if (station != null)
                        {
                            InteractWithObject(station.GetObjectType());
                        }               
                        break;
                    case State.HOLDING_BUILDING_BLOCK:

                        if(station != null && 
                            (station.GetObjectType() == InteractableBase.ObjectType.WOODBOX || 
                            station.GetObjectType() == InteractableBase.ObjectType.METALBOX))
                        {
                            InteractWithObject(station.GetObjectType());
                        }

                        else if(blocksLeft != 0)
                        {
                            if (building.PlaceBlock(holdingBlock, transform.position + new Vector3(GetBuildingOffset().x * building.tileMapGrid.cellSize.x, GetBuildingOffset().y * building.tileMapGrid.cellSize.y, 0)))
                            {
                                blocksLeft--;
                                if(blocksLeft == 0)
                                {
                                    playerState = State.HOLDING_NOTHING;
                                    RemoveHeldObject_Destroy();
                                }
                            }
                        }                     
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
            if (collision.transform.parent.CompareTag("InteractableObject"))
            {
                InteractablesInRange.Add(collision.transform.parent.GetComponent<InteractableBase>());
            }
        }
        /// <summary>
        /// On Exiting an objects range collider, remove it from interactable objects in range
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.parent.CompareTag("InteractableObject"))
            {
                InteractablesInRange.Remove(collision.transform.parent.GetComponent<InteractableBase>());
            }
        }

        /// <summary>
        /// Calculates the object that is nearest to the player and interacts with it
        /// </summary>
        private InteractableBase GetNearestInteractableObject()
        {
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

            return nearestObj;
        }

        #endregion

        #region OBJECT_INTERACTIONS
        private void InteractWithObject(InteractableBase.ObjectType objectType)
        {
            print(objectType.ToString());
            switch (objectType)
            {
                case InteractableBase.ObjectType.WHEEL:
                    print("using");
                    ChangeState(State.USING_WHEEL);
                    currentInteraction.Interact();
                    break;
                case InteractableBase.ObjectType.CANNON:
                    if (playerState == State.HOLDING_NOTHING || currentInteraction.GetObjectType() == InteractableBase.ObjectType.CANNON)
                    {
                        ChangeState(State.USING_STATION);
                        currentInteraction.Interact();
                    }
                    break;

                case InteractableBase.ObjectType.CANNONBALL:
                    ChangeState(State.HOLDING_CANNON_BALL);
                    break;

                case InteractableBase.ObjectType.WOODBOX:
                case InteractableBase.ObjectType.METALBOX:
                    BoxInteractable boxInteraction = currentInteraction.GetComponent<BoxInteractable>();
                    if(playerState == State.HOLDING_NOTHING && !boxInteraction.IsEmpty())
                    {
                        EquipBlockOfType(boxInteraction.GetBlockType(), boxInteraction);
                        ChangeState(State.HOLDING_BUILDING_BLOCK);
                    }
                    else if(playerState == State.HOLDING_BUILDING_BLOCK && holdingBlock == boxInteraction.GetBlockType()
                        && !boxInteraction.IsEmpty())
                    {
                        EquipBlockOfType(boxInteraction.GetBlockType(), boxInteraction);
                        ChangeState(State.HOLDING_BUILDING_BLOCK);
                    }
                    break;

                case InteractableBase.ObjectType.TURBINE:
                    ChangeState(State.USING_STATION);
                    currentInteraction.Interact();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Decrease amount of blocks left in box by PICKUPAMOUNT
        /// Change holding block enum to the type of box
        /// Spawn block being held by player
        /// </summary>
        /// <param name="block"></param>
        /// <param name="boxInter"></param>
        private void EquipBlockOfType(BuildingSystem.BlockType block, BoxInteractable boxInter)
        {
            holdingBlock = block;

            int increaseInBlocks = Mathf.Clamp(blocksPickUp - blocksLeft, 0, boxInter.GetBlocksStored());

            boxInter.DecreaseBlocksStored(increaseInBlocks);
            blocksLeft += increaseInBlocks;
            
            //INSTANTIATE BLOCK ONTO PLAYER
            if(holdPoint.transform.childCount == 0)
            {
                GameObject blockInstance = GameObject.Instantiate(boxInter.GetBlockInstance(), holdPoint.transform);
                blockInstance.SetActive(true);
                blockInstance.transform.localPosition = Vector3.zero;
            }     
        }

        /// <summary>
        /// Deletes the held object completely (as opposed to dropping it)
        /// </summary>
        public void RemoveHeldObject_Destroy()
        {
            if(holdPoint.transform.childCount == 0) { return; }
            Destroy(holdPoint.transform.GetChild(0).gameObject);
        }

        /// <summary>
        /// Uninteracts with current interactable
        /// </summary>
        private void LeaveInteract()
        {
            if(playerState == State.USING_STATION || playerState == State.USING_WHEEL)
            {
                currentInteraction.LeaveInteract();
                currentInteraction = null;
                playerState = State.HOLDING_NOTHING;
            }   
        }

        private void UpdateBlockDisplay()
        {
            if(blocksLeft == 0)
            {
                blockText.text = string.Empty;
            }
            else
            {
                blockText.text = blocksLeft.ToString();
            }
        }

        private void HighlightNearestInteractable()
        {

        }
        #endregion

        #region ANIMATION
        /// <summary>
        /// Checks if the player has landed after jumping
        /// </summary>
        private void MotionStateSwitch()
        {

            if (IsGrounded() && playerMotion == Motion.AIR)
            {
                playerMotion = Motion.IDLE;
            }

            if(IsGrounded() && rb.velocity.x == 0)
            {
                playerMotion = Motion.IDLE;
            }

            if (!IsGrounded())
            {
                playerMotion = Motion.AIR;
            }

            if(playerMotion != Motion.AIR && rb.velocity.x != 0)
            {
                playerMotion = Motion.MOVING;
            }
        }
        private void MoveAnimation()
        {
            if(playerMotion == Motion.MOVING)
            {
                animator.Play("Run");
            }
            else
            {
                playerMotion = Motion.IDLE;
                IdleAnimation();
            }
        }

        private void IdleAnimation()
        {
            animator.Play("Idle");
        }
        #endregion

        #region PUBLIC_INTERFACE
        public void ClearCurrentInteraction()
        {
            currentInteraction = null;
        }
        public void ClearState()
        {
            playerState = State.HOLDING_NOTHING;
        }
        #endregion
    }
}