using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    public sealed class Color4ValidationEventArgs : EventArgs
    {
        /// <summary>
        /// Whether to accept this RGBA color value.
        /// </summary>
        public bool Valid { get; set; }
        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }
        public float A { get; private set; }

        public Color4ValidationEventArgs(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }

    /// <summary>
    /// An AntTweakBar variable which can hold an RGBA color value.
    /// </summary>
    public sealed class Color4Variable : Variable, IValueVariable
    {
        /// <summary>
        /// Occurs when the user changes this variable's value.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Occurs when the new value of this variable is validated.
        /// </summary>
        public event EventHandler<Color4ValidationEventArgs> Validating;

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
        /// Gets or sets this color's red channel component.
        /// </summary>
        public float R
        {
            get { ThrowIfDisposed(); return r; }
            set { ValidateAndSet(value, G, B, A); }
        }

        /// <summary>
        /// Gets or sets this color's green channel component.
        /// </summary>
        public float G
        {
            get { ThrowIfDisposed(); return g; }
            set { ValidateAndSet(R, value, B, A); }
        }

        /// <summary>
        /// Gets or sets this color's blue channel component.
        /// </summary>
        public float B
        {
            get { ThrowIfDisposed(); return b; }
            set { ValidateAndSet(R, G, value, A); }
        }

        /// <summary>
        /// Gets or sets this color's alpha channel component.
        /// </summary>
        public float A
        {
            get { ThrowIfDisposed(); return a; }
            set { ValidateAndSet(R, G, B, value); }
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
        private static void InitColor4Variable(Variable var, String id)
        {
            var it = var as Color4Variable;

            Variable.SetCallbacks.Add(id, new Tw.SetVarCallback(it.SetCallback));
            Variable.GetCallbacks.Add(id, new Tw.GetVarCallback(it.GetCallback));

            Tw.AddVarCB(var.ParentBar.Pointer, id,
                        Tw.VariableType.Color4F,
                        Variable.SetCallbacks[id],
                        Variable.GetCallbacks[id],
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
            Validating += (s, e) => { e.Valid = (0 <= e.R) && (e.R <= 1); };
            Validating += (s, e) => { e.Valid = (0 <= e.G) && (e.G <= 1); };
            Validating += (s, e) => { e.Valid = (0 <= e.B) && (e.B <= 1); };
            Validating += (s, e) => { e.Valid = (0 <= e.A) && (e.A <= 1); };

            ValidateAndSet(r, g, b, a);
        }

        /// <summary>
        /// Checks if this variable can hold this value.
        /// </summary>
        private bool IsValid(float r, float g, float b, float a)
        {
            ThrowIfDisposed();

            return !Validating.GetInvocationList().Select(h => {
                var check = new Color4ValidationEventArgs(r, g, b, a);
                h.DynamicInvoke(new object[] { this, check });
                return !check.Valid;
            }).Any(failed => failed);
        }

        /// <summary>
        /// Tries to set this variable's value, validating it.
        /// </summary>
        private void ValidateAndSet(float r, float g, float b, float a)
        {
            if (!IsValid(r, g, b, a)) {
                throw new ArgumentException("Invalid variable value.");
            } else {
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
            }
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            float[] data = new float[4]; /* R, G, B, A */
            Marshal.Copy(pointer, data, 0, data.Length);

            if (IsValid(data[0], data[1], data[2], data[3]))
            {
                bool changed = (data[0] != r)
                            || (data[1] != g)
                            || (data[2] != b)
                            || (data[3] != a);

                r = data[0];
                g = data[1];
                b = data[2];
                a = data[3];

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
