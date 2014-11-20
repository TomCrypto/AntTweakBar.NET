using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    /// <summary>
    /// An AntTweakBar variable which can hold an RGBA color value.
    /// </summary>
    public sealed class Color4Variable : Variable
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

        /// <summary>
        /// Gets or sets this color's alpha channel component.
        /// </summary>
        public float A
        {
            get { return a; }
            set
            {
                if (!(0 <= value && value <= 1))
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value.");
                else
                    a = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float r;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float g;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float b;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float a;

        /// <summary>
        /// Initialization delegate, which creates the RGBA color variable.
        /// </summary>
        private static void InitColor4Variable(Variable _var, String id)
        {
            var var = _var as Color4Variable;

            Tw.SetCallbacks.Add(id, new Tw.SetVarCallback(var.SetCallback));
            Tw.GetCallbacks.Add(id, new Tw.GetVarCallback(var.GetCallback));

            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        Tw.VariableType.Color4F,
                        Tw.SetCallbacks[id],
                        Tw.GetCallbacks[id],
                        IntPtr.Zero, null);
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
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            float[] data = new float[4]; /* R, G, B, A */
            Marshal.Copy(pointer, data, 0, data.Length);

            bool changed = (data[0] != R)
                        || (data[1] != G)
                        || (data[2] != B)
                        || (data[3] != A);

            R = data[0];
            G = data[1];
            B = data[2];
            A = data[3];

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            float[] data = new float[] { R, G, B, A };
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
            return String.Format("[Color4Variable: Label={0}, Value=({1}, {2}, {3}, {4})]", Label, R, G, B, A);
        }
    }
}
