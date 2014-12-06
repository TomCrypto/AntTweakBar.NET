using System;
using System.Collections.Generic;

namespace AntTweakBar
{
    /// <summary>
    /// A low-level wrapper to the AntTweakBar API.
    /// </summary>
    public static partial class Tw
    {
        /// <summary>
        /// AntTweakBar input mappings for some graphics frameworks.
        /// </summary>
        public static class Mappings
        {
            /// <summary>
            /// OpenTK to AntTweakBar input mappings.
            /// </summary>
            public static class OpenTK
            {
                /// <summary>
                /// Input mapping for mouse buttons.
                /// </summary>
                public static IDictionary<Int32, Tw.MouseButton> Buttons = new Dictionary<Int32, Tw.MouseButton>()
                {
                    { 0, Tw.MouseButton.Left },
                    { 1, Tw.MouseButton.Middle },
                    { 2, Tw.MouseButton.Right },
                };

                /// <summary>
                /// Input mapping for key modifiers.
                /// </summary>
                public static IDictionary<Int32, Tw.KeyModifiers> Modifiers = new Dictionary<Int32, KeyModifiers>()
                {
                    { 1 << 0, Tw.KeyModifiers.Alt },
                    { 1 << 1, Tw.KeyModifiers.Ctrl },
                    { 1 << 2, Tw.KeyModifiers.Shift },
                };

                /// <summary>
                /// Input mapping for keyboard keys.
                /// </summary>
                public static IDictionary<Int32, Tw.Key> Keys = new Dictionary<Int32, Tw.Key>()
                {
                    { 0, Tw.Key.None },
                    { 53, Tw.Key.Backspace },
                    { 52, Tw.Key.Tab },
                    { 65, Tw.Key.Clear },
                    { 49, Tw.Key.Return },
                    { 63, Tw.Key.Pause },
                    { 50, Tw.Key.Escape },
                    { 51, Tw.Key.Space },
                    { 55, Tw.Key.Delete },
                    { 45, Tw.Key.Up },
                    { 46, Tw.Key.Down },
                    { 48, Tw.Key.Right },
                    { 47, Tw.Key.Left },
                    { 54, Tw.Key.Insert },
                    { 58, Tw.Key.Home },
                    { 59, Tw.Key.End },
                    { 56, Tw.Key.PageUp },
                    { 57, Tw.Key.PageDown },
                    { 10, Tw.Key.F1 },
                    { 11, Tw.Key.F2 },
                    { 12, Tw.Key.F3 },
                    { 13, Tw.Key.F4 },
                    { 14, Tw.Key.F5 },
                    { 15, Tw.Key.F6 },
                    { 16, Tw.Key.F7 },
                    { 17, Tw.Key.F8 },
                    { 18, Tw.Key.F9 },
                    { 19, Tw.Key.F10 },
                    { 20, Tw.Key.F11 },
                    { 21, Tw.Key.F12 },
                    { 22, Tw.Key.F13 },
                    { 23, Tw.Key.F14 },
                    { 24, Tw.Key.F15 },
                    { 83, Tw.Key.A },
                    { 84, Tw.Key.B },
                    { 85, Tw.Key.C },
                    { 86, Tw.Key.D },
                    { 87, Tw.Key.E },
                    { 88, Tw.Key.F },
                    { 89, Tw.Key.G },
                    { 90, Tw.Key.H },
                    { 91, Tw.Key.I },
                    { 92, Tw.Key.J },
                    { 93, Tw.Key.K },
                    { 94, Tw.Key.L },
                    { 95, Tw.Key.M },
                    { 96, Tw.Key.N },
                    { 97, Tw.Key.O },
                    { 98, Tw.Key.P },
                    { 99, Tw.Key.Q },
                    { 100, Tw.Key.R },
                    { 101, Tw.Key.S },
                    { 102, Tw.Key.T },
                    { 103, Tw.Key.U },
                    { 104, Tw.Key.V },
                    { 105, Tw.Key.W },
                    { 106, Tw.Key.X },
                    { 107, Tw.Key.Y },
                    { 108, Tw.Key.Z },
                };
            }
        }
    }
}

