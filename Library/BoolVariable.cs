using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    public sealed class BoolValidationEventArgs : EventArgs
    {
        /// <summary>
        /// Whether to accept this boolean value.
        /// </summary>
        public bool Valid { get; set; }
        public bool Value { get; private set; }

        public BoolValidationEventArgs(bool value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// An AntTweakBar variable which can hold a boolean value.
    /// </summary>
    public sealed class BoolVariable : Variable, IValueVariable
    {
        /// <summary>
        /// Occurs when the user changes this variable's value.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Occurs when the new value of this variable is validated.
        /// </summary>
        public event EventHandler<BoolValidationEventArgs> Validating;

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
        public Boolean Value
        {
            get { ThrowIfDisposed(); return value; }
            set { ValidateAndSet(value); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean value;

        /// <summary>
        /// Initialization delegate, which creates the boolean variable.
        /// </summary>
        private static void InitBoolVariable(Variable var, String id)
        {
            var it = var as BoolVariable;

            Variable.SetCallbacks.Add(id, new Tw.SetVarCallback(it.SetCallback));
            Variable.GetCallbacks.Add(id, new Tw.GetVarCallback(it.GetCallback));

            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        Tw.VariableType.Bool8,
                        Variable.SetCallbacks[id],
                        Variable.GetCallbacks[id],
                        IntPtr.Zero, null);
        }

        /// <summary>
        /// Creates a new boolean variable in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the boolean variable in.</param>
        /// <param name="initialValue">The initial value of the variable.</param>
        /// <param name="def">An optional definition string for the new variable.</param>
        public BoolVariable(Bar bar, Boolean initialValue = false, String def = null)
            : base(bar, InitBoolVariable, def)
        {
            Validating += (s, e) => { e.Valid = true; };

            ValidateAndSet(initialValue);
        }

        /// <summary>
        /// Checks if this variable can hold this value.
        /// </summary>
        private bool IsValid(bool value)
        {
            ThrowIfDisposed();

            return !Validating.GetInvocationList().Select(h => {
                var check = new BoolValidationEventArgs(value);
                h.DynamicInvoke(new object[] { this, check });
                return !check.Valid;
            }).Any(failed => failed);
        }

        /// <summary>
        /// Tries to set this variable's value, validating it.
        /// </summary>
        private void ValidateAndSet(bool value)
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
            bool data = Convert.ToBoolean(Marshal.ReadByte(pointer));
            bool changed = (data != Value);
            Value = data;

            if (changed) {
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            Marshal.WriteByte(pointer, Convert.ToByte(Value));
        }

        #region Customization

        /// <summary>
        /// Gets or sets the "true" label for this variable.
        /// </summary>
        public String LabelTrue
        {
            get { return Tw.GetStringParam(ParentBar.Pointer, ID, "true"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "true", value); }
        }

        /// <summary>
        /// Gets or sets the "false" label for this variable.
        /// </summary>
        public String LabelFalse
        {
            get { return Tw.GetStringParam(ParentBar.Pointer, ID, "false"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "false", value); }
        }

        #endregion

        public override String ToString()
        {
            return String.Format("[BoolVariable: Label={0}, Value={1}]", Label, Value);
        }
    }
}
