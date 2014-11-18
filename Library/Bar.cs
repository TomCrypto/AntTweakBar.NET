using System;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

namespace AntTweakBar
{
    /// <summary>
    /// Represents a configuration bar.
    /// </summary>
    public class Bar : IDisposable, IEnumerable<Variable>
    {
        private readonly IntPtr pointer;
        private readonly Context owner;
        private readonly String id;

        /// <summary>
        /// Gets the identifier of this bar.
        /// </summary>
        internal String ID { get { return id; } }

        /// <summary>
        /// Gets the unmanaged pointer of this bar.
        /// </summary>
        internal IntPtr Pointer { get { return pointer; } }

        /// <summary>
        /// Used implicitly to obtain access to the bar pointer.
        /// </summary>
        /// <remarks>
        /// Used to simplify internal code. Do not use!
        /// </remarks>
        public static implicit operator IntPtr(Bar bar) { return bar.Pointer; }

        /// <summary>
        /// Gets the context which owns this bar.
        /// </summary>
        internal Context Owner { get { return owner; } }

        /// <summary>
        /// Creates a new bar associated with a given context.
        /// </summary>
        public Bar(Context context, String def = null)
        {
            if ((this.owner = context) == null)
                throw new ArgumentNullException("context");
            else
            {
                TW.SetCurrentWindow(context.Identifier);
                id = Guid.NewGuid().ToString();
                pointer = TW.NewBar(id);

                Owner.Add(this);
                Label = "undef";

                if (def != null)
                    SetDefinition(def);
            }
        }

        #region Customization

        /// <summary>
        /// Configures the bar from a definition string.
        /// </summary>
        public void SetDefinition(String def)
        {
            if (def == null)
                throw new ArgumentNullException("def");
            else
            {
                TW.SetCurrentWindow(Owner.Identifier);
                TW.Define(String.Format("{0} {1}", ID, def));
            }
        }

        /// <summary>
        /// Shows or hides a group in this bar.
        /// </summary>
        public void ShowGroup(String name, Boolean visible)
        {
            TW.SetCurrentWindow(Owner.Identifier);
            TW.Define(String.Format("{0}/{1} visible={2}", id, name, visible ? "true" : "false"));
        }

        /// <summary>
        /// Expands or collapses a group in this bar.
        /// </summary>
        public void ExpandGroup(String name, Boolean expand)
        {
            TW.SetCurrentWindow(Owner.Identifier);
            TW.Define(String.Format("{0}/{1} opened={2}", id, name, expand ? "true" : "false"));
        }

        /// <summary>
        /// Gets or sets the bar's label.
        /// </summary>
        public String Label
        {
            get { return TW.GetStringParam(pointer, null, "label"); }
            set { TW.SetParam(pointer, null, "label", value); }
        }

        /// <summary>
        /// Gets or sets the bar's help text.
        /// </summary>
        public String Help
        {
            get { return TW.GetStringParam(pointer, null, "help"); }
            set { TW.SetParam(pointer, null, "help", value); }
        }

        /// <summary>
        /// Gets or sets the bar's color.
        /// </summary>
        public Color Color
        {
            get { return TW.GetColorParam(pointer, null, "color"); }
            set { TW.SetParam(pointer, null, "color", value); }
        }

        /// <summary>
        /// Gets or sets the bar's alpha value (opacity).
        /// </summary>
        public Single Alpha
        {
            get { return TW.GetSingleParam(pointer, null, "alpha")[0] / 255.0f; }
            set
            {
                if ((0 <= value) && (value <= 1))
                    TW.SetParam(pointer, null, "alpha", (int)(value * 255.0f));
                else
                    throw new ArgumentOutOfRangeException("value", "Alpha must be in the interval [0, 1]");
            }
        }

        /// <summary>
        /// Gets or sets the bar's position.
        /// </summary>
        public Point Position
        {
            get { return TW.GetPointParam(pointer, null, "position"); }
            set { TW.SetParam(pointer, null, "position", value); }
        }

        /// <summary>
        /// Gets or sets the bar's size.
        /// </summary>
        public Size Size
        {
            get { return TW.GetSizeParam(pointer, null, "size"); }
            set { TW.SetParam(pointer, null, "size", value); }
        }

        /// <summary>
        /// Gets or sets whether the bar can be iconified by the user.
        /// </summary>
        public Boolean Iconifiable
        {
            get { return TW.GetBooleanParam(pointer, null, "iconifiable"); }
            set { TW.SetParam(pointer, null, "iconifiable", value); }
        }

        /// <summary>
        /// Gets or sets whether the bar can be moved by the user.
        /// </summary>
        public Boolean Movable
        {
            get { return TW.GetBooleanParam(pointer, null, "movable"); }
            set { TW.SetParam(pointer, null, "movable", value); }
        }

        /// <summary>
        /// Gets or sets whether the bar can be resized by the user.
        /// </summary>
        public Boolean Resizable
        {
            get { return TW.GetBooleanParam(pointer, null, "resizable"); }
            set { TW.SetParam(pointer, null, "resizable", value); }
        }

        /// <summary>
        /// Gets or sets whether the bar is constrained to the window.
        /// </summary>
        public Boolean Contained
        {
            get { return TW.GetBooleanParam(pointer, null, "contained"); }
            set { TW.SetParam(pointer, null, "contained", value); }
        }

        /// <summary>
        /// Gets or sets whether the bar is visible.
        /// </summary>
        public Boolean Visible
        {
            get { return TW.GetBooleanParam(pointer, null, "visible"); }
            set { TW.SetParam(pointer, null, "visible", value); }
        }

        /// <summary>
        /// Brings this bar in front of all others.
        /// </summary>
        public void BringToFront()
        {
            TW.SetTopBar(Pointer);
        }

        /// <summary>
        /// Sends this bar behind all others.
        /// </summary>
        public void SendToBack(Bar bar)
        {
            TW.SetBottomBar(Pointer);
        }

        #endregion

        #region IEnumerable

        private readonly ICollection<Variable> variables = new HashSet<Variable>();

        internal void Add(Variable variable)
        {
            variables.Add(variable);
        }

        internal void Remove(Variable variable)
        {
            variables.Remove(variable);
        }

        public IEnumerator<Variable> GetEnumerator()
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                while (variables.Any())
                    variables.First().Dispose();
            }

            if (Owner.Contains(this))
            {
                TW.SetCurrentWindow(Owner.Identifier);
                TW.DeleteBar(pointer);
                Owner.Remove(this);
            }

            disposed = true;
        }

        private bool disposed = false;

        #endregion
    }
}
