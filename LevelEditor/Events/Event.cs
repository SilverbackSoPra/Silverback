using System;
using System.Windows.Forms;
using LevelEditor.UIv2;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace LevelEditor.Events
{
    enum EventType
    {
        Mouse,
        Keyboard
    }

    sealed class Event
    {
        public readonly Keys mKey;
        public readonly MouseButtons mMouseButton;
        public InputState InputState { get; private set; }
        public Action Action { get; private set; }

        public EventType EventType { get; private set; }

        public Event(Keys k, InputState inputState, Action a)
        {
            mMouseButton = MouseButtons.None;
            EventType = EventType.Keyboard;
            Setup(inputState, a);
        }

        public Event(MouseButtons mb, InputState inputState, Action a)
        {
            mMouseButton = mb;
            EventType = EventType.Mouse;
            Setup(inputState, a);
        }

        private void Setup(InputState inputState, Action a)
        {
            InputState = inputState;
            Action = a;
        }
    }
}
