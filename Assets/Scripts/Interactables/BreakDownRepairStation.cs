using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Architecture
{
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class BreakDownRepairStation : InteractableBase
    {
        public Managers.GameManager.GameOverEventType Destroyed { get; private set; } = new Managers.GameManager.GameOverEventType();

        [Header("References")]
        [SerializeField] GameObject timeLeftSlider;

        [Header("Settings")]
        [SerializeField] int breakdownToLoss = 120;

        private bool broken = false;
        private float timeLeft;
        private Vector3 originalScale;

        private void Start()
        {
            objectType = ObjectType.TURBINE;

            originalScale = timeLeftSlider.transform.localScale;
            timeLeftSlider.SetActive(false);
        }

        public void OnCannonballHit()
        {
            broken = true;
            timeLeft = breakdownToLoss;
            timeLeftSlider.SetActive(true);
        }

        public override void Interact()
        {
            if (broken)
            {
                broken = false;
                timeLeftSlider.SetActive(false);
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

                timeLeftSlider.transform.localScale = new Vector3(Mathf.Lerp(originalScale.x, 0, 1 - timeLeft / breakdownToLoss), originalScale.y, originalScale.z);

                if (timeLeft < 0)
                {
                    broken = false;
                    timeLeftSlider.SetActive(false);
                    Destroyed.Invoke(Managers.GameManager.GameOverReason.GENERATOR_DESTROYED, playerIndex);
                }
            }
        }
    }
}