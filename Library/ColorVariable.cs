using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    /// <summary>
    /// An AntTweakBar variable which can hold an RGB color value.
    /// </summary>
    public sealed class ColorVariable : Variable
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
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value.");
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
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value.");
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
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value.");
                else
                    b = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float r;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float g;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float b;

        /// <summary>
        /// Initialization delegate, which creates the RGB color variable.
        /// </summary>
        private static void InitColorVariable(Variable var, String id)
        {
            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        Tw.VariableType.Color3F,
                        ((ColorVariable)var).SetCallback,
                        ((ColorVariable)var).GetCallback,
                        IntPtr.Zero, null);
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
        private readonly Tw.SetVarCallback setCallback;
        private void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            float[] data = new float[3]; /* R, G, B */
            Marshal.Copy(pointer, data, 0, data.Length);

            bool changed = (data[0] != R)
                        || (data[1] != G)
                        || (data[2] != B);

            R = data[0];
            G = data[1];
            B = data[2];

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private readonly Tw.GetVarCallback getCallback;
        private void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            float[] data = new float[] { R, G, B };
            Marshal.Copy(data, 0, pointer, data.Length);
        }

        #region Customization

        /// <summary>
        /// Gets or sets this variable's color selection mode.
        /// </summary>
        public ColorMode Mode
        {
            get
            {
                switch (Tw.GetStringParam(ParentBar.Pointer, ID, "colormode"))
                {
                    case "rgb":
                        return ColorMode.RGB;
                    case "hls":
                        return ColorMode.HLS;
                    default:
                        throw new InvalidOperationException("Invalid color mode.");
                }
            }
            set
            {
                switch (value)
                {
                    case ColorMode.RGB:
                        Tw.SetParam(ParentBar.Pointer, ID, "colormode", "rgb");
                        break;
                    case ColorMode.HLS:
                        Tw.SetParam(ParentBar.Pointer, ID, "colormode", "hls");
                        break;
                    default:
                        throw new ArgumentException("Invalid color mode.");
                }
            }
        }

        #endregion

        public override String ToString()
        {
            return String.Format("[ColorVariable: Label={0}, Value=({1}, {2}, {3})]", Label, R, G, B);
        }
    }
}
