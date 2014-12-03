using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    public abstract class StructVariable<T> : IValueVariable
    {
        /// <summary>
        /// Occurs when the user changes this variable's value.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        public void OnChanged(EventArgs e)
        {
            ThrowIfDisposed();

            if (Changed != null) {
                Changed(this, e);
            }
        }

        /// <summary>
        /// Gets this variable's parent bar.
        /// </summary>
        public Bar ParentBar { get { return parentBar; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Bar parentBar;

        /// <summary>
        /// Gets or sets the value of this variable.
        /// </summary>
        public abstract T Value { get; set; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected IList<IValueVariable> variables;

        public StructVariable(Bar bar, params IValueVariable[] variables)
        {
            if (bar == null) {
                throw new ArgumentNullException("bar");
            } else if (variables == null) {
                throw new ArgumentNullException("variables");
            } else if (variables.Length == 0) {
                throw new ArgumentException("At least one variable.");
            } else if (variables.Any((var) => (var.ParentBar != bar))) {
                throw new ArgumentException("All variables must be in the same bar.");
            }

            this.variables = new List<IValueVariable>(variables);
            this.parentBar = bar;

            foreach (var variable in this.variables) {
                variable.Changed += (s, e) => {
                    OnChanged(e);
                };
            }
        }

        public Group Group
        {
            get { return (variables.First() as Variable).Group; }
            set
            {
                foreach (var variable in variables) {
                    (variable as Variable).Group = value;
                }
            }
        }

        #region IDisposable

        ~StructVariable()
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
                if (disposing) {
                    foreach (var variable in variables) {
                        variable.Dispose();
                    }
                }

                disposed = true;
            }
        }

        private bool disposed = false;

        /// <summary>
        /// Throws an ObjectDisposedException if this variable has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (disposed) {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        #endregion

        public override String ToString()
        {
            return String.Format("[StructVariable<{0}>: Label={1}, Value={2}]", typeof(T).Name, null /* TODO */, Value);
        }
    }
}
