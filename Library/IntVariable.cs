using System;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    /// <summary>
    /// An AntTweakBar variable which can hold an integer.
    /// </summary>
    public class IntVariable : Variable
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
        public Int32 Value
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

        private Int32 value;

        /// <summary>
        /// Initialization delegate, which creates the integer variable.
        /// </summary>
        private static void InitIntVariable(Variable var, String id)
        {
            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        Tw.VariableType.Int32,
                        ((IntVariable)var).SetCallback,
                        ((IntVariable)var).GetCallback,
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
            setCallback = SetCallback;
            getCallback = GetCallback;
            Value = initialValue;
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private readonly Tw.SetVarCallback setCallback;
        private void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            int tmp = Marshal.ReadInt32(pointer);
            bool changed = tmp != Value;
            Value = tmp;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private readonly Tw.GetVarCallback getCallback;
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
