using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture
{
    public class SteeringWheel : InteractableBase
    {
        private void Start()
        {
            objectType = ObjectType.WHEEL;
        }

        public override void Interact()
        {
            print("Wheel 1");
        }

        public override void LeaveInteract()
        {
            print("Wheel 2");
        }
    }
}