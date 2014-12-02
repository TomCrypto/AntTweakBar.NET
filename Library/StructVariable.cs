using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    public sealed class StructValidationEventArgs<T> : EventArgs where T : struct
    {
        /// <summary>
        /// Whether to accept this integer value.
        /// </summary>
        public bool Valid { get; set; }
        public T Value { get; private set; }

        public StructValidationEventArgs(T value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// An AntTweakBar variable which can hold a composite struct.
    /// </summary>
    public sealed class StructVariable<T> : Variable where T : struct
    {
        /// <summary>
        /// Occurs when the user changes this variable's value.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Occurs when the new value of this variable is validated.
        /// </summary>
        public event EventHandler<StructValidationEventArgs<T>> Validating;

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
        /// Gets this struct variable's type.
        /// </summary>
        public Tw.VariableType Type { get { ThrowIfDisposed(); return type; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Tw.VariableType type;

        /// <summary>
        /// Initialization delegate, which creates the struct variable.
        /// </summary>
        private static void InitStructVariable(Variable var, String id, IDictionary<String, Tw.StructMemberInfo> members)
        {
            if (!(typeof(T).IsValueType && !typeof(T).IsPrimitive && !typeof(T).IsEnum)) {
                throw new ArgumentException(String.Format("Type {0} is not a struct.", typeof(T).FullName));
            }

            var it = var as StructVariable<T>;

            Variable.SetCallbacks.Add(id, new Tw.SetVarCallback(it.SetCallback));
            Variable.GetCallbacks.Add(id, new Tw.GetVarCallback(it.GetCallback));

            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        it.type = Tw.DefineStruct(Guid.NewGuid().ToString(), members, (int)Marshal.SizeOf(typeof(T)), null, IntPtr.Zero),
                        Variable.SetCallbacks[id],
                        Variable.GetCallbacks[id],
                        IntPtr.Zero, null);
        }

        /// <summary>
        /// Creates a new struct variable in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the struct variable in.</param>
        /// <param name="members">The set of struct members to consider.</param>
        /// <param name="initialValue">The initial value of the variable.</param>
        /// <param name="def">An optional definition string for the new variable.</param>
        public StructVariable(Bar bar, IDictionary<String, Tw.StructMemberInfo> members, T initialValue = default(T), String def = null)
            : base(bar, (var, id) => InitStructVariable(var, id, members), def)
        {
            Validating += (s, e) => { e.Valid = true; };

            ValidateAndSet(initialValue);
        }

        /// <summary>
        /// Checks if this variable can hold this value.
        /// </summary>
        private bool IsValid(T value)
        {
            ThrowIfDisposed();

            return !Validating.GetInvocationList().Select(h => {
                var check = new StructValidationEventArgs<T>(value);
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
            T data = (T)Marshal.PtrToStructure(pointer, typeof(T));

            if (IsValid(data))
            {
                bool changed = !value.Equals(data);
                value = data;

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
            Marshal.StructureToPtr(Value, pointer, false);
        }

        public override String ToString()
        {
            return String.Format("[StructVariable: Label={0}, Value={1}]", Label, Value);
        }
    }
}

