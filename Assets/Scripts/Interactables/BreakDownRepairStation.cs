using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Architecture
{
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class BreakDownRepairStation : InteractableBase
    {
        public class DamagedEventType : UnityEvent<ObjectType, Managers.GameManager.PlayerIndex, bool> { }
        public DamagedEventType DamageToggleEvent { get; private set; } = new DamagedEventType();
        public Managers.GameManager.GameOverEventType Destroyed { get; private set; } = new Managers.GameManager.GameOverEventType();

        [Header("References")]
        [SerializeField] GameObject timeLeftSlider;
        [SerializeField] ParticleSystem explosionPFX;
        [SerializeField] ParticleSystem smokePFX;

        [Header("Settings")]
        [SerializeField] int breakdownToLoss = 120;
        [SerializeField] ObjectType stationType;

        private bool broken = false;
        private float timeLeft;
        private Vector3 originalScale;

        private void Start()
        {
            objectType = stationType;

            originalScale = timeLeftSlider.transform.localScale;
            timeLeftSlider.SetActive(false);
        }

        public void OnCannonballHit()
        {
            if (!broken)
            {
                Managers.SoundSystem.Instance.PlaySound("Hit2");
                smokePFX.Play();
                broken = true;
                timeLeft = breakdownToLoss;
                timeLeftSlider.SetActive(true);
                DamageToggleEvent.Invoke(objectType, playerIndex, true);
            }
        }

        public override void Interact()
        {
            if (broken)
            {
                smokePFX.Stop();
                broken = false;
                timeLeftSlider.SetActive(false);
                DamageToggleEvent.Invoke(objectType, playerIndex, false);
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
                    //blow up
                    smokePFX.Stop();
                    explosionPFX.Play();
                    broken = false;
                    timeLeftSlider.SetActive(false);

                    if(stationType == ObjectType.GENERATOR)
                    {
                        Destroyed.Invoke(Managers.GameManager.GameOverReason.GENERATOR_DESTROYED, playerIndex);
                    }
                    else if(stationType == ObjectType.TURBINE)
                    {
                        Destroyed.Invoke(Managers.GameManager.GameOverReason.TURBINE_DESTROYED, playerIndex);
                    }
                }
            }
        }

        public bool IsBroken()
        {
            return broken;
        }
    }
}