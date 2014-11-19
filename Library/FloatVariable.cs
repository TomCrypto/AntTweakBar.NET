﻿using System;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    /// <summary>
    /// An AntTweakBar variable which can hold a single-precision floating-point number.
    /// </summary>
    public class FloatVariable : Variable
    {
        /// <summary>
        /// Occurs when the user changes this variable's value.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        private void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        /// <summary>
        /// Gets or sets the value of this variable.
        /// </summary>
        public Single Value
        {
            get { return value; }
            set
            {
                if (!((Min <= value) && (value <= Max)))
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value");
                else
                    this.value = value;
            }
        }

        private Single value;

        /// <summary>
        /// Initialization delegate, which creates the floating-point variable.
        /// </summary>
        private static void InitFloatVariable(Variable var, String id)
        {
            TW.AddVarCB(var.ParentBar.Pointer, id,
                        TW.VariableType.Float,
                        ((FloatVariable)var).SetCallback,
                        ((FloatVariable)var).GetCallback,
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
            setCallback = SetCallback;
            getCallback = GetCallback;
            Value = initialValue;
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private readonly TW.SetVarCallback setCallback;
        private void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            float[] data = new float[1]; /* Value */
            Marshal.Copy(pointer, data, 0, data.Length);

            bool changed = (data[0] != Value);

            Value = data[0];

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private readonly TW.GetVarCallback getCallback;
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
            get { return TW.GetSingleParam(ParentBar.Pointer, ID, "min")[0]; }
            set
            {
                TW.SetParam(ParentBar.Pointer, ID, "min", value);
                Value = Math.Max(value, Value);
            }
        }

        /// <summary>
        /// Gets or sets this variable's maximum value.
        /// </summary>
        public Single Max
        {
            get { return TW.GetSingleParam(ParentBar.Pointer, ID, "max")[0]; }
            set
            {
                TW.SetParam(ParentBar.Pointer, ID, "max", value);
                Value = Math.Min(value, Value);
            }
        }

        /// <summary>
        /// Gets or sets this variable's step (increment).
        /// </summary>
        public Single Step
        {
            get { return TW.GetSingleParam(ParentBar.Pointer, ID, "step")[0]; }
            set { TW.SetParam(ParentBar.Pointer, ID, "step", value); }
        }

        /// <summary>
        /// Gets or sets this variable's precision.
        /// </summary>
        public Single Precision
        {
            get { return TW.GetSingleParam(ParentBar.Pointer, ID, "precision")[0]; }
            set { TW.SetParam(ParentBar.Pointer, ID, "precision", value); }
        }

        #endregion

        public override String ToString()
        {
            return String.Format("[SingleVariable: Label={0}, Value={1}]", Label, Value);
        }
    }
}