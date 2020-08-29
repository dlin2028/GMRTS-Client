using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient
{
    public static class InputManager
    {
        //Approval By Kevin ✔
        //remember kevin k? (i think it was kevin k)
        public static MouseState MouseState { get; set; }
        public static MouseState LastMouseState { get; private set; }
        public static KeyboardState Keys { get; private set; }
        public static KeyboardState LastKeys { get; private set; }

        public static void Update()
        {
            LastMouseState = MouseState;
            MouseState = Mouse.GetState();

            LastKeys = Keys;
            Keys = Keyboard.GetState();
        }
    }
}
