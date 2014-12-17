using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    /// <summary>
    /// Helper class for the ListVariable items.
    /// </summary>
    public class TupleList<T1, T2> : List<Tuple<T1, T2>>
    {
        public void Add(T1 item, T2 item2)
        {
            Add(new Tuple<T1, T2>(item, item2));
        }
    }

    public sealed class ListValidationEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Whether to accept this list value.
        /// </summary>
        public bool Valid { get; set; }
        public T Value { get; private set; }

        public ListValidationEventArgs(T value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// An AntTweakBar variable which can hold a value from a list.
    /// </summary>
    public sealed class ListVariable<T> : Variable, IValueVariable
    {
        /// <summary>
        /// Occurs when the user changes this variable's value.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Occurs when the new value of this variable is validated.
        /// </summary>
        public event EventHandler<ListValidationEventArgs<T>> Validating;

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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IDictionary<T, String> itemToDescr = new Dictionary<T, String>();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IDictionary<String, T> descrToItem = new Dictionary<String, T>();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IList<String> descrIndices = new List<String>();

        /// <summary>
        /// Gets this list variable's type.
        /// </summary>
        public Tw.VariableType Type { get { ThrowIfDisposed(); return type; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Tw.VariableType type;

        /// <summary>
        /// Initialization delegate, which creates the list variable.
        /// </summary>
        private static void InitListVariable(Variable var, String id, IList<Tuple<T, String>> items)
        {
            var it = var as ListVariable<T>;

            Variable.SetCallbacks.Add(id, new Tw.SetVarCallback(it.SetCallback));
            Variable.GetCallbacks.Add(id, new Tw.GetVarCallback(it.GetCallback));

            if (items.Count != items.Select(kv => kv.Item1).Distinct().Count()) {
                throw new ArgumentException("Duplicate items in list.");
            }

            var descr = new Dictionary<int, String>();
            int t = 0;

            foreach (var kv in items) {
                descr.Add(t++, kv.Item2);
            }

            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        it.type = Tw.DefineEnum(Guid.NewGuid().ToString(), descr),
                        Variable.SetCallbacks[id],
                        Variable.GetCallbacks[id],
                        IntPtr.Zero, null);
        }

        /// <summary>
        /// Creates a new list variable in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the list variable in.</param>
        /// <param name="items">The list of items and their descriptions.</param>
        /// <param name="initialValue">The initial value of the variable.</param>
        /// <param name="def">An optional definition string for the new variable.</param>
        public ListVariable(Bar bar, IList<Tuple<T, String>> items, T initialValue, String def = null)
            : base(bar, (var, id) => InitListVariable(var, id, items), def)
        {
            foreach (var kv in items) {
                itemToDescr.Add(kv.Item1, kv.Item2);
                descrToItem.Add(kv.Item2, kv.Item1);
                descrIndices.Add(kv.Item2);
            }

            Validating += (s, e) => { e.Valid = itemToDescr.ContainsKey(e.Value); };
            ValidateAndSet(initialValue);
        }

        /// <summary>
        /// Checks if this variable can hold this value.
        /// </summary>
        private bool IsValid(T value)
        {
            ThrowIfDisposed();

            return !Validating.GetInvocationList().Select(h => {
                var check = new ListValidationEventArgs<T>(value);
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
            T data = descrToItem[descrIndices[Marshal.ReadInt32(pointer)]];

            if (IsValid((T)(object)data))
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
            Marshal.WriteInt32(pointer, descrIndices.IndexOf(itemToDescr[Value]));
        }

        public override String ToString()
        {
            return String.Format("[ListVariable<{0}>: Label={1}, Value={2}]", typeof(T).Name, Label, Value);
        }
    }
}
