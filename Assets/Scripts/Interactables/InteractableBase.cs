using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    public enum ObjectType { UNSET, CANNONBALL, WOODBOX, METALBOX}
    [SerializeField] ObjectType objectType;

    public abstract void Interact();
}
