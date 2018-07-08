using System;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace LevelEditor.UIv2.Components
{
    interface IMenuComponent
    {
        void AddTo(Menu m);
        void AddTo(ScrollList s);

        void AddListener(MouseButtons mb, InputState inputState, Action a);

        void AddListener(Keys k, InputState inputState, Action a);
    }
}
