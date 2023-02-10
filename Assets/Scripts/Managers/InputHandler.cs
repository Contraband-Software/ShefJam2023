using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


namespace Managers
{
    public class InputHandler : Backend.AbstractSingleton<InputHandler>
    {
        #region WASD_INPUT_EVENTS
        public class WASD_MoveEventType : UnityEvent<InputAction.CallbackContext> { }
        public class WASD_JumpEventType : UnityEvent<InputAction.CallbackContext> { }
        public class WASD_InteractEventType : UnityEvent<InputAction.CallbackContext> { }

        public WASD_MoveEventType WASD_MoveEvent = new WASD_MoveEventType();
        public WASD_MoveEventType WASD_JumpEvent = new WASD_MoveEventType();
        public WASD_InteractEventType WASD_InteractEvent = new WASD_InteractEventType();
        #endregion

        #region ARROWS_INPUT_EVENTS
        public class ARROWS_MoveEventType : UnityEvent<InputAction.CallbackContext> { }
        public class ARROWS_JumpEventType : UnityEvent<InputAction.CallbackContext> { }
        public class ARROWS_InteractEventType : UnityEvent<InputAction.CallbackContext> { }
        public ARROWS_MoveEventType ARROWS_MoveEvent = new ARROWS_MoveEventType();
        public ARROWS_MoveEventType ARROWS_JumpEvent = new ARROWS_MoveEventType();
        public ARROWS_InteractEventType ARROWS_InteractEvent = new ARROWS_InteractEventType();
        #endregion


        //THIS IS THE AWAKE
        protected override void SingletonAwake()
        {

        }

        void Start()
        {

        }

        void Update()
        {

        }

        #region WASD_PLAYER_CONTROLS
        public void MoveWASD(InputAction.CallbackContext context)
        {
            WASD_MoveEvent.Invoke(context);
        }
        public void JumpWASD(InputAction.CallbackContext context)
        {
            WASD_JumpEvent.Invoke(context);
        }

        public void InteractWASD(InputAction.CallbackContext context)
        {
            WASD_InteractEvent.Invoke(context);
        }
        #endregion

        #region ARROWS_PLAYER_CONTROLS
        public void MoveARROWS(InputAction.CallbackContext context)
        {
            ARROWS_MoveEvent.Invoke(context);
        }
        public void JumpARROWS(InputAction.CallbackContext context)
        {
            ARROWS_JumpEvent.Invoke(context);
        }

        public void InteractARROWS(InputAction.CallbackContext context)
        {
            ARROWS_InteractEvent.Invoke(context);
        }
        #endregion
    }
}

