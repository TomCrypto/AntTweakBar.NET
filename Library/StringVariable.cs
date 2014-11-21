using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AntTweakBar
{
    public sealed class StringValidationEventArgs : EventArgs
    {
        /// <summary>
        /// Whether to accept this string value.
        /// </summary>
        public bool Valid { get; set; }
        public readonly String Value;

        public StringValidationEventArgs(String value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// An AntTweakBar variable which can hold a string.
    /// </summary>
    public sealed class StringVariable : Variable
    {
        /// <summary>
        /// Occurs when the user changes this variable's value.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Occurs when the new value of this variable is validated.
        /// </summary>
        public event EventHandler<StringValidationEventArgs> Validating;

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
            set { ValidateAndSet(value); }
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
            Validating += (s, e) => { e.Valid = (e.Value != null); };

            ValidateAndSet(initialValue);
        }

        /// <summary>
        /// Checks if this variable can hold this value.
        /// </summary>
        private bool IsValid(String value)
        {
            ThrowIfDisposed();

            return !Validating.GetInvocationList().Select(h => {
                var check = new StringValidationEventArgs(value);
                h.DynamicInvoke(new object[] { this, check });
                return !check.Valid;
            }).Any(failed => failed);
        }

        /// <summary>
        /// Tries to set this variable's value, validating it.
        /// </summary>
        private void ValidateAndSet(String value)
        {
            if (!IsValid(value)) {
                throw new ArgumentException("Invalid variable value.");
            } else {
                this.value = value;
            }
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

            if (IsValid(data))
            {
                bool changed = (data != value);
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
