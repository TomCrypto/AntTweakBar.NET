using System;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

using AntTweakBar;

namespace Tests
{
    public class TestWindow : GameWindow
    {
        private static bool HandleMouseClick(Context context, MouseButtonEventArgs e)
        {
            var action = e.IsPressed ? Tw.MouseAction.Pressed : Tw.MouseAction.Released;

            switch (e.Button)
            {
                case MouseButton.Left:
                    return context.HandleMouseClick(action, Tw.MouseButton.Left);
                    case MouseButton.Right:
                    return context.HandleMouseClick(action, Tw.MouseButton.Right);
                    case MouseButton.Middle:
                    return context.HandleMouseClick(action, Tw.MouseButton.Middle);
            }

            return false;
        }

        private static bool HandleKeyPress(Context context, KeyboardKeyEventArgs e)
        {
            var modifier = Tw.KeyModifier.None;
            if (e.Modifiers.HasFlag(KeyModifiers.Alt))
                modifier |= Tw.KeyModifier.Alt;
            if (e.Modifiers.HasFlag(KeyModifiers.Shift))
                modifier |= Tw.KeyModifier.Shift;
            if (e.Modifiers.HasFlag(KeyModifiers.Control))
                modifier |= Tw.KeyModifier.Ctrl;

            var mapping = new Dictionary<Key, Tw.SpecialKey>()
            {
                { Key.Back,          Tw.SpecialKey.Backspace },
                { Key.Tab,           Tw.SpecialKey.Tab },
                { Key.Clear,         Tw.SpecialKey.Clear },
                { Key.Enter,         Tw.SpecialKey.Return },
                { Key.Pause,         Tw.SpecialKey.Pause },
                { Key.Escape,        Tw.SpecialKey.Escape },
                //{ Key.Space,         TW.SpecialKey.Space }, // already handled by KeyPress
                { Key.Delete,        Tw.SpecialKey.Delete },
                { Key.Up,            Tw.SpecialKey.Up },
                { Key.Left,          Tw.SpecialKey.Left },
                { Key.Down,          Tw.SpecialKey.Down },
                { Key.Right,         Tw.SpecialKey.Right },
                { Key.Insert,        Tw.SpecialKey.Insert },
                { Key.Home,          Tw.SpecialKey.Home },
                { Key.End,           Tw.SpecialKey.End },
                { Key.PageUp,        Tw.SpecialKey.PageUp },
                { Key.PageDown,      Tw.SpecialKey.PageDown },
                { Key.F1,            Tw.SpecialKey.F1 },
                { Key.F2,            Tw.SpecialKey.F2 },
                { Key.F3,            Tw.SpecialKey.F3 },
                { Key.F4,            Tw.SpecialKey.F4 },
                { Key.F5,            Tw.SpecialKey.F5 },
                { Key.F6,            Tw.SpecialKey.F6 },
                { Key.F7,            Tw.SpecialKey.F7 },
                { Key.F8,            Tw.SpecialKey.F8 },
                { Key.F9,            Tw.SpecialKey.F9 },
                { Key.F10,           Tw.SpecialKey.F10 },
                { Key.F11,           Tw.SpecialKey.F11 },
                { Key.F12,           Tw.SpecialKey.F12 },
                { Key.F13,           Tw.SpecialKey.F13 },
                { Key.F14,           Tw.SpecialKey.F14 },
                { Key.F15,           Tw.SpecialKey.F15 },
            };

            if (mapping.ContainsKey(e.Key))
                return context.HandleKeyPress(mapping[e.Key], modifier);
            else
                return false;
        }

        public Context Context { get { return context; } }

        private Context context;

        public TestWindow(String title, int width, int height) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs ev)
        {
            base.OnLoad(ev);
            context = new Context(Tw.GraphicsAPI.OpenGL);
            KeyDown += (sender, e) => HandleKeyPress(context, e);
            Resize += (sender, e) => context.HandleResize(ClientSize);
            KeyPress += (sender, e) => context.HandleKeyPress(e.KeyChar);
            Mouse.ButtonUp += (sender, e) => HandleMouseClick(context, e);
            Mouse.ButtonDown += (sender, e) => HandleMouseClick(context, e);
            Mouse.Move += (sender, e) => context.HandleMouseMove(e.Position);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Console.WriteLine("DRAWING!");

            MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            context.Draw();

            SwapBuffers();
        }

        protected override void Dispose(bool manual)
        {
            if (context != null) {
                context.Dispose();
            }

            base.Dispose(manual);
        }
    }
}

