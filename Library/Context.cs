using System;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace AntTweakBar
{
    /// <summary>
    /// Represents a window context.
    /// </summary>
    public class Context : IEnumerable<Bar>, IDisposable
    {
        private static Int32 ctxCount = 0;
        private readonly Int32 identifier;

        /// <summary>
        /// Gets this context's unique identifier.
        /// </summary>
        internal Int32 Identifier { get { return identifier; } }

        /// <summary>
        /// Creates a new context to associate a window with.
        /// </summary>
        /// <remarks>
        /// The graphics API and device are used for the first context
        /// you create in order to initialize the AntTweakBar library.
        /// </remarks>
        public Context(TW.GraphicsAPI graphicsAPI = TW.GraphicsAPI.Unspecified, IntPtr device = default(IntPtr))
        {
            if ((graphicsAPI == TW.GraphicsAPI.D3D9 || graphicsAPI == TW.GraphicsAPI.D3D10 || graphicsAPI == TW.GraphicsAPI.D3D11) && device == IntPtr.Zero)
                throw new InvalidOperationException("DirectX interop requires a valid device pointer.");

            if ((identifier = Interlocked.Increment(ref ctxCount) - 1) == 0)
            {
                if (graphicsAPI == TW.GraphicsAPI.Unspecified) {
                    Interlocked.Decrement(ref ctxCount);
                    throw new InvalidOperationException("Must specify graphics API for initial context.");
                }

                if (!TW.Init(graphicsAPI, device)) {
                    Interlocked.Increment(ref ctxCount);
                    throw new AntTweakBarException("Failed to initialize AntTweakBar.");
                }
            }
        }

        #region Customization

        /// <summary>
        /// Shows or hides the help bar.
        /// </summary>
        public void ShowHelpBar(Boolean visible)
        {
            TW.SetCurrentWindow(identifier); // one help bar per context
            TW.Define("TW_HELP visible=" + (visible ? "true" : "false"));
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Draws every bar in this context.
        /// </summary>
        public void Draw()
        {
            TW.SetCurrentWindow(identifier);
            TW.Draw();
        }

        /// <summary>
        /// Passes a window resize event to the context.
        /// </summary>
        public void HandleResize(Size size)
        {
            TW.SetCurrentWindow(identifier);
            TW.WindowSize(size.Width, size.Height);
        }

        /// <summary>
        /// Passes a mouse move event to the context.
        /// </summary>
        public bool HandleMouseMove(Point point)
        {
            TW.SetCurrentWindow(identifier);
            return TW.MouseMotion(point.X, point.Y);
        }

        /// <summary>
        /// Passes a mouse scroll event to the context.
        /// </summary>
        public bool HandleMouseWheel(int pos)
        {
            TW.SetCurrentWindow(identifier);
            return TW.MouseWheel(pos);
        }

        /// <summary>
        /// Passes a mouse click event to the context.
        /// </summary>
        public bool HandleMouseClick(TW.MouseAction action, TW.MouseButton button)
        {
            TW.SetCurrentWindow(identifier);
            return TW.MouseClick(action, button);
        }

        /// <summary>
        /// Passes a key press event to the context.
        /// </summary>
        public bool HandleKeyPress(char key)
        {
            TW.SetCurrentWindow(identifier);
            return TW.KeyPressed((int)key, TW.KeyModifier.None);
        }

        /// <summary>
        /// Passes a special key press event to the context.
        /// </summary>
        public bool HandleKeyPress(TW.SpecialKey key, TW.KeyModifier modifiers)
        {
            TW.SetCurrentWindow(identifier);
            return TW.KeyPressed((int)key, modifiers);
        }

        /// <summary>
        /// Event handler for SFML.
        /// </summary>
        public bool EventHandlerSFML(IntPtr sfmlEvent, byte majorVersion, byte minorVersion)
        {
            TW.SetCurrentWindow(identifier);
            return TW.EventSFML(sfmlEvent, majorVersion, minorVersion);
        }

        /// <summary>
        /// Event handler for SDL.
        /// </summary>
        public bool EventHandlerSDL(IntPtr sdlEvent, byte majorVersion, byte minorVersion)
        {
            TW.SetCurrentWindow(identifier);
            return TW.EventSDL(sdlEvent, majorVersion, minorVersion);
        }

        /// <summary>
        /// Event handler for Windows messages.
        /// </summary>
        public bool EventHandlerWin(IntPtr wnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            TW.SetCurrentWindow(identifier);
            return TW.EventWin(wnd, msg, wParam, lParam);
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
            if (disposed)
                return;

            if (disposing)
            {
                while (bars.Any())
                    bars.First().Dispose();
            }

            if (Interlocked.Decrement(ref ctxCount) == 0) {
                TW.Terminate();
            }

            disposed = true;
        }

        private bool disposed = false;

        #endregion
    }
}
