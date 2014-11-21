using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    public sealed class VectorValidationEventArgs : EventArgs
    {
        /// <summary>
        /// Whether to accept this vector value.
        /// </summary>
        public bool Valid { get; set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public VectorValidationEventArgs(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    /// <summary>
    /// An AntTweakBar variable which can hold a 3D vector.
    /// </summary>
    public sealed class VectorVariable : Variable
    {
        /// <summary>
        /// Occurs when the user changes this variable's value.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Occurs when the new value of this variable is validated.
        /// </summary>
        public event EventHandler<VectorValidationEventArgs> Validating;

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
        /// Gets or sets this vector's X-component.
        /// </summary>
        public float X
        {
            get { ThrowIfDisposed(); return x; }
            set { ValidateAndSet(value, Y, Z); }
        }

        /// <summary>
        /// Gets or sets this vector's Y-component.
        /// </summary>
        public float Y
        {
            get { ThrowIfDisposed(); return y; }
            set { ValidateAndSet(X, value, Z); }
        }

        /// <summary>
        /// Gets or sets this vector's Z-component.
        /// </summary>
        public float Z
        {
            get { ThrowIfDisposed(); return z; }
            set { ValidateAndSet(X, Y, value); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float x;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float y;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float z;

        /// <summary>
        /// Initialization delegate, which creates the vector variable.
        /// </summary>
        private static void InitVectorVariable(Variable var, String id)
        {
            var it = var as VectorVariable;

            Variable.SetCallbacks.Add(id, new Tw.SetVarCallback(it.SetCallback));
            Variable.GetCallbacks.Add(id, new Tw.GetVarCallback(it.GetCallback));

            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        Tw.VariableType.Dir3F,
                        Variable.SetCallbacks[id],
                        Variable.GetCallbacks[id],
                        IntPtr.Zero, null);
        }

        /// <summary>
        /// Creates a new vector variable in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the vector variable in.</param>
        /// <param name="x">The initial X-component value of the variable.</param>
        /// <param name="y">The initial Y-component value of the variable.</param>
        /// <param name="z">The initial Z-component value of the variable.</param>
        /// <param name="def">An optional definition string for the new variable.</param>
        public VectorVariable(Bar bar, float x = 0, float y = 0, float z = 0, String def = null)
            : base(bar, InitVectorVariable, def)
        {
            Validating += (s, e) => { e.Valid = true; };

            ValidateAndSet(x, y, z);
        }

        /// <summary>
        /// Checks if this variable can hold this value.
        /// </summary>
        private bool IsValid(float x, float y, float z)
        {
            ThrowIfDisposed();

            return !Validating.GetInvocationList().Select(h => {
                var check = new VectorValidationEventArgs(x, y, z);
                h.DynamicInvoke(new object[] { this, check });
                return !check.Valid;
            }).Any(failed => failed);
        }

        /// <summary>
        /// Tries to set this variable's value, validating it.
        /// </summary>
        private void ValidateAndSet(float x, float y, float z)
        {
            if (!IsValid(x, y, z)) {
                throw new ArgumentException("Invalid variable value.");
            } else {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            float[] data = new float[3]; /* X, Y, Z */
            Marshal.Copy(pointer, data, 0, data.Length);

            if (IsValid(data[0], data[1], data[2]))
            {
                bool changed = (data[0] != x)
                            || (data[1] != y)
                            || (data[2] != z);

                x = data[0];
                y = data[1];
                z = data[2];

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
            float[] data = new float[] { X, Y, Z };
            Marshal.Copy(data, 0, pointer, data.Length);
        }

        #region Customization

        /// <summary>
        /// Gets or sets this variable's arrow color.
        /// </summary>
        public Color ArrowColor
        {
            get { return Tw.GetColorParam(ParentBar.Pointer, ID, "arrowcolor"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "arrowcolor", value); }
        }

        /// <summary>
        /// Shows or hides this variable's numerical value.
        /// </summary>
        public Boolean ShowValue
        {
            get { return Tw.GetBooleanParam(ParentBar.Pointer, ID, "showval"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "showval", value); }
        }

        // For whatever reason SetParam doesn't appear to work
        // with axisx/axisy/axisz, so we cache the value here.
        private CoordinateSystem coordinates = new CoordinateSystem(AxisOrientation.PositiveX,
                                                                    AxisOrientation.PositiveY,
                                                                    AxisOrientation.PositiveZ);

        /// <summary>
        /// Gets or sets this variable's coordinate system.
        /// </summary>
        public CoordinateSystem Coordinates
        {
            get { return coordinates; }
            set
            {
                Tw.SetParam(ParentBar.Pointer, ID, "axisx", value.GetAxis(0));
                Tw.SetParam(ParentBar.Pointer, ID, "axisy", value.GetAxis(1));
                Tw.SetParam(ParentBar.Pointer, ID, "axisz", value.GetAxis(2));
                this.coordinates = value;
            }
        }

        #endregion

        public override String ToString()
        {
            return String.Format("[VectorVariable: Label={0}, Value=({1}, {2}, {3})]", Label, X, Y, Z);
        }
    }
}
