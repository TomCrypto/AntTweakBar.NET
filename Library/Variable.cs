using System;
using System.Linq;

namespace AntTweakBar
{
    /// <summary>
    /// The base class for all AntTweakBar variables.
    /// </summary>
    /// <remarks>
    /// See comments at the bottom regarding CS0414.
    /// </remarks>
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
            if ((ParentBar = parent) == null) {
                throw new ArgumentNullException("parent");
            }

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
        /// Gets or sets the key shortcut for this variable (if applicable).
        /// </summary>
        public String KeyShortcut
        {
            get { return Tw.GetStringParam(ParentBar.Pointer, ID, "key"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "key", value); }
        }

        /// <summary>
        /// Gets or sets the increment key shortcut for this variable (if applicable).
        /// </summary>
        public String KeyIncrementShortcut
        {
            get { return Tw.GetStringParam(ParentBar.Pointer, ID, "keyincr"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "keyincr", value); }
        }

        /// <summary>
        /// Gets or sets the decrement key shortcut for this variable (if applicable).
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

        /* The #pragma warning in the derived variable classes are used to hide the warnings about the setCallback and
         * getCallback fields not being used. This is not a cover-up, these fields are used to maintain a reference to
         * the SetCallback and GetCallback methods, which are not called from C# but are from inside AntTweakBar.
         *
         * Without these references, the CLR has no way of knowing that the methods are being used as callbacks, as it
         * assumes that they are no longer needed as soon as the Tw.AddVarCB call which uses them returns, which would
         * cause the application to potentially crash if they get garbage-collected or deleted (being unreachable from
         * the perspective of the CLR).
         *
         * Well, actually, this was the case several revisions ago - I have no idea if this still applies with the new
         * way variables are created but I am leaving the fix since it doesn't cost anything whereas removing it could
         * cause user code to start crashing without any reason. If you know for sure, please send a pull request.
        */
    }
}
