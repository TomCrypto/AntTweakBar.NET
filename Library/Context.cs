using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace AntTweakBar
{
    /// <summary>
    /// An AntTweakBar context, which logically maps to a unique window.
    /// </summary>
    public sealed class Context : IEnumerable<Bar>, IDisposable
    {
        private static object lk = new object();
        private static Int32 contextCounter = 0;

        /// <summary>
        /// Gets this context's unique identifier, used to switch between contexts.
        /// </summary>
        internal Int32 Identifier { get { ThrowIfDisposed(); return identifier; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Int32 identifier;

        /// <summary>
        /// Gets this context's last set window size. This is used for release/reset.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Size size;

        /// <summary>
        /// Checks whether a graphics API requires a device pointer.
        /// </summary>
        private static bool RequiresDevicePointer(Tw.GraphicsAPI api)
        {
            return ((api == Tw.GraphicsAPI.D3D9)
                 || (api == Tw.GraphicsAPI.D3D10)
                 || (api == Tw.GraphicsAPI.D3D11));
        }

        /// <summary>
        /// Creates a new AntTweakBar context. If there are no other active contexts, a graphics API to use must be provided.
        /// </summary>
        /// <param name="graphicsAPI">The graphics API you need to share with AntTweakBar.</param>
        /// <param name="device">For Direct3D interop only, a pointer to the D3D device.</param>
        public Context(Tw.GraphicsAPI graphicsAPI = Tw.GraphicsAPI.Unspecified, IntPtr device = default(IntPtr))
        {
            if (RequiresDevicePointer(graphicsAPI) && (device == IntPtr.Zero)) {
                throw new ArgumentException("A valid device pointer is required for Direct3D interop.");
            }

            lock (lk)
            {
                if (contextCounter == 0)
                {
                    if (graphicsAPI == Tw.GraphicsAPI.Unspecified) {
                        throw new ArgumentException("A graphics API must be specified for the initial context.");
                    }

                    Tw.Init(graphicsAPI, device);
                }

                identifier = contextCounter++;
            }
        }

        /// <summary>
        /// Draws every bar in this context.
        /// </summary>
        public void Draw()
        {
            Tw.SetCurrentWindow(Identifier);
            Tw.Draw(); // Applies to all bars
        }

        /// <summary>
        /// Releases all graphics resources used by this context. They can be reset with <see cref="AntTweakBar.Context.ResetResources"/>.
        /// </summary>
        /// <remarks>
        /// This does not affect the bars and variables in this context in any way.
        /// </remarks>
        public void ReleaseResources()
        {
            HandleResize(Size.Empty);
        }

        /// <summary>
        /// Resets all graphics resources used by this context.
        /// </summary>
        public void ResetResources()
        {
            HandleResize(size);
        }

        #region Event Handlers

        /// <summary>
        /// Notifies this context of a change in window size.
        /// </summary>
        /// <param name="size">The new window size.</param>
        public void HandleResize(Size size)
        {
            Tw.SetCurrentWindow(Identifier);
            Tw.WindowSize(size.Width, size.Height);

            if (size != Size.Empty) {
                this.size = size;
            }
        }

        /// <summary>
        /// Notifies this context of a mouse movement.
        /// </summary>
        /// <param name="point">The new cursor location.</param>
        public bool HandleMouseMove(Point point)
        {
            Tw.SetCurrentWindow(Identifier);
            return Tw.MouseMotion(point.X, point.Y);
        }

        /// <summary>
        /// Notifies this context of a mouse scroll.
        /// </summary>
        /// <param name="pos">The new mouse wheel position.</param>
        public bool HandleMouseWheel(int pos)
        {
            Tw.SetCurrentWindow(Identifier);
            return Tw.MouseWheel(pos);
        }

        /// <summary>
        /// Notifies this context of a mouse click.
        /// </summary>
        /// <param name="action">The kind of mouse action.</param>
        /// <param name="button">The mouse button pressed.</param>
        public bool HandleMouseClick(Tw.MouseAction action, Tw.MouseButton button)
        {
            Tw.SetCurrentWindow(Identifier);
            return Tw.MouseClick(action, button);
        }

        /* I don't know if this input handling code is correct. It is actually NOT straightforward to
         * convert from conventional KeyDown/KeyPressed/KeyUp events to AntTweakBar key presses. This
         * may or may not be correct and was inspired from TwEventSFML.cpp, if you know how to fix it
         * please issue a pull request or a patch, until then it looks satisfactory. Kind of.
        */

        private Tw.KeyModifiers lastModifiers;
        private bool ignoreKeyPress;

        /// <summary>
        /// All the special keys supported by AntTweakBar.
        /// </summary>
        private static IList<Tw.Key> SpecialKeys = new List<Tw.Key>() {
            Tw.Key.Escape, Tw.Key.Return, Tw.Key.Tab, Tw.Key.Backspace, Tw.Key.PageUp, Tw.Key.PageDown,
            Tw.Key.Up, Tw.Key.Down, Tw.Key.Left, Tw.Key.Right, Tw.Key.End, Tw.Key.Home, Tw.Key.Insert,
            Tw.Key.Delete, Tw.Key.Space, Tw.Key.F1, Tw.Key.F2, Tw.Key.F3, Tw.Key.F4, Tw.Key.F5,
            Tw.Key.F6, Tw.Key.F7, Tw.Key.F8, Tw.Key.F9, Tw.Key.F10, Tw.Key.F11, Tw.Key.F12,
            Tw.Key.F13, Tw.Key.F14, Tw.Key.F15,
        };

        /// <summary>
        /// Notifies this context that a character has been typed.
        /// </summary>
        /// <param name="character">The character typed.</param>
        public bool HandleKeyPress(char character)
        {
            Tw.SetCurrentWindow(Identifier);

            if (!ignoreKeyPress) {
                if (((character & 0xFF) < 32) && (character != 0) && ((character & 0xFF00) == 0)) {
                    return Tw.KeyPressed((char)((character & 0xFF) + 'a' - 1), Tw.KeyModifiers.Ctrl | lastModifiers);
                } else { // this is supposed to handle the Ctrl+letter combination properly (somehow)
                    return Tw.KeyPressed((char)(character & 0xFF), Tw.KeyModifiers.None);
                }
            } else {
                ignoreKeyPress = false;
            }

            return false;
        }

        /// <summary>
        /// Notifies this context that a key has been pressed.
        /// </summary>
        /// <param name="key">The key pressed.</param>
        /// <param name="modifiers">The key modifiers.</param>
        public bool HandleKeyDown(Tw.Key key, Tw.KeyModifiers modifiers)
        {
            Tw.SetCurrentWindow(Identifier);
            lastModifiers = modifiers;

            if (SpecialKeys.Contains(key)) {
                ignoreKeyPress = true; // special keys
                return Tw.KeyPressed(key, modifiers);
            }

            if (modifiers.HasFlag(Tw.KeyModifiers.Alt)) {
                if ((key >= Tw.Key.A) && (key <= Tw.Key.Z)) {
                    ignoreKeyPress = true; // normal key shortcuts?
                    if (modifiers.HasFlag(Tw.KeyModifiers.Shift)) {
                        key = (Tw.Key)('A' + key - Tw.Key.A);
                        return Tw.KeyPressed(key, modifiers);
                    } else {
                        key = (Tw.Key)('a' + key - Tw.Key.A);
                        return Tw.KeyPressed(key, modifiers);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Notifies this context that a key has been released.
        /// </summary>
        /// <param name="key">The key released.</param>
        /// <param name="modifiers">The key modifiers.</param>
        public bool HandleKeyUp(Tw.Key key, Tw.KeyModifiers modifiers)
        {
            lastModifiers = Tw.KeyModifiers.None;
            ignoreKeyPress = false;
            return false;
        }

        /// <summary>
        /// The SFML event handler.
        /// </summary>
        public bool EventHandlerSFML(IntPtr sfmlEvent, byte majorVersion, byte minorVersion)
        {
            Tw.SetCurrentWindow(Identifier);
            return Tw.EventSFML(sfmlEvent, majorVersion, minorVersion);
        }

        /// <summary>
        /// The SDL event handler.
        /// </summary>
        public bool EventHandlerSDL(IntPtr sdlEvent, byte majorVersion, byte minorVersion)
        {
            Tw.SetCurrentWindow(Identifier);
            return Tw.EventSDL(sdlEvent, majorVersion, minorVersion);
        }

        /// <summary>
        /// The Windows event handle (for use in WndProc).
        /// </summary>
        public bool EventHandlerWin(IntPtr wnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            Tw.SetCurrentWindow(Identifier);
            return Tw.EventWin(wnd, msg, wParam, lParam);
        }

        #endregion

        #region Customization

        /// <summary>
        /// Shows or hides the help bar for this context.
        /// </summary>
        /// <param name="visible">Whether the help bar should be visible.</param>
        public void ShowHelpBar(Boolean visible)
        {
            Tw.SetCurrentWindow(Identifier); // one help bar per context
            Tw.Define("TW_HELP visible=" + (visible ? "true" : "false"));
        }

        /// <summary>
        /// Sets whether to draw or clip overlapping bars.
        /// </summary>
        /// <param name="overlap">If false, clips the contents of overlapping region (default).</param>
        public void SetOverlap(Boolean overlap)
        {
            Tw.SetCurrentWindow(Identifier); // applies to this context
            Tw.Define("GLOBAL overlap=" + (overlap ? "true" : "false"));
        }

        #endregion

        #region IEnumerable

        private readonly ICollection<Bar> bars = new HashSet<Bar>();

        internal void Add(Bar bar)
        {
            bars.Add(bar);
        }

        internal void Remove(Bar bar)
        {
            bars.Remove(bar);
        }

        /// <summary>
        /// Removes all bars in this context.
        /// </summary>
        public void Clear()
        {
            ThrowIfDisposed();

            while (bars.Any()) {
                bars.First().Dispose();
            }
        }

        public IEnumerator<Bar> GetEnumerator()
        {
            return bars.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IDisposable

        ~Context()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                while (disposing && bars.Any()) {
                    bars.First().Dispose();
                }

                ReleaseResources();

                lock (lk)
                {
                    if (--contextCounter == 0) {
                        Tw.Terminate();
                    }
                }

                disposed = true;
            }
        }

        private bool disposed = false;

        /// <summary>
        /// Throws an ObjectDisposedException if this context has been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (disposed) {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        #endregion

        public override String ToString()
        {
            return String.Format("[Context: {0} bar(s)]", bars.Count);
        }
    }
}
