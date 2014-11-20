using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
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
        /// Raises the Changed event.
        /// </summary>
        public void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        /// <summary>
        /// Gets or sets this vector's X-component.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets this vector's Y-component.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets this vector's Z-component.
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Initialization delegate, which creates the vector variable.
        /// </summary>
        private static void InitVectorVariable(Variable var, String id)
        {
            var it = var as VectorVariable;

            Tw.SetCallbacks.Add(id, new Tw.SetVarCallback(it.SetCallback));
            Tw.GetCallbacks.Add(id, new Tw.GetVarCallback(it.GetCallback));

            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        Tw.VariableType.Dir3F,
                        Tw.SetCallbacks[id],
                        Tw.GetCallbacks[id],
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
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            float[] data = new float[3]; /* X, Y, Z */
            Marshal.Copy(pointer, data, 0, data.Length);

            bool changed = (data[0] != X)
                        || (data[1] != Y)
                        || (data[2] != Z);

            X = data[0];
            Y = data[1];
            Z = data[2];

            if (changed)
                OnChanged(EventArgs.Empty);
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
        private CoordinateSystem coordinates = 
            new CoordinateSystem(AxisOrientation.PositiveX,
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
