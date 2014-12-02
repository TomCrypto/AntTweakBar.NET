using System;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;

using AntTweakBar;

namespace Sample
{
    /// <summary>
    /// This variable will hold a polynomial (if an invalid formula
    /// is entered by the user, it will simply refuse to accept it).
    /// </summary>
    class PolynomialVariable
    {
        // Used to hold optional symbols used during polynomial parsing
        private readonly IDictionary<String, Double> symbols = new Dictionary<String, Double>();

        /// <summary>
        /// Gets or sets a polynomial symbol.
        /// </summary>
        public double this[String index]
        {
            get { return symbols[index]; }
            set
            {
                if (symbols.ContainsKey(index)) {
                    symbols[index] = value;
                } else {
                    symbols.Add(index, value);
                }
            }
        }

        /// <summary>
        /// The actual backing variable.
        /// </summary>
        public StringVariable PolyString { get; set; }

        public PolynomialVariable(Bar bar, String poly, String def = null)
        {
            PolyString = new StringVariable(bar, poly, def);
            PolyString.Validating += (s, e) => { e.Valid = (Polynomial.Parse(e.Value, symbols) != null); };
        }

        public Polynomial Polynomial
        {
            get { return Polynomial.Parse(PolyString.Value, symbols); }
        }
    }

    /// <summary>
    /// Not a real variable (just contains two variables for the
    /// real/imaginary parts and puts them in a single group).
    /// </summary>
    class ComplexVariable : IDisposable
    {
        private readonly DoubleVariable re, im;

        /// <summary>
        /// Occurs when the user changes the variable.
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
        /// Gets or sets the value of this variable.
        /// </summary>
        public Complex Value
        {
            get { return new Complex(re.Value, im.Value); }
            set
            {
                bool changed = !value.Equals(Value);
                re.Value = value.Real;
                im.Value = value.Imaginary;
                if (changed)
                    OnChanged(EventArgs.Empty);
            }
        }

        public ComplexVariable(Bar bar, Complex initialValue)
        {
            re = new DoubleVariable(bar, initialValue.Real);
            im = new DoubleVariable(bar, initialValue.Imaginary);

            re.Changed += (sender, e) => OnChanged(EventArgs.Empty);
            im.Changed += (sender, e) => OnChanged(EventArgs.Empty);

            re.Label = "Real";
            im.Label = "Imaginary";

            Label = "undef";
        }

        public Double Step
        {
            get { return re.Step; }
            set
            {
                re.Step = value;
                im.Step = value;
            }
        }

        public Double Precision
        {
            get { return re.Precision; }
            set
            {
                re.Precision = value;
                im.Precision = value;
            }
        }

        public String Label
        {
            get { return re.Group; }
            set
            {
                re.Group = value;
                im.Group = value;
            }
        }

        #region IDisposable

        ~ComplexVariable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            re.Dispose();
            im.Dispose();

            disposed = true;
        }

        private bool disposed = false;

        #endregion
    }

    class Program : GameWindow
    {
        #region Entry Point

        private const int EXIT_SUCCESS = 0;
        private const int EXIT_FAILURE = 1;

        [STAThread]
        public static int Main(String[] args)
        {
            try
            {
                using (var window = new Program()) {
                    window.Run(120.0, 60.0);
                }

                return EXIT_SUCCESS;
            }
            catch (AntTweakBarException e)
            {
                Console.WriteLine("AntTweakBar error: " + e.Message);
                Console.WriteLine("Exception details: " + e.Details);
                Console.WriteLine("\n======= STACK TRACE =======\n");
                Console.WriteLine(e.StackTrace); return EXIT_FAILURE;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                Console.WriteLine("\n======= STACK TRACE =======\n");
                Console.WriteLine(e.StackTrace); return EXIT_FAILURE;
            }
        }

        #endregion

        #region Event Conversion

        // Because OpenTK does not use an event loop, the native AntTweakBar library
        // has no provisions for directly handling user events. Therefore we need to
        // convert polled OpenTK events to AntTweakBar events before handling them.

        private static bool HandleMouseClick(Context context, MouseButtonEventArgs e)
        {
            var action = e.IsPressed ? Tw.MouseAction.Pressed : Tw.MouseAction.Released;

            switch (e.Button)
            {
                case MouseButton.Left:
                    return context.HandleMouseClick(action, Tw.MouseButton.Left);
                case MouseButton.Right:
                    return context.HandleMouseClick(action, Tw.MouseButton.Right);
                case MouseButton.Middle:
                    return context.HandleMouseClick(action, Tw.MouseButton.Middle);
            }

            return false;
        }

        private static bool HandleKeyPress(Context context, KeyboardKeyEventArgs e)
        {
            var modifier = Tw.KeyModifier.None;
            if (e.Modifiers.HasFlag(KeyModifiers.Alt))
                modifier |= Tw.KeyModifier.Alt;
            if (e.Modifiers.HasFlag(KeyModifiers.Shift))
                modifier |= Tw.KeyModifier.Shift;
            if (e.Modifiers.HasFlag(KeyModifiers.Control))
                modifier |= Tw.KeyModifier.Ctrl;

            var mapping = new Dictionary<Key, Tw.SpecialKey>()
            {
                { Key.Back,          Tw.SpecialKey.Backspace },
                { Key.Tab,           Tw.SpecialKey.Tab },
                { Key.Clear,         Tw.SpecialKey.Clear },
                { Key.Enter,         Tw.SpecialKey.Return },
                { Key.Pause,         Tw.SpecialKey.Pause },
                { Key.Escape,        Tw.SpecialKey.Escape },
                //{ Key.Space,         TW.SpecialKey.Space }, // already handled by KeyPress
                { Key.Delete,        Tw.SpecialKey.Delete },
                { Key.Up,            Tw.SpecialKey.Up },
                { Key.Left,          Tw.SpecialKey.Left },
                { Key.Down,          Tw.SpecialKey.Down },
                { Key.Right,         Tw.SpecialKey.Right },
                { Key.Insert,        Tw.SpecialKey.Insert },
                { Key.Home,          Tw.SpecialKey.Home },
                { Key.End,           Tw.SpecialKey.End },
                { Key.PageUp,        Tw.SpecialKey.PageUp },
                { Key.PageDown,      Tw.SpecialKey.PageDown },
                { Key.F1,            Tw.SpecialKey.F1 },
                { Key.F2,            Tw.SpecialKey.F2 },
                { Key.F3,            Tw.SpecialKey.F3 },
                { Key.F4,            Tw.SpecialKey.F4 },
                { Key.F5,            Tw.SpecialKey.F5 },
                { Key.F6,            Tw.SpecialKey.F6 },
                { Key.F7,            Tw.SpecialKey.F7 },
                { Key.F8,            Tw.SpecialKey.F8 },
                { Key.F9,            Tw.SpecialKey.F9 },
                { Key.F10,           Tw.SpecialKey.F10 },
                { Key.F11,           Tw.SpecialKey.F11 },
                { Key.F12,           Tw.SpecialKey.F12 },
                { Key.F13,           Tw.SpecialKey.F13 },
                { Key.F14,           Tw.SpecialKey.F14 },
                { Key.F15,           Tw.SpecialKey.F15 },
            };

            if (mapping.ContainsKey(e.Key))
                return context.HandleKeyPress(mapping[e.Key], modifier);
            else
                return false;
        }

        #endregion

        private Context context;
        private Fractal fractal;

        public Program() : base(1024, 768, GraphicsMode.Default, "AntTweakBar.NET Sample")
        {
            this.Icon = new Icon("Properties/Fractal.ico");
        }

        protected override void OnLoad(EventArgs _)
        {
            base.OnLoad(_);

            context = new Context(Tw.GraphicsAPI.OpenGL);
            fractal = new Fractal();

            /* Hook up the different events to the AntTweakBar.NET context, and
             * allow the user to navigate the fractal using the keyboard/mouse. */

            KeyPress += (sender, e) => context.HandleKeyPress(e.KeyChar);
            Resize += (sender, e) => context.HandleResize(ClientSize);
            KeyDown += (sender, e) => {
                if (!HandleKeyPress(context, e) && (e.Key == Key.Escape)) {
                    Close(); // Close program on Esc when appropriate
                }
            };

            Mouse.WheelChanged += (sender, e) => fractal.ZoomIn(e.DeltaPrecise);
            Mouse.Move += (sender, e) => context.HandleMouseMove(e.Position);
            Mouse.ButtonUp += (sender, e) => HandleMouseClick(context, e);
            Mouse.ButtonDown += (sender, e) =>
            {
                if (!HandleMouseClick(context, e))
                {
                    if (e.Button == MouseButton.Left)
                        fractal.Pan(0.5f * (2.0f * e.X -  Width) / Height,
                                    0.5f * (2.0f * e.Y - Height) / Height);
                    else if (e.Button == MouseButton.Right)
                        fractal.ZoomIn(1);
                }
            };

            /* Add AntTweakBar variables and events */

            var configsBar = new Bar(context);
            configsBar.Label = "Configuration";
            configsBar.Contained = true;

            var thresholdVar = new FloatVariable(configsBar, fractal.Threshold);
            thresholdVar.Changed += delegate { fractal.Threshold = thresholdVar.Value; };
            thresholdVar.SetDefinition("min=0 max=5 step=0.01 precision=2)");
            thresholdVar.Label = "Convergence";

            var itersVar = new IntVariable(configsBar, fractal.Iterations);
            itersVar.Changed += delegate { fractal.Iterations = itersVar.Value; };
            itersVar.Label = "Iterations";
            itersVar.Max = 256;
            itersVar.Min = 16;

            var aaVar = new EnumVariable<AAQuality>(configsBar, fractal.AA);
            aaVar.Changed += delegate { fractal.AA = aaVar.Value; };
            aaVar.Label = "AA";

            var hardcode = new BoolVariable(configsBar, fractal.HardcodePolynomial);
            hardcode.Changed += delegate { fractal.HardcodePolynomial = hardcode.Value; };
            hardcode.Label = "Hardcoded";
            hardcode.Help = "If enabled, this will improve runtime performance, but " +
                "changing the fractal formula will be more expensive. In particular, " +
                "modifying symbolic variables may feel quite sluggish.";

            new Separator(configsBar);

            var paletteVar = new Color4Variable(configsBar, fractal.Palette.R, fractal.Palette.G, fractal.Palette.B, fractal.Palette.A);
            paletteVar.Changed += delegate { fractal.Palette = new Color4(paletteVar.R, paletteVar.G, paletteVar.B, paletteVar.A); };
            paletteVar.Label = "Palette";

            var shadingVar = new EnumVariable<ShadingType>(configsBar, fractal.Shading);
            shadingVar.Changed += delegate { fractal.Shading = shadingVar.Value; };
            shadingVar.Label = "Shading";

            var intensityVar = new FloatVariable(configsBar, fractal.Intensity);
            intensityVar.Changed += delegate { fractal.Intensity = intensityVar.Value; };
            intensityVar.SetDefinition("min=0 max=3 step=0.01 precision=2)");
            intensityVar.Label = "Intensity";

            /*var aCoeffVar = new ComplexVariable(configsBar, fractal.ACoeff);
            aCoeffVar.Changed += delegate { fractal.ACoeff = aCoeffVar.Value; };
            aCoeffVar.Label = "Relaxation Coeff.";
            aCoeffVar.Step = 0.0002;
            aCoeffVar.Precision = 4;*/

            var aCoeffVar = new StructVariable<Complex>(configsBar, new Dictionary<String, Tw.StructMemberInfo>()
            {
                { "re", Tw.StructMemberInfo.FromStruct<Complex>("real", Tw.VariableType.Double, "label='real' step=0.0002 precision=4") },
                { "im", Tw.StructMemberInfo.FromStruct<Complex>("imaginary", Tw.VariableType.Double, "label='imaginary' step=0.0002 precision=4") },
            }, fractal.ACoeff, "label='Relaxation Coeff.'");
            aCoeffVar.Changed += delegate { fractal.ACoeff = aCoeffVar.Value; };

            var kcoeff = new ComplexVariable(configsBar, fractal.KCoeff);
            kcoeff.Changed += delegate { fractal.KCoeff = kcoeff.Value; };
            kcoeff.Label = "Nova Coeff.";
            kcoeff.Step = 0.0002;
            kcoeff.Precision = 4;

            /* Set up a bar for the user to play with the equation */

            var fractalBar = new Bar(context, "label='Fractal Equation' valueswidth=400");
            fractalBar.Contained = true;
            fractalBar.Position = new Point(Width - 500 - 20, 20);
            fractalBar.Size = new Size(500, 150);

            var poly = new PolynomialVariable(fractalBar, "z^3 - 1");
            var preset = new EnumVariable<FractalPreset>(fractalBar, FractalPreset.Cubic);
            poly.PolyString.Changed += delegate { fractal.Polynomial = poly.Polynomial; };
            poly.PolyString.Label = "Equation";
            preset.Label = "Presets";
            preset.Changed += delegate
            {
                switch (preset.Value)
                {
                    case FractalPreset.Cubic:
                        poly.PolyString.Value = "z^3 - 1";
                        break;
                    case FractalPreset.OtherCubic:
                        poly.PolyString.Value = "z^3 - 2z + 2";
                        break;
                    case FractalPreset.SineTaylor:
                        poly.PolyString.Value = "z - 1/6z^3 + 1/120z^5 - 1/5040z^7 + 1/362880z^9";
                        break;
                    case FractalPreset.ExpIz:
                        poly.PolyString.Value = "1 + iz - 1/2z^2 + (1/6i)z^3";
                        break;
                }

                fractal.Polynomial = poly.Polynomial;
            };

            /* This is where you can use the Changed event to great advantage: we are
             * going to create variables to manipulate symbols in the polynomial formula,
             * and directly bind them to the "poly" variable's symbol dictionary, without
             * keeping a reference to them. That way we don't need to track them at all.
            */

            for (char symbol = 'A'; symbol <= 'F'; ++symbol)
            {
                var symbolVar = new DoubleVariable(fractalBar);
                symbolVar.Group = "Symbolic Variables";
                symbolVar.Label = symbol.ToString();
                symbolVar.Step = 0.0002f;
                symbolVar.Precision = 4;

                symbolVar.Changed += delegate
                {
                    poly[symbolVar.Label] = symbolVar.Value; // update symbol
                    fractal.Polynomial = poly.Polynomial; // update fractal
                };

                // also add the symbol initially, so that it exists on startup
                poly[symbolVar.Label] = symbolVar.Value;
            }

            /* Start the symbol list closed to avoid clutter */
            fractalBar.OpenGroup("Symbolic Variables", false);
        }

        private enum FractalPreset
        {
            [Description("Standard cubic")]
            Cubic,

            [Description("Basins of nonconvergence")]
            OtherCubic,

            [Description("Taylor series for sin(z)")]
            SineTaylor,

            [Description("Taylor series for e^(iz)")]
            ExpIz,
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            fractal.Dimensions = ClientSize;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            fractal.Draw();
            context.Draw();
            SwapBuffers();
        }

        protected override void Dispose(bool manual)
        {
            if (fractal != null) {
                fractal.Dispose();
            }

            if (context != null) {
                context.Dispose();
            }

            base.Dispose(manual);
        }
    }
}
