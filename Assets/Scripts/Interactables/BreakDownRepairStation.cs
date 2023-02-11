using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Architecture
{
    public class BreakDownRepairStation : InteractableBase
    {
        public UnityEvent Destroyed { get; private set; } = new UnityEvent();

        [Header("Settings")]
        [SerializeField] int breakdownToLoss = 120;

        private bool broken = false;
        private float timeLeft;

        public void OnCannonballHit()
        {
            broken = true;
            timeLeft = breakdownToLoss;
        }

        public override void Interact()
        {
            if (broken)
            {
                broken = false;
            }
        }

        public override void LeaveInteract()
        {

        }

        private void Update()
        {
            if (broken)
            {
                timeLeft -= Time.deltaTime;
                broken = false;
                Destroyed.Invoke();
            }
        }
    }
}