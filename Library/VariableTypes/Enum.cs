using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
                        TW.VariableType.TW_TYPE_BOOL8,
                        ((BoolVariable)var).SetCallback,
                        ((BoolVariable)var).GetCallback,
                        IntPtr.Zero);
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
        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            bool tmp = *(bool*)pointer;
            bool changed = tmp != Value;
            Value = tmp;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private readonly TW.GetVarCallback getCallback;
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

    /// <summary>
    /// An AntTweakBar variable which can hold an enum.
    /// </summary>
    public class EnumVariable<T> : Variable where T : struct
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

        private T value;

        /// <summary>
        /// Initialization delegate, which creates the enum variable.
        /// </summary>
        private static void InitEnumVariable(Variable var, String id)
        {
            if (!typeof(T).IsEnum)
                throw new InvalidOperationException(String.Format("Type {0} is not an enumeration", typeof(T).FullName));

            var enumNames = String.Join(",", typeof(T).GetEnumNames());

            TW.AddVarCB(var.ParentBar.Pointer, id,
                        TW.DefineEnumFromString(typeof(T).FullName, enumNames),
                        ((EnumVariable<T>)var).SetCallback,
                        ((EnumVariable<T>)var).GetCallback,
                        IntPtr.Zero);
        }

        /// <summary>
        /// Creates a new enum variable in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the enum variable in.</param>
        /// <param name="initialValue">The initial value of the variable.</param>
        /// <param name="def">An optional definition string for the new variable.</param>
        public EnumVariable(Bar bar, T initialValue, String def = null)
            : base(bar, InitEnumVariable, def)
        {
            TW.SetParam(ParentBar.Pointer, ID, "enum", GetEnumString());

            setCallback = SetCallback;
            getCallback = GetCallback;
            Value = initialValue;
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private readonly TW.SetVarCallback setCallback;
        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            int tmp = *(int*)pointer;
            bool changed = tmp != (int)(Object)Value;
            Value = (T)(Object)tmp;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private readonly TW.GetVarCallback getCallback;
        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            *(int*)pointer = (int)(Object)Value;
        }

        /// <summary>
        /// Returns a formatted key-value representation of this enum.
        /// </summary>
        private static String GetEnumString()
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

        #region Misc.

        public override String ToString()
        {
            return String.Format("[EnumVariable<{0}>: Label={1}, Value={2}]", typeof(T).Name, Label, Value);
        }

        #endregion
    }
}

