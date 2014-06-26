using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    /// <summary>
    /// Represents an abstract bar variable.
    /// </summary>
    /// <remarks>
    /// Note: derived classes _must_ create an AntTweakBar variable
    /// using the generated identifier for this base class to work.
    /// </remarks>
    public abstract class Variable : IDisposable
    {
        private readonly String id;
        private readonly Bar owner;

        /// <summary>
        /// Gets the owner of this variable.
        /// </summary>
        internal Bar Owner { get { return owner; } }

        /// <summary>
        /// Gets the identifier of this variable.
        /// </summary>
        internal String ID { get { return id; } }

        protected Variable(Bar bar)
        {
            if ((this.owner = bar) == null)
                throw new ArgumentNullException("bar");
            else
                this.id = Guid.NewGuid().ToString();
        }

        #region Customization

        /// <summary>
        /// Configures the variable from a definition string.
        /// </summary>
        public void SetDefinition(String def)
        {
            if (def == null)
                throw new ArgumentNullException("def");
            else
            {
                TW.SetCurrentWindow(Owner.Owner.WindowIndex); // ??
                TW.Define(String.Format("{0}/{1} {2}", Owner.ID, ID, def));
            }
        }

        /// <summary>
        /// Gets or sets the label of this variable.
        /// </summary>
        public String Label
        {
            get { return TW.GetStringParam(Owner, ID, "label"); }
            set { TW.SetParam(Owner, ID, "label", value); }
        }

        /// <summary>
        /// Gets or sets the help text of this variable.
        /// </summary>
        public String Help
        {
            get { return TW.GetStringParam(Owner, ID, "help"); }
            set { TW.SetParam(Owner, ID, "help", value); }
        }

        /// <summary>
        /// Gets or sets the group this variable is in.
        /// </summary>
        public String Group
        {
            get { return TW.GetStringParam(Owner, ID, "group"); }
            set { TW.SetParam(Owner, ID, "group", value); }
        }

        /// <summary>
        /// Gets or sets whether this variable is visible.
        /// </summary>
        public Boolean Visible
        {
            get { return TW.GetBooleanParam(Owner, ID, "visible"); }
            set { TW.SetParam(Owner, ID, "visible", value); }
        }

        /// <summary>
        /// Gets or sets whether this variable is read-only.
        /// </summary>
        public Boolean ReadOnly
        {
            get { return TW.GetBooleanParam(Owner, ID, "readonly"); }
            set { TW.SetParam(Owner, ID, "readonly", value); }
        }

        /// <summary>
        /// Gets or sets the key shortcut for this variable.
        /// </summary>
        public String KeyShortcut
        {
            get { return TW.GetStringParam(Owner, ID, "key"); }
            set { TW.SetParam(Owner, ID, "key", value); }
        }

        /// <summary>
        /// Gets or sets the increment key shortcut for this variable.
        /// </summary>
        public String KeyIncrementShortcut
        {
            get { return TW.GetStringParam(Owner, ID, "keyincr"); }
            set { TW.SetParam(Owner, ID, "keyincr", value); }
        }

        /// <summary>
        /// Gets or sets the decrement key shortcut for this variable.
        /// </summary>
        public String KeyDecrementShortcut
        {
            get { return TW.GetStringParam(Owner, ID, "keydecr"); }
            set { TW.SetParam(Owner, ID, "keydecr", value); }
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
            if (disposed)
                return;

            if (Owner.Contains(this))
            {
                TW.RemoveVar(Owner, ID);
                Owner.Remove(this);
            }

            disposed = true;
        }

        private bool disposed = false;

        #endregion
    }
}
