using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    [DisallowMultipleComponent]
    public class PlayerShipController : MonoBehaviour
    {
        [SerializeField] GameManager.PlayerIndex player;
        public GameManager.PlayerIndex GetPlayerIndex()
        {
            return player;
        }
    }
}