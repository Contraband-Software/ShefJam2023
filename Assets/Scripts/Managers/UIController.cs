using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class UIController : MonoBehaviour
    {
        [Header("Warnings")]
        [SerializeField] GameObject leftGenerator;
        [SerializeField] GameObject leftTurbine;
        [SerializeField] GameObject rightGenerator;
        [SerializeField] GameObject rightTurbine;

        private void Start()
        {
            GameManager.GetReference().GameOverEvent.AddListener(OnGameOver);

            List<Architecture.BreakDownRepairStation> failStations = FindObjectsOfType<Architecture.BreakDownRepairStation>().ToList();
            foreach (Architecture.BreakDownRepairStation station in failStations)
            {
                station.DamageToggleEvent.AddListener(StationDamageToggle);
            }

            DisableAllStationWarnings();
        }

        private void DisableAllStationWarnings()
        {
            leftGenerator.SetActive(false);
            leftTurbine.SetActive(false);
            rightGenerator.SetActive(false);
            rightTurbine.SetActive(false);
        }

        private void StationDamageToggle(Architecture.InteractableBase.ObjectType type, GameManager.PlayerIndex player, bool damaged)
        {
            if(player == GameManager.PlayerIndex.ONE)
            {
                switch (type)
                {
                    case Architecture.InteractableBase.ObjectType.GENERATOR:
                        leftGenerator.gameObject.SetActive(damaged);
                        break;

                    case Architecture.InteractableBase.ObjectType.TURBINE:
                        leftTurbine.gameObject.SetActive(damaged);
                        break;

                    default:
                        break;
                }
            }

            else if (player == GameManager.PlayerIndex.TWO)
            {
                switch (type)
                {
                    case Architecture.InteractableBase.ObjectType.GENERATOR:
                        rightGenerator.gameObject.SetActive(damaged);
                        break;

                    case Architecture.InteractableBase.ObjectType.TURBINE:
                        rightTurbine.gameObject.SetActive(damaged);
                        break;

                    default:
                        break;
                }
            }
        }

        private void OnGameOver(GameManager.GameOverReason reason, GameManager.PlayerIndex player)
        {
            string gameOverMessage;
            switch (player)
            {
                case GameManager.PlayerIndex.ONE:
                    gameOverMessage = "Player one";
                    break;
                case GameManager.PlayerIndex.TWO:
                    gameOverMessage = "Player two";
                    break;
                default:
                    throw new System.ArgumentException("Must specifiy one of the two players that lost.");
            }

            switch (reason)
            {
                case GameManager.GameOverReason.GENERATOR_DESTROYED:
                    gameOverMessage += "'s ship sank due to their generator exploding!";
                    break;
                case GameManager.GameOverReason.PLAYER_KILLED:
                    gameOverMessage += " ate a cannon ball.";
                    break;
                case GameManager.GameOverReason.TURBINE_DESTROYED:
                    gameOverMessage += " sank to the depths due to their turbine being destroyed...";
                    break;
                case GameManager.GameOverReason.PLAYER_FELL:
                    gameOverMessage += " went to go visit satan.";
                    break;
            }

            // Now put gameOverMessage on a game over screen
            // perhaps show a restart button
        }
    }
}