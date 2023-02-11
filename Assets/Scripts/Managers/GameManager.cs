using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Managers {
    public class GameManager : MonoBehaviour
    {
        public class GameOverEventType : UnityEngine.Events.UnityEvent<GameOverReason> { }

        public enum GameOverReason
        {
            GENERATOR_DESTROYED,
            TURBINE_DESTROYED,
            PLAYER_KILLED,
            PLAYER_FELL
        }

        public GameManager GetReference()
        {
            return GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        }

        private void Awake()
        {
            List<Architecture.BreakDownRepairStation> failStations = FindObjectsOfType<Architecture.BreakDownRepairStation>().ToList();

            foreach (Architecture.BreakDownRepairStation station in failStations)
            {
                station.Destroyed.AddListener(GameOver);
            }
        }

        public void GameOver(GameOverReason reason)
        {
            print("Game over: " + reason.ToString());
        }
    }
}