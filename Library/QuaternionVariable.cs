using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
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
        /// Raises the Changed event.
        /// </summary>
        private void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        /// <summary>
        /// Gets or sets this quaternion's X-component.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets this quaternion's Y-component.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets this quaternion's Z-component.
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Gets or sets the W-component.
        /// </summary>
        public float W { get; set; }

        /// <summary>
        /// Initialization delegate, which creates the quaternion variable.
        /// </summary>
        private static void InitQuaternionVariable(Variable var, String id)
        {
            TW.AddVarCB(var.ParentBar.Pointer, id,
                        TW.VariableType.Quat4F,
                        ((QuaternionVariable)var).SetCallback,
                        ((QuaternionVariable)var).GetCallback,
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
            setCallback = SetCallback;
            getCallback = GetCallback;
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private readonly TW.SetVarCallback setCallback;
        private void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            float[] data = new float[4]; /* X, Y, Z, W */
            Marshal.Copy(pointer, data, 0, data.Length);

            bool changed = (data[0] != X)
                        || (data[1] != Y)
                        || (data[2] != Z)
                        || (data[3] != W);

            X = data[0];
            Y = data[1];
            Z = data[2];
            W = data[3];

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private readonly TW.GetVarCallback getCallback;
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
            get { return TW.GetColorParam(ParentBar.Pointer, ID, "arrowcolor"); }
            set { TW.SetParam(ParentBar.Pointer, ID, "arrowcolor", value); }
        }

        /// <summary>
        /// Shows or hides this variable's numerical value.
        /// </summary>
        public Boolean ShowValue
        {
            get { return TW.GetBooleanParam(ParentBar.Pointer, ID, "showval"); }
            set { TW.SetParam(ParentBar.Pointer, ID, "showval", value); }
        }

        private CoordinateSystem coordinates =
            new CoordinateSystem(CoordinateSystem.Axis.PositiveX,
                                 CoordinateSystem.Axis.PositiveY,
                                 CoordinateSystem.Axis.PositiveZ);

        /// <summary>
        /// Gets or sets this variable's coordinate system.
        /// </summary>
        public CoordinateSystem Coordinates
        {
            get { return coordinates; }
            set
            {
                TW.SetParam(ParentBar.Pointer, ID, "axisx", value.GetAxis(0));
                TW.SetParam(ParentBar.Pointer, ID, "axisy", value.GetAxis(1));
                TW.SetParam(ParentBar.Pointer, ID, "axisz", value.GetAxis(2));
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