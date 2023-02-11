using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    public enum ObjectType { CANNONBALL, CANNON, WHEEL, WOODBOX, METALBOX}
    protected ObjectType objectType;

    public abstract void Interact();

    public abstract void LeaveInteract();

    public ObjectType GetObjectType()
    {
        return objectType;
    }
}
