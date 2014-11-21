using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    public sealed class IntValidationEventArgs : EventArgs
    {
        /// <summary>
        /// Whether to accept this integer value.
        /// </summary>
        public bool Valid { get; set; }
        public int Value { get; private set; }

        public IntValidationEventArgs(int value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// An AntTweakBar variable which can hold an integer.
    /// </summary>
    public sealed class IntVariable : Variable
    {
        /// <summary>
        /// Occurs when the user changes this variable's value.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Occurs when the new value of this variable is validated.
        /// </summary>
        public event EventHandler<IntValidationEventArgs> Validating;

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
        public Int32 Value
        {
            get { ThrowIfDisposed(); return value; }
            set { ValidateAndSet(value); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Int32 value;

        /// <summary>
        /// Initialization delegate, which creates the integer variable.
        /// </summary>
        private static void InitIntVariable(Variable var, String id)
        {
            var it = var as IntVariable;

            Variable.SetCallbacks.Add(id, new Tw.SetVarCallback(it.SetCallback));
            Variable.GetCallbacks.Add(id, new Tw.GetVarCallback(it.GetCallback));

            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        Tw.VariableType.Int32,
                        Variable.SetCallbacks[id],
                        Variable.GetCallbacks[id],
                        IntPtr.Zero, null);
        }

        /// <summary>
        /// Creates a new integer variable in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the integer variable in.</param>
        /// <param name="initialValue">The initial value of the variable.</param>
        /// <param name="def">An optional definition string for the new variable.</param>
        public IntVariable(Bar bar, Int32 initialValue = 0, String def = null)
            : base(bar, InitIntVariable, def)
        {
            Validating += (s, e) => { e.Valid = (Min <= e.Value) && (e.Value <= Max); };

            ValidateAndSet(initialValue);
        }

        /// <summary>
        /// Checks if this variable can hold this value.
        /// </summary>
        private bool IsValid(int value)
        {
            ThrowIfDisposed();

            return !Validating.GetInvocationList().Select(h => {
                var check = new IntValidationEventArgs(value);
                h.DynamicInvoke(new object[] { this, check });
                return !check.Valid;
            }).Any(failed => failed);
        }

        /// <summary>
        /// Tries to set this variable's value, validating it.
        /// </summary>
        private void ValidateAndSet(int value)
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
            int data = Marshal.ReadInt32(pointer);

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
            Marshal.WriteInt32(pointer, Value);
        }

        #region Customization

        /// <summary>
        /// Gets or sets this variable's minimum value.
        /// </summary>
        public Int32 Min
        {
            get { return Tw.GetIntParam(ParentBar.Pointer, ID, "min")[0]; }
            set
            {
                Tw.SetParam(ParentBar.Pointer, ID, "min", value);
                Value = Math.Max(value, Value);
            }
        }

        /// <summary>
        /// Gets or sets this variable's maximum value.
        /// </summary>
        public Int32 Max
        {
            get { return Tw.GetIntParam(ParentBar.Pointer, ID, "max")[0]; }
            set
            {
                Tw.SetParam(ParentBar.Pointer, ID, "max", value);
                Value = Math.Min(value, Value);
            }
        }

        /// <summary>
        /// Gets or sets whether this variable should display in hexadecimal.
        /// </summary>
        public Boolean Hexadecimal
        {
            get { return Tw.GetBooleanParam(ParentBar.Pointer, ID, "hexa"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "hexa", value); }
        }

        /// <summary>
        /// Gets or sets this variable's step (increment).
        /// </summary>
        public Int32 Step
        {
            get { return Tw.GetIntParam(ParentBar.Pointer, ID, "step")[0]; }
            set { Tw.SetParam(ParentBar.Pointer, ID, "step", value); }
        }

        #endregion

        public override String ToString()
        {
            return String.Format("[IntVariable: Label={0}, Value={1}]", Label, Value);
        }
    }
}
