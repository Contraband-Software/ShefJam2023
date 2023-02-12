using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers {
    [DisallowMultipleComponent]
    public class GameManager : MonoBehaviour
    {
        public class GameOverEventType : UnityEngine.Events.UnityEvent<GameOverReason, PlayerIndex> { }
        public GameOverEventType GameOverEvent { get; private set; } = new GameOverEventType();

        public enum GameOverReason
        {
            NONE,
            GENERATOR_DESTROYED,
            TURBINE_DESTROYED,
            PLAYER_KILLED,
            PLAYER_FELL
        }

        public enum PlayerIndex
        {
            ONE,
            TWO,
            NEITHER
        }

        public enum State
        {
            PLAYING,
            OVER
        }

        public State GameState { get; private set; } = State.PLAYING;

        private void SetState(State state, GameOverReason reason = GameOverReason.NONE, PlayerIndex player = PlayerIndex.NEITHER)
        {
            GameState = state;
            switch (state)
            {
                case State.OVER:
                    GameOverEvent.Invoke(reason, player);
                    break;
                case State.PLAYING:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    break;
            }
        }

        public void Restart()
        {
            SetState(State.PLAYING);
        }

        public static GameManager GetReference()
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

        public void GameOver(GameOverReason reason, PlayerIndex player)
        {
            SetState(State.OVER, reason, player);
            print("Player " + player.ToString() + " lost due to: " + reason.ToString());
        }
    }
}