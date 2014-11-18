using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AntTweakBar
{
    /// <summary>
    /// An AntTweakBar context, which logically maps to a unique window.
    /// </summary>
    public class Context : IEnumerable<Bar>, IDisposable
    {
        private static object lk = new object();
        private static Int32 contextCounter = 0;

        /// <summary>
        /// Gets this context's unique identifier, used to switch between contexts.
        /// </summary>
        internal Int32 Identifier { get; private set; }

        /// <summary>
        /// Creates a new AntTweakBar context. If there are no other active contexts, a graphics API to use must be provided.
        /// </summary>
        /// <param name="graphicsAPI">The graphics API you need to share with AntTweakBar.</param>
        /// <param name="device">For Direct3D interop only, a pointer to the D3D device.</param>
        public Context(TW.GraphicsAPI graphicsAPI = TW.GraphicsAPI.Unspecified, IntPtr device = default(IntPtr))
        {
            lock (lk)
            {
                if (graphicsAPI == TW.GraphicsAPI.D3D9 || graphicsAPI == TW.GraphicsAPI.D3D10 || graphicsAPI == TW.GraphicsAPI.D3D11) {
                    if (device == IntPtr.Zero) {
                        throw new InvalidOperationException("A valid device pointer is required for Direct3D interop.");
                    }
                }

                if (contextCounter == 0)
                {
                    if (graphicsAPI == TW.GraphicsAPI.Unspecified) {
                        throw new InvalidOperationException("A graphics API must be specified for the initial context.");
                    }

                    TW.Init(graphicsAPI, device);
                }

                Identifier = contextCounter++;
            }
        }

        /// <summary>
        /// Draws every bar in this context.
        /// </summary>
        public void Draw()
        {
            TW.SetCurrentWindow(Identifier);
            TW.Draw(); // Applies to all bars
        }

        #region Event Handlers

        /// <summary>
        /// Notifies this context of a change in window size.
        /// </summary>
        /// <param name="size">The new window size.</param>
        public void HandleResize(Size size)
        {
            TW.SetCurrentWindow(Identifier);
            TW.WindowSize(size.Width, size.Height);
        }

        /// <summary>
        /// Notifies this context of a mouse movement.
        /// </summary>
        /// <param name="point">The new cursor location.</param>
        public bool HandleMouseMove(Point point)
        {
            TW.SetCurrentWindow(Identifier);
            return TW.MouseMotion(point.X, point.Y);
        }

        /// <summary>
        /// Notifies this context of a mouse scroll.
        /// </summary>
        /// <param name="pos">The new mouse wheel position.</param>
        public bool HandleMouseWheel(int pos)
        {
            TW.SetCurrentWindow(Identifier);
            return TW.MouseWheel(pos);
        }

        /// <summary>
        /// Notifies this context of a mouse click.
        /// </summary>
        /// <param name="action">The kind of mouse action.</param>
        /// <param name="button">The mouse button pressed.</param>
        public bool HandleMouseClick(TW.MouseAction action, TW.MouseButton button)
        {
            TW.SetCurrentWindow(Identifier);
            return TW.MouseClick(action, button);
        }

        /// <summary>
        /// Notifies this context of a key press.
        /// </summary>
        /// <param name="key">The key character pressed.</param>
        public bool HandleKeyPress(char key)
        {
            TW.SetCurrentWindow(Identifier);
            return TW.KeyPressed((int)key, TW.KeyModifier.None);
        }

        /// <summary>
        /// Notifies this context of a special key press.
        /// </summary>
        /// <param name="key">The key pressed.</param>
        /// <param name="modifiers">The key modifiers pressed.</param>
        public bool HandleKeyPress(TW.SpecialKey key, TW.KeyModifier modifiers)
        {
            TW.SetCurrentWindow(Identifier);
            return TW.KeyPressed((int)key, modifiers);
        }

        /// <summary>
        /// The SFML event handler.
        /// </summary>
        public bool EventHandlerSFML(IntPtr sfmlEvent, byte majorVersion, byte minorVersion)
        {
            TW.SetCurrentWindow(Identifier);
            return TW.EventSFML(sfmlEvent, majorVersion, minorVersion);
        }

        /// <summary>
        /// The SDL event handler.
        /// </summary>
        public bool EventHandlerSDL(IntPtr sdlEvent, byte majorVersion, byte minorVersion)
        {
            TW.SetCurrentWindow(Identifier);
            return TW.EventSDL(sdlEvent, majorVersion, minorVersion);
        }

        /// <summary>
        /// The Windows event handle (for use in WndProc).
        /// </summary>
        public bool EventHandlerWin(IntPtr wnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            TW.SetCurrentWindow(Identifier);
            return TW.EventWin(wnd, msg, wParam, lParam);
        }

        #endregion

        #region Customization

        /// <summary>
        /// Shows or hides the help bar for this context.
        /// </summary>
        /// <param name="visible">Whether the help bar should be visible.</param>
        public void ShowHelpBar(Boolean visible)
        {
            TW.SetCurrentWindow(Identifier); // one help bar per context
            TW.Define("TW_HELP visible=" + (visible ? "true" : "false"));
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                while (disposing && bars.Any()) {
                    bars.First().Dispose();
                }

                lock (lk)
                {
                    if (--contextCounter == 0) {
                        TW.Terminate();
                    }
                }

                disposed = true;
            }
        }

        private bool disposed = false;

        #endregion

        #region Misc.

        public override String ToString()
        {
            return String.Format("[Context: {0} bars]", bars.Count);
        }

        #endregion
    }
}
