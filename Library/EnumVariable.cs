using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    public sealed class EnumValidationEventArgs<T> : EventArgs where T : struct
    {
        /// <summary>
        /// Whether to accept this enum value.
        /// </summary>
        public bool Valid { get; set; }
        public T Value { get; private set; }

        public EnumValidationEventArgs(T value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// An AntTweakBar variable which can hold an enum.
    /// </summary>
    public sealed class EnumVariable<T> : Variable where T : struct
    {
        /// <summary>
        /// Occurs when the user changes this variable's value.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Occurs when the new value of this variable is validated.
        /// </summary>
        public event EventHandler<EnumValidationEventArgs<T>> Validating;

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
        public T Value
        {
            get { ThrowIfDisposed(); return value; }
            set { ValidateAndSet(value); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private T value;

        /// <summary>
        /// Initialization delegate, which creates the enum variable.
        /// </summary>
        private static void InitEnumVariable(Variable var, String id)
        {
            if (!typeof(T).IsEnum) {
                throw new ArgumentException(String.Format("Type {0} is not an enumeration.", typeof(T).FullName));
            }

            var it = var as EnumVariable<T>;

            Variable.SetCallbacks.Add(id, new Tw.SetVarCallback(it.SetCallback));
            Variable.GetCallbacks.Add(id, new Tw.GetVarCallback(it.GetCallback));

            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        Tw.DefineEnum(typeof(T).FullName, GetEnumLabels()),
                        Variable.SetCallbacks[id],
                        Variable.GetCallbacks[id],
                        IntPtr.Zero, null);
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
            Validating += (s, e) => { e.Valid = Enum.IsDefined(typeof(T), e.Value); };
            ValidateAndSet(initialValue);
        }

        /// <summary>
        /// Checks if this variable can hold this value.
        /// </summary>
        private bool IsValid(T value)
        {
            ThrowIfDisposed();

            return !Validating.GetInvocationList().Select(h => {
                var check = new EnumValidationEventArgs<T>(value);
                h.DynamicInvoke(new object[] { this, check });
                return !check.Valid;
            }).Any(failed => failed);
        }

        /// <summary>
        /// Tries to set this variable's value, validating it.
        /// </summary>
        private void ValidateAndSet(T value)
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
            int data = Marshal.ReadInt32(pointer);

            if (IsValid((T)(object)data))
            {
                bool changed = (data != (int)(object)value);
                value = (T)(object)data;

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
            Marshal.WriteInt32(pointer, (int)(object)Value);
        }

        /// <summary>
        /// Returns the enum type's values and labels/descriptions.
        /// </summary>
        private static IDictionary<int, String> GetEnumLabels()
        {
            var labels = new Dictionary<int, String>();

            foreach (var kv in ((int[])Enum.GetValues(typeof(T))).Zip(typeof(T).GetEnumNames(), (i, n) => new Tuple<int, string>(i, n)))
            {
                var attr = typeof(T).GetMember(kv.Item2)[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                labels.Add(kv.Item1, attr.Any() ? ((DescriptionAttribute)attr.First()).Description : kv.Item2);
            }

            return labels;
        }

        public override String ToString()
        {
            return String.Format("[EnumVariable<{0}>: Label={1}, Value={2}]", typeof(T).Name, Label, Value);
        }
    }
}
