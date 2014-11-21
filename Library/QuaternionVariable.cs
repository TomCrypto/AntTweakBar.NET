using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    public sealed class QuaternionValidationEventArgs : EventArgs
    {
        /// <summary>
        /// Whether to accept this quaternion value.
        /// </summary>
        public bool Valid { get; set; }
        public readonly float X;
        public readonly float Y;
        public readonly float Z;
        public readonly float W;

        public QuaternionValidationEventArgs(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }

    /// <summary>
    /// An AntTweakBar variable which can hold a quaternion.
    /// </summary>
    public sealed class QuaternionVariable : Variable
    {
        /// <summary>
        /// Occurs when the user changes this variable's value.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Occurs when the new value of this variable is validated.
        /// </summary>
        public event EventHandler<QuaternionValidationEventArgs> Validating;

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
        /// Gets or sets this quaternion's X-component.
        /// </summary>
        public float X
        {
            get { ThrowIfDisposed(); return x; }
            set { ValidateAndSet(value, Y, Z, W); }
        }

        /// <summary>
        /// Gets or sets this quaternion's Y-component.
        /// </summary>
        public float Y
        {
            get { ThrowIfDisposed(); return y; }
            set { ValidateAndSet(X, value, Z, W); }
        }

        /// <summary>
        /// Gets or sets this quaternion's Z-component.
        /// </summary>
        public float Z
        {
            get { ThrowIfDisposed(); return z; }
            set { ValidateAndSet(X, Y, value, W); }
        }

        /// <summary>
        /// Gets or sets this quaternion's W-component.
        /// </summary>
        public float W
        {
            get { ThrowIfDisposed(); return w; }
            set { ValidateAndSet(X, Y, Z, value); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float x;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float y;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float z;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float w;

        /// <summary>
        /// Initialization delegate, which creates the quaternion variable.
        /// </summary>
        private static void InitQuaternionVariable(Variable var, String id)
        {
            var it = var as QuaternionVariable;

            Variable.SetCallbacks.Add(id, new Tw.SetVarCallback(it.SetCallback));
            Variable.GetCallbacks.Add(id, new Tw.GetVarCallback(it.GetCallback));

            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        Tw.VariableType.Quat4F,
                        Variable.SetCallbacks[id],
                        Variable.GetCallbacks[id],
                        IntPtr.Zero, null);
        }

        /// <summary>
        /// Creates a new quaternion variable in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the quaternion variable in.</param>
        /// <param name="x">The initial X-component value of the variable.</param>
        /// <param name="y">The initial Y-component value of the variable.</param>
        /// <param name="z">The initial Z-component value of the variable.</param>
        /// <param name="w">The initial W-component value of the variable.</param>
        /// <param name="def">An optional definition string for the new variable.</param>
        public QuaternionVariable(Bar bar, float x = 0, float y = 0, float z = 0, float w = 1, String def = null)
            : base(bar, InitQuaternionVariable, def)
        {
            Validating += (s, e) => { e.Valid = true; };

            ValidateAndSet(x, y, z, w);
        }

        /// <summary>
        /// Checks if this variable can hold this value.
        /// </summary>
        private bool IsValid(float x, float y, float z, float w)
        {
            ThrowIfDisposed();

            return !Validating.GetInvocationList().Select(h => {
                var check = new QuaternionValidationEventArgs(x, y, z, w);
                h.DynamicInvoke(new object[] { this, check });
                return !check.Valid;
            }).Any(failed => failed);
        }

        /// <summary>
        /// Tries to set this variable's value, validating it.
        /// </summary>
        private void ValidateAndSet(float x, float y, float z, float w)
        {
            if (!IsValid(x, y, z, w)) {
                throw new ArgumentException("Invalid variable value.");
            } else {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            float[] data = new float[4]; /* X, Y, Z, W */
            Marshal.Copy(pointer, data, 0, data.Length);

            if (IsValid(data[0], data[1], data[2], data[3]))
            {
                bool changed = (data[0] != x)
                            || (data[1] != y)
                            || (data[2] != z)
                            || (data[3] != w);

                x = data[0];
                y = data[1];
                z = data[2];
                w = data[3];

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
            float[] data = new float[] { X, Y, Z, W };
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
            return String.Format("[QuaternionVariable: Label={0}, Value=({1}, {2}, {3}, {4})]", Label, X, Y, Z, W);
        }
    }
}
