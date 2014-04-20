using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    /// <summary>
    /// Represents an abstract bar variable.
    /// </summary>
    /// <remarks>
    /// Note: derived classes _must_ create an AntTweakBar variable
    /// using the generated identifier for this base class to work.
    /// </remarks>
    public abstract class Variable : IDisposable
    {
        private readonly String id;
        private readonly Bar owner;

        /// <summary>
        /// Gets the owner of this variable.
        /// </summary>
        internal Bar Owner { get { return owner; } }

        /// <summary>
        /// Gets the identifier of this variable.
        /// </summary>
        internal String ID { get { return id; } }

        protected Variable(Bar bar)
        {
            if ((this.owner = bar) == null)
                throw new ArgumentNullException("bar");
            else
                this.id = Guid.NewGuid().ToString();
        }

        #region Customization

        /// <summary>
        /// Configures the variable from a definition string.
        /// </summary>
        public void SetDefinition(String def)
        {
            if (def == null)
                throw new ArgumentNullException("def");
            else
            {
                TW.SetCurrentWindow(Owner.Owner.WindowIndex); // ??
                TW.Define(String.Format("{0}/{1} {2}", Owner.ID, ID, def));
            }
        }

        /// <summary>
        /// Gets or sets the label of this variable.
        /// </summary>
        public String Label
        {
            get { return TW.GetStringParam(Owner, ID, "label"); }
            set { TW.SetParam(Owner, ID, "label", value); }
        }

        /// <summary>
        /// Gets or sets the help text of this variable.
        /// </summary>
        public String Help
        {
            get { return TW.GetStringParam(Owner, ID, "help"); }
            set { TW.SetParam(Owner, ID, "help", value); }
        }

        /// <summary>
        /// Gets or sets the group this variable is in.
        /// </summary>
        public String Group
        {
            get { return TW.GetStringParam(Owner, ID, "group"); }
            set { TW.SetParam(Owner, ID, "group", value); }
        }

        /// <summary>
        /// Gets or sets whether this variable is visible.
        /// </summary>
        public Boolean Visible
        {
            get { return TW.GetBooleanParam(Owner, ID, "visible"); }
            set { TW.SetParam(Owner, ID, "visible", value); }
        }

        /// <summary>
        /// Gets or sets whether this variable is read-only.
        /// </summary>
        public Boolean ReadOnly
        {
            get { return TW.GetBooleanParam(Owner, ID, "readonly"); }
            set { TW.SetParam(Owner, ID, "readonly", value); }
        }

        /// <summary>
        /// Gets or sets the key shortcut for this variable.
        /// </summary>
        public String KeyShortcut
        {
            get { return TW.GetStringParam(Owner, ID, "key"); }
            set { TW.SetParam(Owner, ID, "key", value); }
        }

        /// <summary>
        /// Gets or sets the increment key shortcut for this variable.
        /// </summary>
        public String KeyIncrementShortcut
        {
            get { return TW.GetStringParam(Owner, ID, "keyincr"); }
            set { TW.SetParam(Owner, ID, "keyincr", value); }
        }

        /// <summary>
        /// Gets or sets the decrement key shortcut for this variable.
        /// </summary>
        public String KeyDecrementShortcut
        {
            get { return TW.GetStringParam(Owner, ID, "keydecr"); }
            set { TW.SetParam(Owner, ID, "keydecr", value); }
        }

        #endregion

        #region IDisposable

        ~Variable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (Owner.Contains(this))
            {
                TW.RemoveVar(Owner, ID);
                Owner.Remove(this);
            }

            disposed = true;
        }

        private bool disposed = false;

        #endregion
    }

    /// <summary>
    /// A variable representing a single value of a specific type.
    /// </summary>
    /// <remarks>
    /// Internal use only.
    /// </remarks>
    /// <remarks>
    /// This is a reflective-polymorphic class which acts based on
    /// the type of T - it is a hack, here be dragons, however, it
    /// is a lot better than the alternative of scattering pointer
    /// operations across multiple end-user classes. This way, the
    /// Variable base class manages the AntTweakBar interop, while
    /// the derived classes provide extra functionality/checking.
    /// </remarks>
    /// <remarks>
    /// This class operates by sharing a variable object in memory
    /// with the AntTweakBar library in a coherent fashion. It has
    /// to make sure the variable is kept synchronized between the
    /// C# and native implementations, and performs quite a lot of
    /// bounds checking to ensure that the programmer can't modify
    /// the value of the variable in a way that the user couldn't,
    /// which would lead to an inconsistent variable state. All of
    /// this checking is done by derived (non-abstract) classes.
    /// </remarks>
    public abstract class Variable<T> : Variable
    {
        internal readonly TW.VariableType variableType;
        private readonly TW.GetVarCallback getCallback;
        private readonly TW.SetVarCallback setCallback;

        /// <summary>
        /// Occurs when the user changes the variable.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// floataises the Changed event.
        /// </summary>
        private void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        private T value;

        /// <summary>
        /// Gets or sets the variable's value.
        /// </summary>
        public T Value
        {
            get { return value; }

            set
            {
                if (Validate(value))
                    this.value = value;
                else
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value");
            }
        }

        /// <summary>
        /// Validates a value in the same way AntTweakBar does.
        /// </summary>
        protected abstract bool Validate(T newValue);

        /// <summary>
        /// Creates a new variable of a specific type.
        /// </summary>
        internal Variable(Bar bar, T initialValue = default(T), String def = null) : base(bar)
        {
            /* Save both delegates. */
            setCallback = SetCallback;
            getCallback = GetCallback;

            variableType = ResolveType(value = initialValue); /* Detect variable type */
            TW.AddVarCB(Owner, ID, variableType, setCallback, getCallback, IntPtr.Zero);
            if (typeof(T).IsEnum) variableType = TW.VariableType.TW_TYPE_UNDEF; // enum!

            Owner.Add(this);
            Label = "undef";

            if (def != null)
                SetDefinition(def);
        }

        /// <summary>
        /// Called by AntTweakBar to set the variable value.
        /// </summary>
        private void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            if (!Value.Equals(Value = ReadPointer(variableType, pointer)))
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar to get the variable value.
        /// </summary>
        private void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            WritePointer(variableType, pointer, Value);
        }

        #region Helper Methods

        /// <summary>
        /// Helper method to cast to generic types.
        /// </summary>
        internal static T2 Cast<T2>(Object obj)
        {
            return (T2)obj;
        }

        /// <summary>
        /// Helper method to invoke a constructor.
        /// </summary>
        internal static T2 Create<T2>(params Object[] args)
        {
            var argTypes = args.Select(x => x.GetType()).ToArray(); // Save the types of each parameter, in order to look up the constructor.
            var ctor = typeof(T2).GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, argTypes, null);

            if (ctor == null)
                throw new InvalidOperationException(String.Format("Missing constructor {0}({1}) expected", typeof(T2).Name, String.Join(", ", argTypes.Select(t => t.Name))));
            else
                return (T2)ctor.Invoke(args);
        }

        #endregion

        #region Polymorphic Methods

        /// <summary>
        /// Matches a generic type with a suitable AntTweakBar variable type.
        /// </summary>
        private static TW.VariableType ResolveType(T value)
        {
            if (typeof(T).IsEnum)
                return TW.DefineEnumFromString(typeof(T).Name, String.Join(",", typeof(T).GetEnumNames()));
            if (typeof(String).IsAssignableFrom(typeof(T)))
                return TW.VariableType.TW_TYPE_CSSTRING;
            if (typeof(Boolean).IsAssignableFrom(typeof(T)))
                return TW.VariableType.TW_TYPE_BOOL8;
            if (typeof(Int32).IsAssignableFrom(typeof(T)))
                return TW.VariableType.TW_TYPE_INT32;
            if (typeof(Single).IsAssignableFrom(typeof(T)))
                return TW.VariableType.TW_TYPE_FLOAT;
            if (typeof(Double).IsAssignableFrom(typeof(T)))
                return TW.VariableType.TW_TYPE_DOUBLE;
            if (typeof(ColorType).IsAssignableFrom(typeof(T)))
                return TW.VariableType.TW_TYPE_COLOR3F;
            if (typeof(VectorType).IsAssignableFrom(typeof(T)))
                return TW.VariableType.TW_TYPE_DIR3F;
            if (typeof(QuaternionType).IsAssignableFrom(typeof(T)))
                return TW.VariableType.TW_TYPE_QUAT4F;

            throw new ArgumentException(String.Format("Type '{0}' cannot be used as a variable type", typeof(T).Name));
        }

        /// <summary>
        /// floateads the supplied pointer and interprets the variable value.
        /// </summary>
        private unsafe static T ReadPointer(TW.VariableType varType, IntPtr pointer)
        {
            switch (varType)
            {
                case TW.VariableType.TW_TYPE_UNDEF:
                    return Cast<T>(*(int*)pointer);
                case TW.VariableType.TW_TYPE_CSSTRING:
                    return Cast<T>(Marshal.PtrToStringAnsi(pointer));
                case TW.VariableType.TW_TYPE_BOOL8:
                    return Cast<T>(*(bool*)pointer);
                case TW.VariableType.TW_TYPE_INT32:
                    return Cast<T>(*(int*)pointer);
                case TW.VariableType.TW_TYPE_FLOAT:
                    return Cast<T>(*(float*)pointer);
                case TW.VariableType.TW_TYPE_DOUBLE:
                    return Cast<T>(*(double*)pointer);
                case TW.VariableType.TW_TYPE_COLOR3F:
                    return Create<T>(((float*)pointer)[0],
                                     ((float*)pointer)[1],
                                     ((float*)pointer)[2]);
                case TW.VariableType.TW_TYPE_DIR3F:
                    return Create<T>(((float*)pointer)[0],
                                     ((float*)pointer)[1],
                                     ((float*)pointer)[2]);
                case TW.VariableType.TW_TYPE_QUAT4F:
                    return Create<T>(((float*)pointer)[0],
                                     ((float*)pointer)[1],
                                     ((float*)pointer)[2],
                                     ((float*)pointer)[3]);
                default:
                    throw new InvalidOperationException("Unknown variable type");
            }
        }

        /// <summary>
        /// Writes the variable value into the supplied pointer.
        /// </summary>
        private unsafe static void WritePointer(TW.VariableType varType, IntPtr pointer, T value)
        {
            switch (varType)
            {
                case TW.VariableType.TW_TYPE_UNDEF:
                    *(int*)pointer = Cast<int>(value);
                    return;
                case TW.VariableType.TW_TYPE_CSSTRING:
                    var bytes = Encoding.UTF8.GetBytes(Cast<String>(value));
                    Marshal.Copy(bytes, 0, pointer, bytes.Length);
                    ((byte*)pointer)[bytes.Length] = 0;
                    return;
                case TW.VariableType.TW_TYPE_BOOL8:
                    *(bool*)pointer = Cast<Boolean>(value);
                    return;
                case TW.VariableType.TW_TYPE_INT32:
                    *(int*)pointer = Cast<Int32>(value);
                    return;
                case TW.VariableType.TW_TYPE_FLOAT:
                    *(float*)pointer = Cast<Single>(value);
                    return;
                case TW.VariableType.TW_TYPE_DOUBLE:
                    *(double*)pointer = Cast<Double>(value);
                    return;
                case TW.VariableType.TW_TYPE_COLOR3F:
                    ((float*)pointer)[0] = Cast<ColorType>(value).R;
                    ((float*)pointer)[1] = Cast<ColorType>(value).G;
                    ((float*)pointer)[2] = Cast<ColorType>(value).B;
                    return;
                case TW.VariableType.TW_TYPE_DIR3F:
                    ((float*)pointer)[0] = Cast<VectorType>(value).X;
                    ((float*)pointer)[1] = Cast<VectorType>(value).Y;
                    ((float*)pointer)[2] = Cast<VectorType>(value).Z;
                    return;
                case TW.VariableType.TW_TYPE_QUAT4F:
                    ((float*)pointer)[0] = Cast<QuaternionType>(value).X;
                    ((float*)pointer)[1] = Cast<QuaternionType>(value).Y;
                    ((float*)pointer)[2] = Cast<QuaternionType>(value).Z;
                    ((float*)pointer)[3] = Cast<QuaternionType>(value).W;
                    return;
                default:
                    throw new InvalidOperationException("Unknown variable type");
            }
        }

        #endregion
    }
}
