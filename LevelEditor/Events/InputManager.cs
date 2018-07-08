using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
                if (!KeyState.IsKeyDown(k) || !PreviousKeyState.IsKeyDown(k))
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
                if (!KeyState.IsKeyDown(k) || !PreviousKeyState.IsKeyDown(k))
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
    }
}
