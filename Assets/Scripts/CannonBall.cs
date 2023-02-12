using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using UnityEngine.Tilemaps;

namespace Architecture
{
    [
        RequireComponent(typeof(CircleCollider2D)),
        DisallowMultipleComponent
    ]
    public class CannonBall : MonoBehaviour
    {
        [SerializeField] CircleCollider2D circleCollider;
        [SerializeField, Min(0)] int power = 3;

        int damage = 0;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!CheckExpired())
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Platform") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    var ship = collision.gameObject.transform.parent.GetComponent<BuildingSystem>();

                    damage += ship.DestroyBlock(transform.position);
                } else if (collision.gameObject.layer == LayerMask.NameToLayer("FailStations"))
                {
                    collision.gameObject.GetComponent<BreakDownRepairStation>()?.OnCannonballHit();
                    damage = 1000000;
                }
                else if(collision.gameObject.CompareTag("Player"))
                {
                    collision.gameObject.GetComponent<PlayerController>().DeathViaCannonball();
                }
            } else
            {
                Expire(collision);
            }
        }

        bool CheckExpired()
        {
            return !(damage < power);
        }

        void Expire(Collider2D collider)
        {
            circleCollider.isTrigger = false;
            var ship = collider.gameObject.transform.parent.GetComponent<BuildingSystem>();
            ship?.DestroyBlock(transform.position);
        }
    }
}