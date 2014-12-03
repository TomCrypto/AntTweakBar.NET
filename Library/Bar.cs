using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace AntTweakBar
{
    /// <summary>
    /// An AntTweakBar bar, which holds a set of variables.
    /// </summary>
    public sealed class Bar : IEnumerable<IVariable>, IDisposable
    {
        /// <summary>
        /// The default label for unnamed bars.
        /// </summary>
        private const String UnnamedLabel = "<unnamed>";

        /// <summary>
        /// Gets this bar's context-dependent unique identifier.
        /// </summary>
        internal String ID { get { ThrowIfDisposed(); return id; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly String id;

        /// <summary>
        /// Gets this bar's unmanaged AntTweakBar pointer.
        /// </summary>
        internal IntPtr Pointer { get { ThrowIfDisposed(); return pointer; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IntPtr pointer;

        /// <summary>
        /// Gets this bar's parent context.
        /// </summary>
        public Context ParentContext { get { ThrowIfDisposed(); return parentContext; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Context parentContext;

        /// <summary>
        /// Creates a new bar in a a given AntTweakBar context.
        /// </summary>
        /// <param name="parent">The context the bar should be created in.</param>
        /// <param name="def">An optional definition string for the new bar.</param>
        public Bar(Context parent, String def = null)
        {
            if ((parentContext = parent) == null) {
                throw new ArgumentNullException("parent");
            }

            Tw.SetCurrentWindow(ParentContext.Identifier); // per context
            pointer = Tw.NewBar(id = Guid.NewGuid().ToString());
            ParentContext.Add(this);
            Label = UnnamedLabel;
            SetDefinition(def);
        }

        /// <summary>
        /// Sets this bar's properties from a definition string.
        /// </summary>
        /// <param name="def">An AntTweakBar definition string, excluding the name prefix.</param>
        public void SetDefinition(String def)
        {
            if (def != null)
            {
                Tw.SetCurrentWindow(ParentContext.Identifier);
                Tw.Define(String.Format("{0} {1}", ID, def));
            }
        }

        #region Customization

        /// <summary>
        /// Gets or sets this bar's label.
        /// </summary>
        public String Label
        {
            get { return Tw.GetStringParam(Pointer, null, "label"); }
            set { Tw.SetParam(Pointer, null, "label", value); }
        }

        /// <summary>
        /// Gets or sets this bar's help text.
        /// </summary>
        public String Help
        {
            get { return Tw.GetStringParam(Pointer, null, "help"); }
            set { Tw.SetParam(Pointer, null, "help", value); }
        }

        /// <summary>
        /// Gets or sets this bar's color.
        /// </summary>
        public Color Color
        {
            get { return Tw.GetColorParam(Pointer, null, "color"); }
            set { Tw.SetParam(Pointer, null, "color", value); }
        }

        /// <summary>
        /// Gets or sets this bar's alpha value (opacity).
        /// </summary>
        public byte Alpha
        {
            get { return (byte)Tw.GetIntParam(Pointer, null, "alpha")[0]; }
            set { Tw.SetParam(Pointer, null, "alpha", value); }
        }

        /// <summary>
        /// Gets or sets this bar's position.
        /// </summary>
        public Point Position
        {
            get { return Tw.GetPointParam(Pointer, null, "position"); }
            set { Tw.SetParam(Pointer, null, "position", value); }
        }

        /// <summary>
        /// Gets or sets this bar's size.
        /// </summary>
        public Size Size
        {
            get { return Tw.GetSizeParam(Pointer, null, "size"); }
            set { Tw.SetParam(Pointer, null, "size", value); }
        }

        /// <summary>
        /// Gets or sets whether this bar can be iconified by the user.
        /// </summary>
        public Boolean Iconifiable
        {
            get { return Tw.GetBooleanParam(Pointer, null, "iconifiable"); }
            set { Tw.SetParam(Pointer, null, "iconifiable", value); }
        }

        /// <summary>
        /// Gets or sets whether this bar can be moved by the user.
        /// </summary>
        public Boolean Movable
        {
            get { return Tw.GetBooleanParam(Pointer, null, "movable"); }
            set { Tw.SetParam(Pointer, null, "movable", value); }
        }

        /// <summary>
        /// Gets or sets whether this bar can be resized by the user.
        /// </summary>
        public Boolean Resizable
        {
            get { return Tw.GetBooleanParam(Pointer, null, "resizable"); }
            set { Tw.SetParam(Pointer, null, "resizable", value); }
        }

        /// <summary>
        /// Gets or sets whether this bar is constrained to the window.
        /// </summary>
        public Boolean Contained
        {
            get { return Tw.GetBooleanParam(Pointer, null, "contained"); }
            set { Tw.SetParam(Pointer, null, "contained", value); }
        }

        /// <summary>
        /// Gets or sets whether this bar is visible.
        /// </summary>
        public Boolean Visible
        {
            get { return Tw.GetBooleanParam(Pointer, null, "visible"); }
            set { Tw.SetParam(Pointer, null, "visible", value); }
        }

        /// <summary>
        /// Brings this bar in front of all others.
        /// </summary>
        public void BringToFront()
        {
            Tw.SetTopBar(Pointer);
        }

        /// <summary>
        /// Sends this bar behind all others.
        /// </summary>
        public void SendToBack()
        {
            Tw.SetBottomBar(Pointer);
        }

        #endregion

        #region IEnumerable

        private readonly ICollection<IVariable> variables = new HashSet<IVariable>();

        internal void Add(IVariable variable)
        {
            variables.Add(variable);
        }

        internal void Remove(IVariable variable)
        {
            variables.Remove(variable);
        }

        /// <summary>
        /// Removes all variables in this bar.
        /// </summary>
        public void Clear()
        {
            ThrowIfDisposed();

            while (variables.Any()) {
                variables.First().Dispose();
            }
        }

        public IEnumerator<IVariable> GetEnumerator()
        {
            return variables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IDisposable

        ~Bar()
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
            if (!disposed && (ParentContext != null))
            {
                while (disposing && variables.Any()) {
                    variables.First().Dispose();
                }

                if (disposing && ParentContext.Contains(this)) {
                    ParentContext.Remove(this);
                }

                if (Pointer != IntPtr.Zero) {
                    Tw.DeleteBar(Pointer);
                }

                disposed = true;
            }
        }

        private bool disposed = false;

        /// <summary>
        /// Throws an ObjectDisposedException if this bar has been disposed.
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
            return String.Format("[Bar: {0} variable(s), Label={1}]", variables.Count, Label);
        }
    }
}
