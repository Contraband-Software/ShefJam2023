using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture
{
    [DisallowMultipleComponent]
    public abstract class InteractableBase : MonoBehaviour
    {
        public enum ObjectType { NONE, CANNONBALL, TURBINE, CANNON, WHEEL, WOODBOX, METALBOX, GENERATOR }
        protected ObjectType objectType;
        protected Managers.GameManager.PlayerIndex playerIndex = Managers.GameManager.PlayerIndex.NEITHER;

        private void Awake()
        {
            playerIndex = transform.parent.GetComponent<Managers.PlayerShipController>().GetPlayerIndex();
        }

        public abstract void Interact();

        public abstract void LeaveInteract();

        public ObjectType GetObjectType()
        {
            return objectType;
        }
    }
}