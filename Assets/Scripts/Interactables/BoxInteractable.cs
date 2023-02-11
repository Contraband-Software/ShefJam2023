using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxInteractable : InteractableBase
{
    [SerializeField] SpriteRenderer spriteRend;

    public override void Interact()
    {
        spriteRend.color = Color.green;
    }

    public override void LeaveInteract()
    {
        
    }
}
