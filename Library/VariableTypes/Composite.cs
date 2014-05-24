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
    public sealed class ColorVariable<T> : Variable<T> where T : ColorType
    {
        public ColorVariable(Bar bar, T initialValue = default(T), String def = null)
            : base(bar, initialValue, def)
        {

        }

        protected override bool Validate(T newValue)
        {
            return (0 <= newValue.R && newValue.R <= 1)
                && (0 <= newValue.G && newValue.G <= 1)
                && (0 <= newValue.B && newValue.B <= 1);
        }

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
    }

    /// <summary>
    /// A variable holding a 3D vector variable.
    /// </summary>
    public sealed class VectorVariable<T> : Variable<T> where T : VectorType
    {
        public VectorVariable(Bar bar, T initialValue = default(T), String def = null)
            : base(bar, initialValue, def)
        {

        }

        protected override bool Validate(T newValue)
        {
            return true; // no restrictions
        }

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
    }

    /// <summary>
    /// A variable holding a quaternion variable.
    /// </summary>
    public sealed class QuaternionVariable<T> : Variable<T> where T : QuaternionType
    {
        public QuaternionVariable(Bar bar, T initialValue = default(T), String def = null)
            : base(bar, initialValue, def)
        {

        }

        protected override bool Validate(T newValue)
        {
            return true; // no restrictions
        }

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
    }
}

