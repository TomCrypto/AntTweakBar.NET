using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AntTweakBar
{
    /// <summary>
    /// A pseudo-variable acting as a container for a structured group of related variables.
    /// </summary>
    /// <typeparam name="T">The type of this variable's value.</typeparam>
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

        /// <summary>
        /// Creates a new struct variable in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the struct variable in.</param>
        /// <param name="members">The set of member variables.</param>
        public StructVariable(Bar bar, params IValueVariable[] members)
        {
            if (bar == null) {
                throw new ArgumentNullException("bar");
            } else if (members == null) {
                throw new ArgumentNullException("members");
            } else if (members.Length == 0) {
                throw new ArgumentException("At least one variable.");
            } else if (members.Any((var) => (var.ParentBar != bar))) {
                throw new ArgumentException("All variables must be in the same bar.");
            }

            variables = new List<IValueVariable>(members);
            parentBar = bar;

            foreach (var variable in variables) {
                variable.Changed += (s, e) => {
                    OnChanged(e);
                };
            }
        }

        /// <summary>
        /// Gets or sets the group this variable represents.
        /// </summary>
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

        public void Dispose()
        {
            Dispose(true);
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
            return String.Format("[StructVariable<{0}>: Group={1}, Value={2}]", typeof(T).Name, Group, Value);
        }
    }
}
