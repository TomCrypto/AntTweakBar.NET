using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

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
    /// Interface to implement for RGB colors.
    /// </summary>
    public interface ColorType
    {
        float R { get; }
        float G { get; }
        float B { get; }
    }

    /// <summary>
    /// Interface to implement for RGBA colors.
    /// </summary>
    public interface Color4Type
    {
        float R { get; }
        float G { get; }
        float B { get; }
        float A { get; }
    }

    /// <summary>
    /// Interface to implement for 3D vectors.
    /// </summary>
    public interface VectorType
    {
        float X { get; }
        float Y { get; }
        float Z { get; }
    }

    /// <summary>
    /// Interface to implement for quaternions.
    /// </summary>
    public interface QuaternionType
    {
        float X { get; }
        float Y { get; }
        float Z { get; }
        float W { get; }
    }

    /// <summary>
    /// A variable holding an RGB color value.
    /// </summary>
    public class ColorVariable<T> : Variable where T : ColorType
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

        private T value;

        #endregion

        public ColorVariable(Bar bar, T initialValue = default(T), String def = null)
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
            Value = initialValue;
        }

        /// <summary>
        /// Gets or sets the value of this variable.
        /// </summary>
        public T Value
        {
            get { return value; }
            set
            {
                if (!(0 <= value.R && value.R <= 1)
                    && !(0 <= value.G && value.G <= 1)
                    && !(0 <= value.B && value.B <= 1))
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value");
                else
                {
                    bool changed = !value.Equals(this.value);
                    this.value = value;
                    if (changed)
                        OnChanged(EventArgs.Empty);
                }
            }
        }

        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            Value = Helper.Create<T>(((float*)pointer)[0],
                                     ((float*)pointer)[1],
                                     ((float*)pointer)[2]);
        }

        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            ((float*)pointer)[0] = Value.R;
            ((float*)pointer)[1] = Value.G;
            ((float*)pointer)[2] = Value.B;
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
    public class Color4Variable<T> : Variable where T : Color4Type
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

        private T value;

        #endregion

        public Color4Variable(Bar bar, T initialValue = default(T), String def = null)
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
            Value = initialValue;
        }

        /// <summary>
        /// Gets or sets the value of this variable.
        /// </summary>
        public T Value
        {
            get { return value; }
            set
            {
                if (!(0 <= value.R && value.R <= 1)
                 && !(0 <= value.G && value.G <= 1)
                 && !(0 <= value.B && value.B <= 1)
                 && !(0 <= value.A && value.A <= 1))
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value");
                else
                {
                    bool changed = !value.Equals(this.value);
                    this.value = value;
                    if (changed)
                        OnChanged(EventArgs.Empty);
                }
            }
        }

        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            Value = Helper.Create<T>(((float*)pointer)[0],
                                     ((float*)pointer)[1],
                                     ((float*)pointer)[2],
                                     ((float*)pointer)[3]);
        }

        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            ((float*)pointer)[0] = Value.R;
            ((float*)pointer)[1] = Value.G;
            ((float*)pointer)[2] = Value.B;
            ((float*)pointer)[3] = Value.A;
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
    public sealed class VectorVariable<T> : Variable where T : VectorType
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

        public VectorVariable(Bar bar, T initialValue = default(T), String def = null)
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
            Value = initialValue;
        }

        /// <summary>
        /// Gets or sets the value of this variable.
        /// </summary>
        public T Value { get; set; }

        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            Value = Helper.Create<T>(((float*)pointer)[0],
                                     ((float*)pointer)[1],
                                     ((float*)pointer)[2]);
            OnChanged(EventArgs.Empty);
        }

        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            ((float*)pointer)[0] = Value.X;
            ((float*)pointer)[1] = Value.Y;
            ((float*)pointer)[2] = Value.Z;
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

        #endregion
    }

    /// <summary>
    /// A variable holding a quaternion variable.
    /// </summary>
    public sealed class QuaternionVariable<T> : Variable where T : QuaternionType
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

        public QuaternionVariable(Bar bar, T initialValue = default(T), String def = null)
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
            Value = initialValue;
        }

        /// <summary>
        /// Gets or sets the value of this variable.
        /// </summary>
        public T Value { get; set; }

        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            Value = Helper.Create<T>(((float*)pointer)[0],
                                     ((float*)pointer)[1],
                                     ((float*)pointer)[2],
                                     ((float*)pointer)[3]);
            OnChanged(EventArgs.Empty);
        }

        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            ((float*)pointer)[0] = Value.X;
            ((float*)pointer)[1] = Value.Y;
            ((float*)pointer)[2] = Value.Z;
            ((float*)pointer)[3] = Value.W;
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

        #endregion
    }
}

