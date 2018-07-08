using Microsoft.Xna.Framework.Input;

namespace LevelEditor.Events
{
    public enum InputType
    {
        Mouse,
        Keyboard,
    }

    internal sealed class InputEvent
    {
        private static int LastScrollValue { get; set; }
        private static int CurrentScrollValue { get; set; }

        private static MouseState PreviousMouseState { get; set; }
        public static MouseState MouseState { get; set; }

        public InputEvent(MouseState m)
        {
            Used = false;
            MouseState = m;
            LastScrollValue = m.ScrollWheelValue;
            CurrentScrollValue = m.ScrollWheelValue;
        }

        public InputEvent(MouseState m, bool multiClick)
        {
            Used = false;
            MouseState = m;
            LastScrollValue = m.ScrollWheelValue;
            CurrentScrollValue = m.ScrollWheelValue;
        }

        public static void UpdateMouse()
        {
            PreviousMouseState = MouseState;
            MouseState = Mouse.GetState();
        }

        public static void UpdateScrollWheelValue()
        {
            MouseState = Mouse.GetState();
            LastScrollValue = CurrentScrollValue;
            CurrentScrollValue = MouseState.ScrollWheelValue;
        }
        public InputEvent(KeyboardState k)
        {
            Used = false;
            KeyboardState = k;
        }
        public InputType Type { get; set; }
        private bool Used { get; set; }

        private KeyboardState KeyboardState { get; set; }

    }

}
