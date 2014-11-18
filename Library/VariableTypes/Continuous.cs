using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace AntTweakBar
{
    /// <summary>
    /// A variable holding a single-precision floating-point value.
    /// </summary>
    public class FloatVariable : Variable
    {
        #region Fields

        private readonly TW.GetVarCallback getCallback;
        private readonly TW.SetVarCallback setCallback;

        /// <summary>
        /// Occurs when the user changes the variable.
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

        private Single value;

        #endregion

        public FloatVariable(Bar bar, Single initialValue = 0, String def = null)
            : base(bar)
        {
            setCallback = SetCallback;
            getCallback = GetCallback;

            TW.SetCurrentWindow(bar.Owner.Identifier);
            TW.AddVarCB(Owner, ID, TW.VariableType.TW_TYPE_FLOAT,
                        setCallback, getCallback, IntPtr.Zero);

            Owner.Add(this);
            Label = "undef";
            if (def != null)
                SetDefinition(def);
            Value = initialValue;
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

        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            float tmp = *(float*)pointer;
            bool changed = tmp != Value;
            Value = tmp;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            *(float*)pointer = Value;
        }

        #region Customization

        /// <summary>
        /// Gets or sets the minimum value of this variable.
        /// </summary>
        public Single Min
        {
            get { return TW.GetSingleParam(Owner, ID, "min")[0]; }
            set
            {
                TW.SetParam(Owner, ID, "min", value);
                Value = Math.Max(value, Value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value of this variable.
        /// </summary>
        public Single Max
        {
            get { return TW.GetSingleParam(Owner, ID, "max")[0]; }
            set
            {
                TW.SetParam(Owner, ID, "max", value);
                Value = Math.Min(value, Value);
            }
        }

        /// <summary>
        /// Gets or sets the step (increment) of this variable.
        /// </summary>
        public Single Step
        {
            get { return TW.GetSingleParam(Owner, ID, "step")[0]; }
            set { TW.SetParam(Owner, ID, "step", value); }
        }

        /// <summary>
        /// Gets or sets the precision of this variable.
        /// </summary>
        public Single Precision
        {
            get { return TW.GetSingleParam(Owner, ID, "precision")[0]; }
            set { TW.SetParam(Owner, ID, "precision", value); }
        }

        #endregion
    }

    /// <summary>
    /// A variable holding a double precision floating-point value.
    /// </summary>
    public class DoubleVariable : Variable
    {
        #region Fields

        private readonly TW.GetVarCallback getCallback;
        private readonly TW.SetVarCallback setCallback;

        /// <summary>
        /// Occurs when the user changes the variable.
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

        private Double value;

        #endregion

        public DoubleVariable(Bar bar, Double initialValue = 0, String def = null)
            : base(bar)
        {
            setCallback = SetCallback;
            getCallback = GetCallback;

            TW.SetCurrentWindow(bar.Owner.Identifier);
            TW.AddVarCB(Owner, ID, TW.VariableType.TW_TYPE_DOUBLE,
                        setCallback, getCallback, IntPtr.Zero);

            Owner.Add(this);
            Label = "undef";
            if (def != null)
                SetDefinition(def);
            Value = initialValue;
        }

        /// <summary>
        /// Gets or sets the value of this variable.
        /// </summary>
        public Double Value
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

        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            double tmp = *(double*)pointer;
            bool changed = tmp != Value;
            Value = tmp;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            *(double*)pointer = Value;
        }

        #region Customization

        /// <summary>
        /// Gets or sets the minimum value of this variable.
        /// </summary>
        public Double Min
        {
            get { return TW.GetDoubleParam(Owner, ID, "min")[0]; }
            set
            {
                TW.SetParam(Owner, ID, "min", value);
                Value = Math.Max(value, Value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value of this variable.
        /// </summary>
        public Double Max
        {
            get { return TW.GetDoubleParam(Owner, ID, "max")[0]; }
            set
            {
                TW.SetParam(Owner, ID, "max", value);
                Value = Math.Min(value, Value);
            }
        }

        /// <summary>
        /// Gets or sets the step (increment) of this variable.
        /// </summary>
        public Double Step
        {
            get { return TW.GetDoubleParam(Owner, ID, "step")[0]; }
            set { TW.SetParam(Owner, ID, "step", value); }
        }

        /// <summary>
        /// Gets or sets the precision of this variable.
        /// </summary>
        public Double Precision
        {
            get { return TW.GetDoubleParam(Owner, ID, "precision")[0]; }
            set { TW.SetParam(Owner, ID, "precision", value); }
        }

        #endregion
    }
}

