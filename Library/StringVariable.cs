using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace AntTweakBar
{
    /// <summary>
    /// An AntTweakBar variable which can hold a string.
    /// </summary>
    public class StringVariable : Variable
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
        /// Gets or sets the value of this variable.
        /// </summary>
        public String Value
        {
            get { ThrowIfDisposed(); return value; }
            set
            {
                ThrowIfDisposed();

                if ((value == null) || !Validate(value)) {
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value.");
                } else {
                    this.value = value;
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private String value;

        /// <summary>
        /// Initialization delegate, which creates the string variable.
        /// </summary>
        private static void InitStringVariable(Variable var, String id)
        {
            var it = var as StringVariable;

            Variable.SetCallbacks.Add(id, new Tw.SetVarCallback(it.SetCallback));
            Variable.GetCallbacks.Add(id, new Tw.GetVarCallback(it.GetCallback));

            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        Tw.VariableType.CSString,
                        Variable.SetCallbacks[id],
                        Variable.GetCallbacks[id],
                        IntPtr.Zero, null);
        }

        /// <summary>
        /// Creates a new string variable in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the string variable in.</param>
        /// <param name="initialValue">The initial value of the variable.</param>
        /// <param name="def">An optional definition string for the new variable.</param>
        public StringVariable(Bar bar, String initialValue = "", String def = null)
            : base(bar, InitStringVariable, def)
        {
            if (initialValue == null) {
                throw new ArgumentNullException(initialValue);
            }

            value = initialValue;
        }

        /// <summary>
        /// A validation method for derived classes. Override this to provide custom
        /// variables with arbitrary validation logic. If the user's input does not
        /// pass validation, the variable's value reverts to its previous contents.
        /// </summary>
        /// <remarks>
        /// The validation method is also invoked when setting the variable's value.
        /// </remarks>
        /// <param name="value">The value the variable will contain.</param>
        /// <returns>Whether the variable is allowed to have this value.</returns>
        protected virtual bool Validate(String value)
        {
            return true;
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            var strBytes = new List<byte>();
            var off = 0;
            while (true)
            {
              var ch = Marshal.ReadByte(pointer, off++);
              if (ch == 0) break;
              strBytes.Add(ch);
            }

            string data = Encoding.UTF8.GetString(strBytes.ToArray());

            if (Validate(data)) {
                bool changed = (data != Value);
                value = data;

                if (changed) {
                    OnChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            var bytes = new List<byte>(Encoding.UTF8.GetBytes(Value));
            bytes.Add(0); /* Append the null-terminated character. */
            Marshal.Copy(bytes.ToArray(), 0, pointer, bytes.Count);
        }

        public override String ToString()
        {
            return String.Format("[StringVariable: Label={0}, Value={1}]", Label, Value);
        }
    }
}
