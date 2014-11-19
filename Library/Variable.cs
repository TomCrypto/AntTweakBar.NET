using System;
using System.Linq;

namespace AntTweakBar
{
    /// <summary>
    /// The base class for all AntTweakBar variables.
    /// </summary>
    public abstract class Variable : IDisposable
    {
        /// <summary>
        /// The default label for unnamed variables.
        /// </summary>
        private const String UnnamedLabel = "<unnamed>";

        /// <summary>
        /// Gets this variable's context-dependent unique identifier.
        /// </summary>
        internal String ID { get; private set; }

        /// <summary>
        /// Gets this variable's parent bar.
        /// </summary>
        public Bar ParentBar { get; private set; }

        /// <summary>
        /// Creates a new variable in a given AntTweakBar bar.
        /// </summary>
        /// <param name="parent">The bar the variable should be created in.</param>
        /// <param name="initFunc">A delegate which will initialize the variable.</param>
        /// <param name="def">An optional definition string for the new variable.</param>
        /// <param name="initLabel">Whether to initialize the variable's label.</param>
        protected Variable(Bar parent, Action<Variable, String> initFunc, String def = null, bool initLabel = true)
        {
            if ((ParentBar = parent) == null)
                throw new ArgumentNullException("parent");
            
            Tw.SetCurrentWindow(ParentBar.ParentContext.Identifier);
            initFunc(this, ID = Guid.NewGuid().ToString());
            created = true; /* Variable now created. */
            if (initLabel) Label = UnnamedLabel;
            ParentBar.Add(this);
            SetDefinition(def);
        }

        /// <summary>
        /// Sets this variable's properties from a definition string.
        /// </summary>
        /// <param name="def">An AntTweakBar definition string, excluding the name prefix.</param>
        public void SetDefinition(String def)
        {
            if (def != null)
            {
                Tw.SetCurrentWindow(ParentBar.ParentContext.Identifier);
                Tw.Define(String.Format("{0}/{1} {2}", ParentBar.ID, ID, def));
            }
        }

        #region Customization

        /// <summary>
        /// Gets or sets this variable's label.
        /// </summary>
        public String Label
        {
            get { return Tw.GetStringParam(ParentBar.Pointer, ID, "label"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "label", value); }
        }

        /// <summary>
        /// Gets or sets this variable's help text.
        /// </summary>
        public String Help
        {
            get { return Tw.GetStringParam(ParentBar.Pointer, ID, "help"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "help", value); }
        }

        /// <summary>
        /// Gets or sets this variable's group.
        /// </summary>
        public String Group
        {
            get { return Tw.GetStringParam(ParentBar.Pointer, ID, "group"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "group", value); }
        }

        /// <summary>
        /// Gets or sets whether this variable is visible.
        /// </summary>
        public Boolean Visible
        {
            get { return Tw.GetBooleanParam(ParentBar.Pointer, ID, "visible"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "visible", value); }
        }

        /// <summary>
        /// Gets or sets whether this variable is read-only.
        /// </summary>
        public Boolean ReadOnly
        {
            get { return Tw.GetBooleanParam(ParentBar.Pointer, ID, "readonly"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "readonly", value); }
        }

        /// <summary>
        /// Gets or sets the key shortcut for this variable.
        /// </summary>
        public String KeyShortcut
        {
            get { return Tw.GetStringParam(ParentBar.Pointer, ID, "key"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "key", value); }
        }

        /// <summary>
        /// Gets or sets the increment key shortcut for this variable.
        /// </summary>
        public String KeyIncrementShortcut
        {
            get { return Tw.GetStringParam(ParentBar.Pointer, ID, "keyincr"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "keyincr", value); }
        }

        /// <summary>
        /// Gets or sets the decrement key shortcut for this variable.
        /// </summary>
        public String KeyDecrementShortcut
        {
            get { return Tw.GetStringParam(ParentBar.Pointer, ID, "keydecr"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "keydecr", value); }
        }

        #endregion

        #region IDisposable

        ~Variable()
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
            if (!disposed && (ParentBar != null))
            {
                if (disposing && ParentBar.Contains(this)) {
                    ParentBar.Remove(this);
                }

                if (created) {
                    Tw.RemoveVar(ParentBar.Pointer, ID);
                }

                disposed = true;
            }
        }

        private bool disposed = false;
        private bool created = false;

        #endregion

        public override String ToString()
        {
            return String.Format("[Variable: Label={0}]", Label);
        }
    }
}
