using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    public sealed class FloatValidationEventArgs : EventArgs
    {
        /// <summary>
        /// Whether to accept this single-precision value.
        /// </summary>
        public bool Valid { get; set; }
        public readonly float Value;

        public FloatValidationEventArgs(float value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// An AntTweakBar variable which can hold a single-precision floating-point number.
    /// </summary>
    public sealed class FloatVariable : Variable
    {
        /// <summary>
        /// Occurs when the user changes this variable's value.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Occurs when the new value of this variable is validated.
        /// </summary>
        public event EventHandler<FloatValidationEventArgs> Validating;

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
        public Single Value
        {
            get { ThrowIfDisposed(); return value; }
            set { ValidateAndSet(value); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Single value;

        /// <summary>
        /// Initialization delegate, which creates the floating-point variable.
        /// </summary>
        private static void InitFloatVariable(Variable var, String id)
        {
            var it = var as FloatVariable;

            Variable.SetCallbacks.Add(id, new Tw.SetVarCallback(it.SetCallback));
            Variable.GetCallbacks.Add(id, new Tw.GetVarCallback(it.GetCallback));

            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        Tw.VariableType.Float,
                        Variable.SetCallbacks[id],
                        Variable.GetCallbacks[id],
                        IntPtr.Zero, null);
        }

        /// <summary>
        /// Creates a new single-precision floating-point variable in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the floating-point variable in.</param>
        /// <param name="initialValue">The initial value of the variable.</param>
        /// <param name="def">An optional definition string for the new variable.</param>
        public FloatVariable(Bar bar, Single initialValue = 0, String def = null)
            : base(bar, InitFloatVariable, def)
        {
            Validating += (s, e) => { e.Valid = (Min <= e.Value) && (e.Value <= Max); };

            ValidateAndSet(initialValue);
        }

        /// <summary>
        /// Checks if this variable can hold this value.
        /// </summary>
        private bool IsValid(float value)
        {
            ThrowIfDisposed();

            return !Validating.GetInvocationList().Select(h => {
                var check = new FloatValidationEventArgs(value);
                h.DynamicInvoke(new object[] { this, check });
                return !check.Valid;
            }).Any(failed => failed);
        }

        /// <summary>
        /// Tries to set this variable's value, validating it.
        /// </summary>
        private void ValidateAndSet(float value)
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
            float[] data = new float[1]; /* Value */
            Marshal.Copy(pointer, data, 0, data.Length);

            if (IsValid(data[0]))
            {
                bool changed = (data[0] != value);
                value = data[0];

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
            float[] data = new float[] { Value };
            Marshal.Copy(data, 0, pointer, data.Length);
        }

        #region Customization

        /// <summary>
        /// Gets or sets this variable's minimum value.
        /// </summary>
        public Single Min
        {
            get { return Tw.GetSingleParam(ParentBar.Pointer, ID, "min")[0]; }
            set
            {
                Tw.SetParam(ParentBar.Pointer, ID, "min", value);
                Value = Math.Max(value, Value);
            }
        }

        /// <summary>
        /// Gets or sets this variable's maximum value.
        /// </summary>
        public Single Max
        {
            get { return Tw.GetSingleParam(ParentBar.Pointer, ID, "max")[0]; }
            set
            {
                Tw.SetParam(ParentBar.Pointer, ID, "max", value);
                Value = Math.Min(value, Value);
            }
        }

        /// <summary>
        /// Gets or sets this variable's step (increment).
        /// </summary>
        public Single Step
        {
            get { return Tw.GetSingleParam(ParentBar.Pointer, ID, "step")[0]; }
            set { Tw.SetParam(ParentBar.Pointer, ID, "step", value); }
        }

        /// <summary>
        /// Gets or sets this variable's precision.
        /// </summary>
        public Single Precision
        {
            get { return Tw.GetSingleParam(ParentBar.Pointer, ID, "precision")[0]; }
            set { Tw.SetParam(ParentBar.Pointer, ID, "precision", value); }
        }

        #endregion

        public override String ToString()
        {
            return String.Format("[SingleVariable: Label={0}, Value={1}]", Label, Value);
        }
    }
}
