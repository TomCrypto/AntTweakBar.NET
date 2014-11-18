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
    public sealed class BoolVariable : Variable
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

        #endregion

        public BoolVariable(Bar bar, Boolean initialValue = false, String def = null)
            : base(bar)
        {
            setCallback = SetCallback;
            getCallback = GetCallback;

            TW.SetCurrentWindow(bar.Owner.Identifier);
            TW.AddVarCB(Owner, ID, TW.VariableType.TW_TYPE_BOOL8,
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
        public Boolean Value { get; set; }

        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            bool tmp = *(bool*)pointer;
            bool changed = tmp != Value;
            Value = tmp;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            *(bool*)pointer = Value;
        }

        #region Customization

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

        #endregion
    }

    /// <summary>
    /// A variable holding an enumeration value.
    /// </summary>
    public class EnumVariable<T> : Variable where T : struct
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

        private T value;

        #endregion

        public EnumVariable(Bar bar, T initialValue, String def = null)
            : base(bar)
        {
            if (!typeof(T).IsEnum)
                throw new InvalidOperationException(String.Format("Type {0} is not an enumeration", typeof(T).Name));

            setCallback = SetCallback;
            getCallback = GetCallback;

            var variableType = TW.DefineEnumFromString(typeof(T).Name, String.Join(",", typeof(T).GetEnumNames()));

            TW.AddVarCB(Owner, ID, variableType, setCallback, getCallback, IntPtr.Zero);
            TW.SetParam(Owner, ID, "enum", EnumString);

            Owner.Add(this);
            Label = "undef";
            if (def != null)
                SetDefinition(def);
            Value = initialValue;
        }

        /// <summary>
        /// Gets or sets the value of this variable.
        /// </summary>
        public T Value
        {
            get { return value; }
            set
            {
                if (!Enum.IsDefined (typeof(T), value))
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value");
                else
                    this.value = value;
            }
        }

        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            int tmp = *(int*)pointer;
            bool changed = tmp != (int)(Object)Value;
            Value = (T)(Object)tmp;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            *(int*)pointer = (int)(Object)Value;
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

