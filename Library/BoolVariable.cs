﻿using System;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    /// <summary>
    /// An AntTweakBar variable which can hold a boolean value.
    /// </summary>
    public sealed class BoolVariable : Variable
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
            if (Changed != null)
                Changed(this, e);
        }

        /// <summary>
        /// Gets or sets the value of this variable.
        /// </summary>
        public Boolean Value { get; set; }

        /// <summary>
        /// Initialization delegate, which creates the boolean variable.
        /// </summary>
        private static void InitBoolVariable(Variable var, String id)
        {
            TW.AddVarCB(var.ParentBar.Pointer, id,
                        TW.VariableType.Bool8,
                        ((BoolVariable)var).SetCallback,
                        ((BoolVariable)var).GetCallback,
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
            bool data = Convert.ToBoolean(Marshal.ReadByte(pointer));
            bool changed = (data != Value);
            Value = data;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private readonly TW.GetVarCallback getCallback;
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
            get { return TW.GetStringParam(ParentBar.Pointer, ID, "true"); }
            set { TW.SetParam(ParentBar.Pointer, ID, "true", value); }
        }

        /// <summary>
        /// Gets or sets the "false" label for this variable.
        /// </summary>
        public String LabelFalse
        {
            get { return TW.GetStringParam(ParentBar.Pointer, ID, "false"); }
            set { TW.SetParam(ParentBar.Pointer, ID, "false", value); }
        }

        #endregion

        #region Misc.

        public override String ToString()
        {
            return String.Format("[BoolVariable: Label={0}, Value={1}]", Label, Value);
        }

        #endregion
    }
}