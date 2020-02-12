using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LevelEditor.UIv2.Components;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace LevelEditor.Events
{
    public static class InputManager
    {
        public enum MouseInput
        {
            LeftButton,
            MiddleButton,
            RightButton,
            MouseWheel,
        }

        private static KeyboardState PreviousKeyState { get; set; }
        private static KeyboardState KeyState { get; set; }

        private static MouseState PreviousMouseState { get; set; }
        private static MouseState MouseState { get; set; }

        private static List<Keys> BlockList { get; set; }
        private static List<MouseButtons> BlockListMouse { get; set; }

        private static Inputbox sInputbox;
        private static bool sCapsLocks;
        
        internal static void Update()
        {
            if (BlockList == null)
            {
                BlockList = new List<Keys>();
            }

            if (BlockListMouse == null)
            {
                BlockListMouse = new List<MouseButtons>();
            }

            PreviousKeyState = KeyState;
            KeyState = Keyboard.GetState();
            BlockList.Clear();

            PreviousMouseState = MouseState;
            MouseState = Mouse.GetState();
            BlockListMouse.Clear();

            if (sInputbox == null)
            {
                return;
            }

            if (AllKeysPressed(Keys.CapsLock))
            {
                sCapsLocks = !sCapsLocks;
            }


            var big = false || sCapsLocks;

            if (AllKeysDown(Keys.LeftShift) || AllKeysDown(Keys.RightShift))
            {
                big = !big;
            }

            var pressedKeys = Enum.GetValues(typeof(Keys))
                .Cast<Keys>()
                .Where(key => AllKeysPressed(key));
            
            foreach (var key in pressedKeys)
            {
                if (key == Keys.Back && sInputbox.Text.Length > 0)
                {
                    if (sInputbox.mCursorPos == 0)
                    {
                        continue;
                    }

                    sInputbox.Text = sInputbox.Text.Substring(0, sInputbox.mCursorPos - 1);//  + sInputbox.Text.Substring(sInputbox.mCursorPos + 1, sInputbox.Text.Length - 1);
                    DecreaseCursor();
                    continue;
                }

                if (key == Keys.Back)
                {
                    continue;
                }

                if (key == Keys.Left)
                {
                    DecreaseCursor();
                    continue;
                }

                if (key == Keys.Right)
                {
                    IncreaseCursor();
                    continue;
                }
                
                if (key == Keys.Space)
                {
                    AddToInputText(" ");
                    continue;
                }

                if (key == Keys.Enter)
                {
                    AddToInputText("\n");
                    continue;
                }

                if (key == Keys.NumPad0)
                {
                    AddToInputText("0");
                    continue;
                }

                if (key == Keys.NumPad1)
                {
                    AddToInputText("1");
                    continue;
                }

                if (key == Keys.NumPad2)
                {
                    AddToInputText("2");
                    continue;
                }

                if (key == Keys.NumPad3)
                {
                    AddToInputText("3");
                    continue;
                }

                if (key == Keys.NumPad4)
                {
                    AddToInputText("4");
                    continue;
                }

                if (key == Keys.NumPad5)
                {
                    AddToInputText("5");
                    continue;
                }

                if (key == Keys.NumPad6)
                {
                    AddToInputText("6");
                    continue;
                }

                if (key == Keys.NumPad7)
                {
                    AddToInputText("7");
                    continue;
                }

                if (key == Keys.NumPad8)
                {
                    AddToInputText("8");
                    continue;
                }

                if (key == Keys.NumPad9)
                {
                    AddToInputText("9");
                    continue;
                }
                
                if (key == Keys.D0)
                {
                    AddToInputText(big ? "=" : "0");
                    continue;
                }

                if (key == Keys.D1)
                {
                    AddToInputText(big ? "!" : "1");
                    continue;
                }

                if (key == Keys.D2)
                {
                    AddToInputText(big ? "\'" : "2"); // Only single quotes, otherwise it is going to crash
                    continue;
                }

                if (key == Keys.D3)
                {
                    AddToInputText("3");
                    continue;
                }

                if (key == Keys.D4)
                {
                    AddToInputText(big ? "$" : "4");
                    continue;
                }

                if (key == Keys.D5)
                {
                    AddToInputText(big ? "%" : "5");
                    continue;
                }

                if (key == Keys.D6)
                {
                    AddToInputText(big ? "&" : "6");
                    continue;
                }

                if (key == Keys.D7)
                {
                    AddToInputText(big ? "/" : "7");
                    continue;
                }

                if (key == Keys.D8)
                {
                    AddToInputText(big ? "(" : "8");
                    continue;
                }

                if (key == Keys.D9)
                {
                    AddToInputText(big ? ")" : "9");
                    continue;
                }
                
                if (key == Keys.OemMinus)
                {
                    AddToInputText("-");
                    continue;
                }
                
                if (key == Keys.OemOpenBrackets)
                {
                    AddToInputText("?");
                    continue;
                }

                if (key == Keys.OemComma)
                {
                    AddToInputText(",");
                    continue;
                }

                if (key == Keys.OemQuestion)
                {
                    AddToInputText("ß");
                    continue;
                }

                if (key == Keys.OemMinus)
                {
                    AddToInputText("-");
                    continue;
                }

                if (key == Keys.OemPeriod)
                {
                    AddToInputText(big ? ":" : ".");
                    continue;
                }
      

                if (key < Keys.A || key > Keys.Z)
                {
                    continue;
                }                

                var keyValue = key.ToString();
                if (!big)
                {
                    keyValue = keyValue.ToLower();
                }
                // sInputbox.Text = sInputbox.Text + keyValue;
                AddToInputText(keyValue);

            }
        }

        private static void AddToInputText(string text)
        {
            var tmp = sInputbox.Text;
            sInputbox.Text = sInputbox.Text.Substring(0, sInputbox.mCursorPos) + text;
            if (tmp.Length > sInputbox.mCursorPos+1 && tmp.Length > 1)
            {
                // sInputbox.Text += tmp.Substring(sInputbox.Text.Length - 1, tmp.Length);
            }
            IncreaseCursor();
        }

        public static void IncreaseCursor()
        {
            if (sInputbox.mCursorPos >= sInputbox.Text.Length)
            {
                return;
            }
            sInputbox.mCursorPos++;
        }

        public static void DecreaseCursor()
        {
            if (sInputbox.mCursorPos == 0)
            {
                return;
            }
            sInputbox.mCursorPos--;
        }

        public static void SetInputboxTarget(Inputbox inputbox)
        {
            sInputbox = inputbox;
        }

        public static Inputbox GetInputboxTarget()
        {
            return sInputbox;
        }

        public static void UnsetInputboxTarget(Inputbox inputbox)
        {
            if (sInputbox != inputbox)
            {
                return;
            }
            sInputbox = null;
        }

        public static bool IsBlocked(Keys key)
        {
            return BlockList.Contains(key);
        }

        public static bool IsBlocked(MouseButtons mb)
        {
            return BlockListMouse.Contains(mb);
        }

        // Returns false if any given key was previously used or is not pressed
        public static bool AllKeysPressed(params Keys[] keys)
        {
            if (keys.Any(k => BlockList.Contains(k)))
            {
                return false;
            }

            return keys.All(k =>
            {
                if (!KeyState.IsKeyDown(k) || PreviousKeyState.IsKeyDown(k))
                {
                    return false;
                }

                BlockList.Add(k);
                return true;

            });
        }

        // Returns false if all given keys were previously used or are not pressed
        public static bool AnyKeysPressed(params Keys[] keys)
        {
            if (keys.All(k => BlockList.Contains(k)))
            {
                return false;
            }

            return keys.Any(k =>
            {
                if (!KeyState.IsKeyDown(k) || PreviousKeyState.IsKeyDown(k))
                {
                    return false;
                }

                BlockList.Add(k);
                return true;

            });
        }

        // Returns false if any key was previously used or is not relesed
        public static bool AllKeysReleased(params Keys[] keys)
        {
            if (keys.Any(k => BlockList.Contains(k)))
            {
                return false;
            }

            return keys.All(k =>
            {
                if (!KeyState.IsKeyUp(k) || !PreviousKeyState.IsKeyDown(k))
                {
                    return false;
                }

                BlockList.Add(k);
                return true;
            });
        }

        // Returns false if all keys were previously used or are not relesead
        public static bool AnyKeysReleased(params Keys[] keys)
        {
            if (keys.All(k => BlockList.Contains(k)))
            {
                return false;
            }

            return keys.Any(k =>
            {

                if (!KeyState.IsKeyUp(k) || !PreviousKeyState.IsKeyDown(k))
                {
                    return false;
                }

                BlockList.Add(k);
                return true;
            });
        }

        // Returns false if any key was previously used or is up
        public static bool AllKeysDown(params Keys[] keys)
        {
            if (keys.Any(k => BlockList.Contains(k)))
            {
                return false;
            }

            return keys.All(k =>
            {
                if (!KeyState.IsKeyDown(k))
                {
                    return false;
                }

                BlockList.Add(k);
                return true;

            });

        }

        // Returns false if all keys were previously used or are up
        public static bool AnyKeysDown(params Keys[] keys)
        {
            if (keys.All(k => BlockList.Contains(k)))
            {
                return false;
            }

            return keys.Any(k =>
            {
                if (!KeyState.IsKeyDown(k))
                {
                    return false;
                }

                BlockList.Add(k);
                return true;

            });

        }

        // Returns false if any key was previously used or is down
        public static bool AllKeysUp(params Keys[] keys)
        {
            if (keys.Any(k => BlockList.Contains(k)))
            {
                return false;
            }

            return keys.All(k =>
            {
                if (!KeyState.IsKeyUp(k))
                {
                    return false;
                }

                BlockList.Add(k);
                return true;

            });

        }

        // Returns false if all keys were previously used or are down
        public static bool AnyKeysUp(params Keys[] keys)
        {
            if (keys.All(k => BlockList.Contains(k)))
            {
                return false;
            }

            return keys.Any(k =>
            {
                if (!KeyState.IsKeyUp(k))
                {
                    return true;
                }

                BlockList.Add(k);
                return false;

            });

        }

        public static bool MouseRightButtonPressed()
        {
            if (BlockListMouse.Contains(MouseButtons.Right))
            {
                return false;
            }

            if (MouseState.RightButton != ButtonState.Pressed || PreviousMouseState.RightButton == ButtonState.Pressed)
            {
                return false;
            }

            BlockListMouse.Add(MouseButtons.Right);
            return true;
        }

        public static bool MouseRightButtonReleased()
        {
            if (BlockListMouse.Contains(MouseButtons.Right))
            {
                return false;
            }

            if (MouseState.RightButton != ButtonState.Released || PreviousMouseState.RightButton == ButtonState.Released)
            {
                return false;
            }

            BlockListMouse.Add(MouseButtons.Right);
            return true;
        }

        public static bool MouseRightButtonDown()
        {
            if (BlockListMouse.Contains(MouseButtons.Right))
            {
                return false;
            }

            if (MouseState.RightButton != ButtonState.Pressed && PreviousMouseState.RightButton != ButtonState.Pressed)
            {
                return false;
            }

            BlockListMouse.Add(MouseButtons.Right);
            return true;
        }

        public static bool MouseLeftButtonPressed()
        {
            if (BlockListMouse.Contains(MouseButtons.Left))
            {
                return false;
            }

            if (MouseState.LeftButton != ButtonState.Pressed || PreviousMouseState.LeftButton == ButtonState.Pressed)
            {
                return false;
            }

            BlockListMouse.Add(MouseButtons.Left);
            return true;
        }

        public static bool MouseLeftButtonReleased()
        {
            if (BlockListMouse.Contains(MouseButtons.Left))
            {
                return false;
            }

            if (MouseState.LeftButton != ButtonState.Released || PreviousMouseState.LeftButton == ButtonState.Released)
            {
                return false;
            }

            BlockListMouse.Add(MouseButtons.Left);
            return true;
        }

        public static bool MouseLeftButtonDown()
        {
            if (BlockListMouse.Contains(MouseButtons.Left))
            {
                return false;
            }

            if (MouseState.LeftButton != ButtonState.Pressed && PreviousMouseState.LeftButton != ButtonState.Pressed)
            {
                return false;
            }

            BlockListMouse.Add(MouseButtons.Left);
            return true;
        }

        public static bool AllMouseButtonsDown(params MouseButtons[] mouseButtons)
        {
            foreach (var mouseButton in mouseButtons)
            {
                if (BlockListMouse.Contains(mouseButton))
                {
                    return false;
                }

                if (mouseButton == MouseButtons.Left && MouseState.LeftButton != ButtonState.Pressed && PreviousMouseState.LeftButton != ButtonState.Pressed)
                {
                    return false;
                }
                if (mouseButton == MouseButtons.Middle && MouseState.MiddleButton != ButtonState.Pressed && PreviousMouseState.MiddleButton != ButtonState.Pressed)
                {
                    return false;
                }
                if (mouseButton == MouseButtons.Right && MouseState.RightButton != ButtonState.Pressed && PreviousMouseState.RightButton != ButtonState.Pressed)
                {
                    return false;
                }
            }

            BlockListMouse.AddRange(mouseButtons);
            return true;
        }

        public static bool AllMouseButtonsUp(params MouseButtons[] mouseButtons)
        {
            foreach (var mouseButton in mouseButtons)
            {
                if (BlockListMouse.Contains(mouseButton))
                {
                    return false;
                }

                if (mouseButton == MouseButtons.Left && MouseState.LeftButton != ButtonState.Released && PreviousMouseState.LeftButton != ButtonState.Released)
                {
                    return false;
                }
                if (mouseButton == MouseButtons.Middle && MouseState.MiddleButton != ButtonState.Released && PreviousMouseState.MiddleButton != ButtonState.Released)
                {
                    return false;
                }
                if (mouseButton == MouseButtons.Right && MouseState.RightButton != ButtonState.Released && PreviousMouseState.RightButton != ButtonState.Released)
                {
                    return false;
                }
            }

            BlockListMouse.AddRange(mouseButtons);
            return true;
        }

        public static bool AllMouseButtonsPressed(params MouseButtons[] mouseButtons)
        {
            foreach (var mouseButton in mouseButtons)
            {
                if (BlockListMouse.Contains(mouseButton))
                {
                    return false;
                }

                if (mouseButton == MouseButtons.Left && MouseState.LeftButton != ButtonState.Pressed || PreviousMouseState.LeftButton == ButtonState.Pressed)
                {
                    return false;
                }
                if (mouseButton == MouseButtons.Middle && MouseState.MiddleButton != ButtonState.Pressed || PreviousMouseState.MiddleButton == ButtonState.Pressed)
                {
                    return false;
                }
                if (mouseButton == MouseButtons.Right && MouseState.RightButton != ButtonState.Pressed || PreviousMouseState.RightButton == ButtonState.Pressed)
                {
                    return false;
                }
            }

            BlockListMouse.AddRange(mouseButtons);
            return true;
        }

        public static bool AllMouseButtonsReleased(params MouseButtons[] mouseButtons)
        {
            foreach (var mouseButton in mouseButtons)
            {
                if (BlockListMouse.Contains(mouseButton))
                {
                    return false;
                }

                if (mouseButton == MouseButtons.Left && MouseState.LeftButton != ButtonState.Released || PreviousMouseState.LeftButton == ButtonState.Released)
                {
                    return false;
                }
                if (mouseButton == MouseButtons.Middle && MouseState.MiddleButton != ButtonState.Released || PreviousMouseState.MiddleButton == ButtonState.Released)
                {
                    return false;
                }
                if (mouseButton == MouseButtons.Right && MouseState.RightButton != ButtonState.Released || PreviousMouseState.RightButton == ButtonState.Released)
                {
                    return false;
                }
            }

            BlockListMouse.AddRange(mouseButtons);
            return true;
        }

        public static bool MouseLeftButtonPressedPeak()
        {
            if (BlockListMouse.Contains(MouseButtons.Left))
            {
                return false;
            }

            if (MouseState.LeftButton != ButtonState.Pressed || PreviousMouseState.LeftButton == ButtonState.Pressed)
            {
                return false;
            }
            
            return true;
        }

    }
}
