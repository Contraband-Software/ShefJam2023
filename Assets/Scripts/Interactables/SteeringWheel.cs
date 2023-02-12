using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Architecture
{
    public class SteeringWheel : InteractableBase
    {
        [Header("References")]
        [SerializeField] GameObject fuelSlider;

        [Header("Settings")]
        [SerializeField, Min(0)] float MinHeightDelta = 0.5f;
        [SerializeField, Min(0)] float MaxHeightDelta = 0.5f;
        [SerializeField, Range(0, 1)] float Speed = 0.5f;
        [SerializeField, Min(0)] float MaxMoveDelta;
        [SerializeField, Min(0)] float rechargeSpeed = 1;

        private float originalY;
        private float vertical;
        private float moveDelta;
        private bool inUse = false;
        private Transform player;
        private Vector3 originalScale;

        private void Start()
        {
            objectType = ObjectType.WHEEL;

            originalY = transform.position.y;

            if (playerIndex == GameManager.PlayerIndex.ONE)
            {
                InputHandler.Instance.WASD_MoveEvent.AddListener(Down);
                InputHandler.Instance.WASD_JumpEvent.AddListener(Up);
            }
            else if (playerIndex == GameManager.PlayerIndex.TWO)
            {
                InputHandler.Instance.ARROWS_MoveEvent.AddListener(Down);
                InputHandler.Instance.ARROWS_JumpEvent.AddListener(Up);
            }
            else
            {
                throw new System.ArgumentException("Player index must be set to one or two.");
            }

            List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
            foreach (GameObject p in players)
            {
                if (p.GetComponent<PlayerController>().GetPlayerIndex() == playerIndex)
                {
                    player = p.transform;
                    break;
                }
            }

            originalScale = fuelSlider.transform.localScale;
        }

        private void Down(InputAction.CallbackContext context)
        {
            vertical = context.ReadValue<Vector2>().y;
        }
        private void Up(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                vertical = 1;
            } else
            {
                vertical = 0;
            }
        }

        private void FixedUpdate()
        {
            if (inUse && moveDelta < MaxMoveDelta)
            {

                //GOING UP
                if(vertical == 1)
                {
                    print("GOING UP");
                    if(transform.position.y < originalY + MaxHeightDelta)
                    {
                        transform.parent.transform.Translate(new Vector3(0, vertical * Speed, 0));
                        player.Translate(new Vector3(0, vertical * Speed, 0));
                        //moveDelta += Mathf.Abs(vertical * Speed);
                    }
                }
                //GOING DOWN
                else if(vertical == -1)
                {
                    print("GOING DOWN");
                    if (transform.position.y > originalY - MinHeightDelta)
                    {
                        transform.parent.transform.Translate(new Vector3(0, vertical * Speed, 0));
                        player.Translate(new Vector3(0, vertical * Speed, 0));
                        //moveDelta += Mathf.Abs(vertical * Speed);
                    }
                }

                /*//MOVE WITHIN BOTH BOUNDS
                if (transform.position.y < originalY + MaxHeightDelta && transform.position.y > originalY - MinHeightDelta)
                {
                    transform.parent.transform.Translate(new Vector3(0, vertical * Speed, 0));
                    player.Translate(new Vector3(0, vertical * Speed, 0));
                    moveDelta += Mathf.Abs(vertical * Speed);
                } 
                //MOVING WHEN AT A BOUND
                else
                {
                    if (transform.position.y >= originalY + MaxHeightDelta)
                    {
                        transform.parent.transform.position = new Vector3(transform.parent.transform.position.x, originalY - 1 + MaxHeightDelta, transform.parent.transform.position.z);
                        player.transform.position = new Vector3(player.transform.position.x, originalY - 1 + MaxHeightDelta, player.transform.position.z);
                    } 
                    else
                    {
                        transform.parent.transform.position = new Vector3(transform.parent.transform.position.x, originalY + 1 + MinHeightDelta, transform.parent.transform.position.z);
                        player.transform.position = new Vector3(player.transform.position.x, originalY + 1 - MinHeightDelta, player.transform.position.z);
                    }
                }*/
            } 
            else
            {
                moveDelta -= Time.deltaTime * rechargeSpeed;
            }
        }

        private void Update()
        {
            fuelSlider.transform.localScale = new Vector3(Mathf.Lerp(originalScale.x, 0, moveDelta / MaxMoveDelta), originalScale.y, originalScale.z);
        }

        public override void Interact()
        {
            inUse = true;
        }

        public override void LeaveInteract()
        {
            inUse = false;
        }
    }
}