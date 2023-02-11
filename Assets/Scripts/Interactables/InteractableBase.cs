using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    public enum ObjectType { UNSET, CANNONBALL, CANNON, WHEEL, WOODBOX, METALBOX}
    public ObjectType objectType;

    public abstract void Interact();

    public abstract void LeaveInteract();
}
