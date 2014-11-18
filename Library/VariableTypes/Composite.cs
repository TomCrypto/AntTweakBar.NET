using System;
using System.Drawing;

namespace AntTweakBar
{
    /// <summary>
    /// The possible color selection modes.
    /// </summary>
    public enum ColorMode
    {
        /// <summary>
        /// RGB color selection.
        /// </summary>
        RGB,

        /// <summary>
        /// HLS color selection.
        /// </summary>
        HLS
    }

    /// <summary>
    /// An AntTweakBar variable which can hold an RGB color value.
    /// </summary>
    public class ColorVariable : Variable
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
        /// Gets or sets this color's red channel component.
        /// </summary>
        public float R
        {
            get { return r; }
            set
            {
                if (!(0 <= value && value <= 1))
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value");
                else
                    r = value;
            }
        }

        /// <summary>
        /// Gets or sets this color's green channel component.
        /// </summary>
        public float G
        {
            get { return g; }
            set
            {
                if (!(0 <= value && value <= 1))
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value");
                else
                    g = value;
            }
        }

        /// <summary>
        /// Gets or sets this color's blue channel component.
        /// </summary>
        public float B
        {
            get { return b; }
            set
            {
                if (!(0 <= value && value <= 1))
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value");
                else
                    b = value;
            }
        }

        private float r, g, b;

        /// <summary>
        /// Initialization delegate, which creates the RGB color variable.
        /// </summary>
        private static void InitColorVariable(Variable var, String id)
        {
            TW.AddVarCB(var.ParentBar.Pointer, id,
                        TW.VariableType.TW_TYPE_COLOR3F,
                        ((ColorVariable)var).SetCallback,
                        ((ColorVariable)var).GetCallback,
                        IntPtr.Zero);
        }

        /// <summary>
        /// Creates a new RGB color variable in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the RGB color variable in.</param>
        /// <param name="r">The initial red channel value of the variable.</param>
        /// <param name="g">The initial green channel value of the variable.</param>
        /// <param name="b">The initial blue channel value of the variable.</param>
        /// <param name="def">An optional definition string for the new variable.</param>
        public ColorVariable(Bar bar, float r = 0, float g = 0, float b = 0, String def = null)
            : base(bar, InitColorVariable, def)
        {
            setCallback = SetCallback;
            getCallback = GetCallback;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private readonly TW.SetVarCallback setCallback;
        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            float tr = ((float*)pointer)[0];
            float tg = ((float*)pointer)[1];
            float tb = ((float*)pointer)[2];

            bool changed = (tr != R) || (tg != G) || (tb != B);

            R = tr;
            G = tg;
            B = tb;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private readonly TW.GetVarCallback getCallback;
        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            ((float*)pointer)[0] = R;
            ((float*)pointer)[1] = G;
            ((float*)pointer)[2] = B;
        }

        #region Customization

        /// <summary>
        /// Gets or sets this variable's color selection mode.
        /// </summary>
        public ColorMode Mode
        {
            get
            {
                switch (TW.GetStringParam(ParentBar.Pointer, ID, "colormode"))
                {
                    case "rgb":
                        return ColorMode.RGB;
                    case "hls":
                        return ColorMode.HLS;
                    default:
                        throw new InvalidOperationException("Invalid color mode"); // should not happen
                }
            }
            set
            {
                switch (value)
                {
                    case ColorMode.RGB:
                        TW.SetParam(ParentBar.Pointer, ID, "colormode", "rgb");
                        break;
                    case ColorMode.HLS:
                        TW.SetParam(ParentBar.Pointer, ID, "colormode", "hls");
                        break;
                    default:
                        throw new ArgumentException("Invalid color mode");
                }
            }
        }

        #endregion

        #region Misc.

        public override String ToString()
        {
            return String.Format("[ColorVariable: Label={0}, Value=({1}, {2}, {3})]", Label, R, G, B);
        }

        #endregion
    }

    /// <summary>
    /// An AntTweakBar variable which can hold an RGBA color value.
    /// </summary>
    public class Color4Variable : Variable
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
        /// Gets or sets this color's red channel component.
        /// </summary>
        public float R
        {
            get { return r; }
            set
            {
                if (!(0 <= value && value <= 1))
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value");
                else
                    r = value;
            }
        }

        /// <summary>
        /// Gets or sets this color's green channel component.
        /// </summary>
        public float G
        {
            get { return g; }
            set
            {
                if (!(0 <= value && value <= 1))
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value");
                else
                    g = value;
            }
        }

        /// <summary>
        /// Gets or sets this color's blue channel component.
        /// </summary>
        public float B
        {
            get { return b; }
            set
            {
                if (!(0 <= value && value <= 1))
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value");
                else
                    b = value;
            }
        }

        /// <summary>
        /// Gets or sets this color's alpha channel component.
        /// </summary>
        public float A
        {
            get { return a; }
            set
            {
                if (!(0 <= value && value <= 1))
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value");
                else
                    a = value;
            }
        }

        private float r, g, b, a;

        /// <summary>
        /// Initialization delegate, which creates the RGBA color variable.
        /// </summary>
        private static void InitColor4Variable(Variable var, String id)
        {
            TW.AddVarCB(var.ParentBar.Pointer, id,
                        TW.VariableType.TW_TYPE_COLOR4F,
                        ((Color4Variable)var).SetCallback,
                        ((Color4Variable)var).GetCallback,
                        IntPtr.Zero);
        }

        /// <summary>
        /// Creates a new RGBA color variable in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the RGBA color variable in.</param>
        /// <param name="r">The initial red channel value of the variable.</param>
        /// <param name="g">The initial green channel value of the variable.</param>
        /// <param name="b">The initial blue channel value of the variable.</param>
        /// <param name="a">The initial alpha channel value of the variable.</param>
        /// <param name="def">An optional definition string for the new variable.</param>
        public Color4Variable(Bar bar, float r = 0, float g = 0, float b = 0, float a = 0, String def = null)
            : base(bar, InitColor4Variable, def)
        {
            setCallback = SetCallback;
            getCallback = GetCallback;
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private readonly TW.SetVarCallback setCallback;
        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            float tr = ((float*)pointer)[0];
            float tg = ((float*)pointer)[1];
            float tb = ((float*)pointer)[2];
            float ta = ((float*)pointer)[3];

            bool changed = (tr != R) || (tg != G) || (tb != B) || (ta != A);

            R = tr;
            G = tg;
            B = tb;
            A = ta;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private readonly TW.GetVarCallback getCallback;
        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            ((float*)pointer)[0] = R;
            ((float*)pointer)[1] = G;
            ((float*)pointer)[2] = B;
            ((float*)pointer)[3] = A;
        }

        #region Customization

        /// <summary>
        /// Gets or sets this variable's color selection mode.
        /// </summary>
        public ColorMode Mode
        {
            get
            {
                switch (TW.GetStringParam(ParentBar.Pointer, ID, "colormode"))
                {
                    case "rgb":
                        return ColorMode.RGB;
                    case "hls":
                        return ColorMode.HLS;
                    default:
                        throw new InvalidOperationException("Invalid color mode"); // should not happen
                }
            }
            set
            {
                switch (value)
                {
                    case ColorMode.RGB:
                        TW.SetParam(ParentBar.Pointer, ID, "colormode", "rgb");
                        break;
                    case ColorMode.HLS:
                        TW.SetParam(ParentBar.Pointer, ID, "colormode", "hls");
                        break;
                    default:
                        throw new ArgumentException("Invalid color mode");
                }
            }
        }

        #endregion

        #region Misc.

        public override String ToString()
        {
            return String.Format("[Color4Variable: Label={0}, Value=({1}, {2}, {3}, {4})]", Label, R, G, B, A);
        }

        #endregion
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
        /// Raises the Changed event.
        /// </summary>
        private void OnChanged(EventArgs e)
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
            TW.AddVarCB(var.ParentBar.Pointer, id,
                        TW.VariableType.TW_TYPE_DIR3F,
                        ((VectorVariable)var).SetCallback,
                        ((VectorVariable)var).GetCallback,
                        IntPtr.Zero);
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
            setCallback = SetCallback;
            getCallback = GetCallback;
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private readonly TW.SetVarCallback setCallback;
        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            float tx = ((float*)pointer)[0];
            float ty = ((float*)pointer)[1];
            float tz = ((float*)pointer)[2];

            bool changed = (tx != X) || (ty != Y) || (tz != Z);

            X = tx;
            Y = ty;
            Z = tz;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private readonly TW.GetVarCallback getCallback;
        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            ((float*)pointer)[0] = X;
            ((float*)pointer)[1] = Y;
            ((float*)pointer)[2] = Z;
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

        // For whatever reason SetParam doesn't appear to work
        // with axisx/axisy/axisz, so we cache the value here.
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

        #region Misc.

        public override String ToString()
        {
            return String.Format("[VectorVariable: Label={0}, Value=({1}, {2}, {3})]", Label, X, Y, Z);
        }

        #endregion
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
                        TW.VariableType.TW_TYPE_QUAT4F,
                        ((QuaternionVariable)var).SetCallback,
                        ((QuaternionVariable)var).GetCallback,
                        IntPtr.Zero);
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
        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            float tx = ((float*)pointer)[0];
            float ty = ((float*)pointer)[1];
            float tz = ((float*)pointer)[2];
            float tw = ((float*)pointer)[3];

            bool changed = (tx != X) || (ty != Y) || (tz != Z) || (tw != W);

            X = tx;
            Y = ty;
            Z = tz;
            W = tw;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private readonly TW.GetVarCallback getCallback;
        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            ((float*)pointer)[0] = X;
            ((float*)pointer)[1] = Y;
            ((float*)pointer)[2] = Z;
            ((float*)pointer)[3] = W;
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

        #region Misc.

        public override String ToString()
        {
            return String.Format("[QuaternionVariable: Label={0}, Value=({1}, {2}, {3}, {4})]", Label, X, Y, Z, W);
        }

        #endregion
    }
}

