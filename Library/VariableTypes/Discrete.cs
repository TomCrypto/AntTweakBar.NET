using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace AntTweakBar
{
    /// <summary>
    /// A variable holding an integer value.
    /// </summary>
    public sealed class IntVariable : Variable<Int32>
    {
        public IntVariable(Bar bar, Int32 initialValue = 0, String def = null)
            : base(bar, initialValue, def)
        {

        }

        protected override bool Validate(Int32 newValue)
        {
            return (Min <= newValue) && (newValue <= Max);
        }

        /// <summary>
        /// Gets or sets the minimum value of this variable.
        /// </summary>
        public Int32 Min
        {
            get { return TW.GetIntParam(Owner, ID, "min")[0]; }
            set
            {
                TW.SetParam(Owner, ID, "min", value);
                Value = Math.Max(value, Value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value of this variable.
        /// </summary>
        public Int32 Max
        {
            get { return TW.GetIntParam(Owner, ID, "max")[0]; }
            set
            {
                TW.SetParam(Owner, ID, "max", value);
                Value = Math.Min(value, Value);
            }
        }

        /// <summary>
        /// Gets or sets whether to use hexadecimal notation.
        /// </summary>
        public Boolean Hexadecimal
        {
            get { return TW.GetBooleanParam(Owner, ID, "hexa"); }
            set { TW.SetParam(Owner, ID, "hexa", value); }
        }

        /// <summary>
        /// Gets or sets the step (increment) of this variable.
        /// </summary>
        public Int32 Step
        {
            get { return TW.GetIntParam(Owner, ID, "step")[0]; }
            set { TW.SetParam(Owner, ID, "step", value); }
        }
    }
}

