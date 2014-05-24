using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

namespace AntTweakBar
{
    /// <summary>
    /// A variable holding a boolean value.
    /// </summary>
    public sealed class BoolVariable : Variable<Boolean>
    {
        public BoolVariable(Bar bar, Boolean initialValue = false, String def = null)
            : base(bar, initialValue, def)
        {

        }

        protected override bool Validate(Boolean newValue)
        {
            return true; // no restrictions
        }

        /// <summary>
        /// Gets or sets the "true" label for this variable.
        /// </summary>
        public String LabelTrue
        {
            get { return TW.GetStringParam(Owner, ID, "true"); }
            set { TW.SetParam(Owner, ID, "true", value); }
        }

        /// <summary>
        /// Gets or sets the "false" label for this variable.
        /// </summary>
        public String LabelFalse
        {
            get { return TW.GetStringParam(Owner, ID, "false"); }
            set { TW.SetParam(Owner, ID, "false", value); }
        }
    }

    /// <summary>
    /// A variable holding an enumeration value.
    /// </summary>
    public sealed class EnumVariable<T> : Variable<T>
    {
        public EnumVariable(Bar bar, T initialValue = default(T), String def = null)
            : base(bar, initialValue, def)
        {
            if (variableType != TW.VariableType.TW_TYPE_UNDEF) // check the type was actually an enumeration
                throw new InvalidOperationException(String.Format("Type {0} is not an enumeration", typeof(T).Name));

            TW.SetParam(Owner, ID, "enum", EnumString);
        }

        protected override bool Validate(T newValue)
        {
            return Enum.IsDefined(typeof(T), (int)(object)newValue);
        }

        /// <summary>
        /// Gets a formatted key-value representation of this enum.
        /// </summary>
        private static String EnumString
        {
            get
            {
                IList<String> enumList = new List<String>();

                foreach (var kv in ((int[])Enum.GetValues(typeof(T))).Zip(typeof(T).GetEnumNames(), (i, n) => new Tuple<int, string>(i, n)))
                {
                    var valueAttributes = typeof(T).GetMember(kv.Item2)[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                    string label = valueAttributes.Any() ? ((DescriptionAttribute)valueAttributes.First()).Description : kv.Item2;
                    enumList.Add(String.Format("{0} {{{1}}}", kv.Item1, label)); // Follows the AntTweakBar enum string format.
                }

                return String.Join(",", enumList);
            }
        }
    }
}

