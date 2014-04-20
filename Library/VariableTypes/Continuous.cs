using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace AntTweakBar
{
    /// <summary>
    /// A variable holding a single-precision floating-point value.
    /// </summary>
    public sealed class FloatVariable : Variable<Single>
    {
        public FloatVariable(Bar bar, Single initialValue = 0, String def = null)
            : base(bar, initialValue, def)
        {

        }

        protected override bool Validate(Single newValue)
        {
            return (Min <= newValue) && (newValue <= Max);
        }

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
    }

    /// <summary>
    /// A variable holding a double precision floating-point value.
    /// </summary>
    public sealed class DoubleVariable : Variable<Double>
    {
        public DoubleVariable(Bar bar, Double initialValue = 0, String def = null)
            : base(bar, initialValue, def)
        {

        }

        protected override bool Validate(Double newValue)
        {
            return (Min <= newValue) && (newValue <= Max);
        }

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
    }
}

