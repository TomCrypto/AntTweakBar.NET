using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace AntTweakBar
{
    /// <summary>
    /// An axis-aligned coordinate system.
    /// </summary>
    public struct CoordinateSystem : IEquatable<CoordinateSystem>
    {
        /// <summary>
        /// The different axis types.
        /// </summary>
        public enum Axis
        {
            /// <summary>
            /// Right axis.
            /// </summary>
            PositiveX,
            /// <summary>
            /// Left axis.
            /// </summary>
            NegativeX,
            /// <summary>
            /// Upwards axis.
            /// </summary>
            PositiveY,
            /// <summary>
            /// Downwards axis.
            /// </summary>
            NegativeY,
            /// <summary>
            /// Front axis (towards the viewer).
            /// </summary>
            PositiveZ,
            /// <summary>
            /// Back axis (into the screen).
            /// </summary>
            NegativeZ
        }

        public readonly Axis axisX;
        public readonly Axis axisY;
        public readonly Axis axisZ;

        public CoordinateSystem(Axis axisX, Axis axisY, Axis axisZ)
        {
            if (!((axisX == Axis.PositiveX) || (axisX == Axis.NegativeX)
               || (axisY == Axis.PositiveX) || (axisY == Axis.NegativeX)
               || (axisZ == Axis.PositiveX) || (axisZ == Axis.NegativeX)))
                throw new ArgumentException("Invalid coordinate system");

            if (!((axisX == Axis.PositiveY) || (axisX == Axis.NegativeY)
               || (axisY == Axis.PositiveY) || (axisY == Axis.NegativeY)
               || (axisZ == Axis.PositiveY) || (axisZ == Axis.NegativeY)))
                throw new ArgumentException("Invalid coordinate system");

            if (!((axisX == Axis.PositiveZ) || (axisX == Axis.NegativeZ)
               || (axisY == Axis.PositiveZ) || (axisY == Axis.NegativeZ)
               || (axisZ == Axis.PositiveZ) || (axisZ == Axis.NegativeZ)))
                throw new ArgumentException("Invalid coordinate system");

            this.axisX = axisX;
            this.axisY = axisY;
            this.axisZ = axisZ;
        }

        private static String Convert(Axis axis)
        {
            switch (axis)
            {
                case Axis.PositiveX: return "x";
                case Axis.NegativeX: return "-x";
                case Axis.PositiveY: return "y";
                case Axis.NegativeY: return "-y";
                case Axis.PositiveZ: return "z";
                case Axis.NegativeZ: return "-z";
                default: throw new ArgumentException();
            }
        }

        internal String GetAxis(int t)
        {
            switch (t)
            {
                case 0: return Convert(axisX);
                case 1: return Convert(axisY);
                case 2: return Convert(axisZ);
                default: throw new ArgumentException();
            }
        }

        public bool Equals(CoordinateSystem other)
        {
            return (this.axisX == other.axisX)
                && (this.axisY == other.axisY)
                && (this.axisZ == other.axisZ);
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is CoordinateSystem))
                return false;

            return Equals((CoordinateSystem)obj);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 67 + axisX.GetHashCode();
            hash = hash * 67 + axisY.GetHashCode();
            hash = hash * 67 + axisZ.GetHashCode();
            return hash;
        }
    }

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
    /// A variable holding an RGB color value.
    /// </summary>
    public class ColorVariable : Variable
    {
        #region Fields

        private readonly TW.GetVarCallback getCallback;
        private readonly TW.SetVarCallback setCallback;

        /// <summary>
        /// Occurs when the user changes the variable.
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

		private float r, g, b;

        #endregion

        public ColorVariable(Bar bar, float r = 0, float g = 0, float b = 0, String def = null)
            : base(bar)
        {
            setCallback = SetCallback;
            getCallback = GetCallback;

            TW.SetCurrentWindow(bar.Owner.WindowIndex);
            TW.AddVarCB(Owner, ID, TW.VariableType.TW_TYPE_COLOR3F,
                        setCallback, getCallback, IntPtr.Zero);

            Owner.Add(this);
            Label = "undef";
            if (def != null)
                SetDefinition(def);
			R = r;
			G = g;
			B = b;
        }

		/// <summary>
		/// Gets or sets the red channel component.
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
		/// Gets or sets the green channel component.
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
		/// Gets or sets the blue channel component.
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

        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            ((float*)pointer)[0] = R;
            ((float*)pointer)[1] = G;
            ((float*)pointer)[2] = B;
        }

        #region Customization

        /// <summary>
        /// Gets or sets the color selection mode.
        /// </summary>
        public ColorMode Mode
        {
            get
            {
                switch (TW.GetStringParam(Owner, ID, "colormode"))
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
                        TW.SetParam(Owner, ID, "colormode", "rgb");
                        break;
                    case ColorMode.HLS:
                        TW.SetParam(Owner, ID, "colormode", "hls");
                        break;
                    default:
                        throw new ArgumentException("Invalid color mode");
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// A variable holding an RGBA color value.
    /// </summary>
    public class Color4Variable : Variable
    {
        #region Fields

        private readonly TW.GetVarCallback getCallback;
        private readonly TW.SetVarCallback setCallback;

        /// <summary>
        /// Occurs when the user changes the variable.
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

        private float r, g, b, a;

        #endregion

        public Color4Variable(Bar bar, float r = 0, float g = 0, float b = 0, float a = 0, String def = null)
            : base(bar)
        {
            setCallback = SetCallback;
            getCallback = GetCallback;

            TW.SetCurrentWindow(bar.Owner.WindowIndex);
            TW.AddVarCB(Owner, ID, TW.VariableType.TW_TYPE_COLOR4F,
                        setCallback, getCallback, IntPtr.Zero);

            Owner.Add(this);
            Label = "undef";
            if (def != null)
                SetDefinition(def);
			R = r;
			G = g;
			B = b;
			A = a;
        }

		/// <summary>
		/// Gets or sets the red channel component.
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
		/// Gets or sets the green channel component.
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
		/// Gets or sets the blue channel component.
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
		/// Gets or sets the alpha channel component.
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

        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            ((float*)pointer)[0] = R;
            ((float*)pointer)[1] = G;
            ((float*)pointer)[2] = B;
            ((float*)pointer)[3] = A;
        }

        #region Customization

        /// <summary>
        /// Gets or sets the color selection mode.
        /// </summary>
        public ColorMode Mode
        {
            get
            {
                switch (TW.GetStringParam(Owner, ID, "colormode"))
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
                        TW.SetParam(Owner, ID, "colormode", "rgb");
                        break;
                    case ColorMode.HLS:
                        TW.SetParam(Owner, ID, "colormode", "hls");
                        break;
                    default:
                        throw new ArgumentException("Invalid color mode");
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// A variable holding a 3D vector variable.
    /// </summary>
    public sealed class VectorVariable : Variable
    {
        #region Fields

        private readonly TW.GetVarCallback getCallback;
        private readonly TW.SetVarCallback setCallback;

        /// <summary>
        /// Occurs when the user changes the variable.
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

        #endregion

        public VectorVariable(Bar bar, float x = 0, float y = 0, float z = 0, String def = null)
            : base(bar)
        {
            setCallback = SetCallback;
            getCallback = GetCallback;

            TW.SetCurrentWindow(bar.Owner.WindowIndex);
            TW.AddVarCB(Owner, ID, TW.VariableType.TW_TYPE_DIR3F,
                        setCallback, getCallback, IntPtr.Zero);

            Owner.Add(this);
            Label = "undef";
            if (def != null)
                SetDefinition(def);
			X = x;
			Y = y;
			Z = z;
        }

		/// <summary>
		/// Gets or sets the X-component.
		/// </summary>
		public float X { get; set; }

		/// <summary>
		/// Gets or sets the Y-component.
		/// </summary>
		public float Y { get; set; }

		/// <summary>
		/// Gets or sets the Z-component.
		/// </summary>
		public float Z { get; set; }

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

        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            ((float*)pointer)[0] = X;
            ((float*)pointer)[1] = Y;
            ((float*)pointer)[2] = Z;
        }

        #region Customization

        /// <summary>
        /// Gets or sets the arrow color of this variable.
        /// </summary>
        public Color ArrowColor
        {
            get { return TW.GetColorParam(Owner, ID, "arrowcolor"); }
            set { TW.SetParam(Owner, ID, "arrowcolor", value); }
        }

        /// <summary>
        /// Shows or hides the numerical value of the vector.
        /// </summary>
        public Boolean ShowValue
        {
            get { return TW.GetBooleanParam(Owner, ID, "showval"); }
            set { TW.SetParam(Owner, ID, "showval", value); }
        }

        // For whatever reason SetParam doesn't appear to work
        // with axisx/axisy/axisz, so we cache the value here.
        private CoordinateSystem coordinates = 
            new CoordinateSystem(CoordinateSystem.Axis.PositiveX,
                                 CoordinateSystem.Axis.PositiveY,
                                 CoordinateSystem.Axis.PositiveZ);

        /// <summary>
        /// Gets or sets the vector's coordinate system.
        /// </summary>
        public CoordinateSystem Coordinates
        {
            get { return coordinates; }
            set
            {
                TW.SetParam(Owner, ID, "axisx", value.GetAxis(0));
                TW.SetParam(Owner, ID, "axisy", value.GetAxis(1));
                TW.SetParam(Owner, ID, "axisz", value.GetAxis(2));
                this.coordinates = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// A variable holding a quaternion variable.
    /// </summary>
    public sealed class QuaternionVariable : Variable
    {
        #region Fields

        private readonly TW.GetVarCallback getCallback;
        private readonly TW.SetVarCallback setCallback;

        /// <summary>
        /// Occurs when the user changes the variable.
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

        #endregion

        public QuaternionVariable(Bar bar, float x = 0, float y = 0, float z = 0, float w = 0, String def = null)
            : base(bar)
        {
            setCallback = SetCallback;
            getCallback = GetCallback;

            TW.SetCurrentWindow(bar.Owner.WindowIndex);
            TW.AddVarCB(Owner, ID, TW.VariableType.TW_TYPE_QUAT4F,
                        setCallback, getCallback, IntPtr.Zero);

            Owner.Add(this);
            Label = "undef";
            if (def != null)
                SetDefinition(def);
			X = x;
			Y = y;
			Z = z;
			W = w;
        }

		/// <summary>
		/// Gets or sets the X-component.
		/// </summary>
		public float X { get; set; }

		/// <summary>
		/// Gets or sets the Y-component.
		/// </summary>
		public float Y { get; set; }

		/// <summary>
		/// Gets or sets the Z-component.
		/// </summary>
		public float Z { get; set; }

		/// <summary>
		/// Gets or sets the W-component.
		/// </summary>
		public float W { get; set; }

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

        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            ((float*)pointer)[0] = X;
            ((float*)pointer)[1] = Y;
            ((float*)pointer)[2] = Z;
            ((float*)pointer)[3] = W;
        }

        #region Customization

        /// <summary>
        /// Gets or sets the arrow color of this variable.
        /// </summary>
        public Color ArrowColor
        {
            get { return TW.GetColorParam(Owner, ID, "arrowcolor"); }
            set { TW.SetParam(Owner, ID, "arrowcolor", value); }
        }

        /// <summary>
        /// Shows or hides the numerical value of the quaternion.
        /// </summary>
        public Boolean ShowValue
        {
            get { return TW.GetBooleanParam(Owner, ID, "showval"); }
            set { TW.SetParam(Owner, ID, "showval", value); }
        }

        private CoordinateSystem coordinates =
            new CoordinateSystem(CoordinateSystem.Axis.PositiveX,
                                 CoordinateSystem.Axis.PositiveY,
                                 CoordinateSystem.Axis.PositiveZ);

        /// <summary>
        /// Gets or sets the quaternion's coordinate system.
        /// </summary>
        public CoordinateSystem Coordinates
        {
            get { return coordinates; }
            set
            {
                TW.SetParam(Owner, ID, "axisx", value.GetAxis(0));
                TW.SetParam(Owner, ID, "axisy", value.GetAxis(1));
                TW.SetParam(Owner, ID, "axisz", value.GetAxis(2));
                this.coordinates = value;
            }
        }

        #endregion
    }
}

