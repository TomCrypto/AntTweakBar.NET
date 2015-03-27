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
    sealed class PolynomialVariable : IValueVariable
    {
        // Used to hold optional symbols used during polynomial parsing
        private readonly IDictionary<String, Double> symbols = new Dictionary<String, Double>();

        public IDictionary<String, Double> Symbols { get { return symbols; } }

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

        public Bar ParentBar { get { return polyString.ParentBar; } }

        public event EventHandler Changed
        {
            add { polyString.Changed += value; }
            remove { polyString.Changed -= value; }
        }

        public void OnChanged(EventArgs e)
        {
            polyString.OnChanged(e);
        }

        /// <summary>
        /// The actual backing variable.
        /// </summary>
        private StringVariable polyString { get; set; }

        public PolynomialVariable(Bar bar, Polynomial poly, String def = null)
        {
            polyString = new StringVariable(bar, poly.ToString(), def);
            polyString.Validating += (s, e) => {
                e.Valid = (Polynomial.Parse(e.Value, symbols) != null);
            };
        }

        public String Label
        {
            get { return polyString.Label; }
            set { polyString.Label = value; }
        }

        public Polynomial Value
        {
            get { return Polynomial.Parse(polyString.Value, symbols); }
            set { polyString.Value = value.ToString(); }
        }

        public Group Group
        {
            get { return polyString.Group; }
            set { polyString.Group = value; }
        }

        public void Dispose()
        {
            polyString.Dispose();
        }
    }

    sealed class ComplexVariable : StructVariable<Complex>
    {
        private DoubleVariable Re { get { return (variables[0] as DoubleVariable); } }
        private DoubleVariable Im { get { return (variables[1] as DoubleVariable); } }

        public ComplexVariable(Bar bar, Complex initialValue)
            : base(bar, new DoubleVariable(bar, initialValue.Real),
                        new DoubleVariable(bar, initialValue.Imaginary))
        {
            Re.Label = "Real";
            Im.Label = "Imaginary";
        }

        public override Complex Value
        {
            get
            {
                return new Complex(Re.Value, Im.Value);
            }

            set
            {
                Re.Value = value.Real;
                Im.Value = value.Imaginary;
            }
        }

        public Double Step
        {
            get { return Re.Step; }
            set
            {
                Re.Step = value;
                Im.Step = value;
            }
        }

        public Double Precision
        {
            get { return Re.Precision; }
            set
            {
                Re.Precision = value;
                Im.Precision = value;
            }
        }
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
        // convert any OpenTK events to AntTweakBar events before handling them.

        private static bool HandleMouseClick(Context context, MouseButtonEventArgs e)
        {
            IDictionary<OpenTK.Input.MouseButton, Tw.MouseButton> Mapping = new Dictionary<OpenTK.Input.MouseButton, Tw.MouseButton>()
            {
                { OpenTK.Input.MouseButton.Left, Tw.MouseButton.Left },
                { OpenTK.Input.MouseButton.Middle, Tw.MouseButton.Middle },
                { OpenTK.Input.MouseButton.Right, Tw.MouseButton.Right },
            };

            var action = e.IsPressed ? Tw.MouseAction.Pressed : Tw.MouseAction.Released;

            if (Mapping.ContainsKey(e.Button)) {
                return context.HandleMouseClick(action, Mapping[e.Button]);
            } else {
                return false;
            }
        }

        private static bool HandleKeyInput(Context context, KeyboardKeyEventArgs e, bool down)
        {
            IDictionary<OpenTK.Input.Key, Tw.Key> Mapping = new Dictionary<OpenTK.Input.Key, Tw.Key>()
            {
                { OpenTK.Input.Key.BackSpace, Tw.Key.Backspace },
                { OpenTK.Input.Key.Tab, Tw.Key.Tab },
                { OpenTK.Input.Key.Clear, Tw.Key.Clear },
                { OpenTK.Input.Key.Enter, Tw.Key.Return },
                { OpenTK.Input.Key.Pause, Tw.Key.Pause },
                { OpenTK.Input.Key.Escape, Tw.Key.Escape },
                { OpenTK.Input.Key.Space, Tw.Key.Space },
                { OpenTK.Input.Key.Delete, Tw.Key.Delete },
                { OpenTK.Input.Key.Up, Tw.Key.Up },
                { OpenTK.Input.Key.Down, Tw.Key.Down },
                { OpenTK.Input.Key.Right, Tw.Key.Right },
                { OpenTK.Input.Key.Left, Tw.Key.Left },
                { OpenTK.Input.Key.Insert, Tw.Key.Insert },
                { OpenTK.Input.Key.Home, Tw.Key.Home },
                { OpenTK.Input.Key.End, Tw.Key.End },
                { OpenTK.Input.Key.PageUp, Tw.Key.PageUp },
                { OpenTK.Input.Key.PageDown, Tw.Key.PageDown },
                { OpenTK.Input.Key.F1, Tw.Key.F1 },
                { OpenTK.Input.Key.F2, Tw.Key.F2 },
                { OpenTK.Input.Key.F3, Tw.Key.F3 },
                { OpenTK.Input.Key.F4, Tw.Key.F4 },
                { OpenTK.Input.Key.F5, Tw.Key.F5 },
                { OpenTK.Input.Key.F6, Tw.Key.F6 },
                { OpenTK.Input.Key.F7, Tw.Key.F7 },
                { OpenTK.Input.Key.F8, Tw.Key.F8 },
                { OpenTK.Input.Key.F9, Tw.Key.F9 },
                { OpenTK.Input.Key.F10, Tw.Key.F10 },
                { OpenTK.Input.Key.F11, Tw.Key.F11 },
                { OpenTK.Input.Key.F12, Tw.Key.F12 },
                { OpenTK.Input.Key.F13, Tw.Key.F13 },
                { OpenTK.Input.Key.F14, Tw.Key.F14 },
                { OpenTK.Input.Key.F15, Tw.Key.F15 },
                { OpenTK.Input.Key.A, Tw.Key.A },
                { OpenTK.Input.Key.B, Tw.Key.B },
                { OpenTK.Input.Key.C, Tw.Key.C },
                { OpenTK.Input.Key.D, Tw.Key.D },
                { OpenTK.Input.Key.E, Tw.Key.E },
                { OpenTK.Input.Key.F, Tw.Key.F },
                { OpenTK.Input.Key.G, Tw.Key.G },
                { OpenTK.Input.Key.H, Tw.Key.H },
                { OpenTK.Input.Key.I, Tw.Key.I },
                { OpenTK.Input.Key.J, Tw.Key.J },
                { OpenTK.Input.Key.K, Tw.Key.K },
                { OpenTK.Input.Key.L, Tw.Key.L },
                { OpenTK.Input.Key.M, Tw.Key.M },
                { OpenTK.Input.Key.N, Tw.Key.N },
                { OpenTK.Input.Key.O, Tw.Key.O },
                { OpenTK.Input.Key.P, Tw.Key.P },
                { OpenTK.Input.Key.Q, Tw.Key.Q },
                { OpenTK.Input.Key.R, Tw.Key.R },
                { OpenTK.Input.Key.S, Tw.Key.S },
                { OpenTK.Input.Key.T, Tw.Key.T },
                { OpenTK.Input.Key.U, Tw.Key.U },
                { OpenTK.Input.Key.V, Tw.Key.V },
                { OpenTK.Input.Key.W, Tw.Key.W },
                { OpenTK.Input.Key.X, Tw.Key.X },
                { OpenTK.Input.Key.Y, Tw.Key.Y },
                { OpenTK.Input.Key.Z, Tw.Key.Z },
            };

            var modifiers = Tw.KeyModifiers.None;
            if (e.Modifiers.HasFlag(KeyModifiers.Alt))
                modifiers |= Tw.KeyModifiers.Alt;
            if (e.Modifiers.HasFlag(KeyModifiers.Shift))
                modifiers |= Tw.KeyModifiers.Shift;
            if (e.Modifiers.HasFlag(KeyModifiers.Control))
                modifiers |= Tw.KeyModifiers.Ctrl;

            if (Mapping.ContainsKey(e.Key)) {
                if (down) {
                    return context.HandleKeyDown(Mapping[e.Key], modifiers);
                } else {
                    return context.HandleKeyUp(Mapping[e.Key], modifiers);
                }
            } else {
                return false;
            }
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

            var aCoeffVar = new ComplexVariable(configsBar, fractal.ACoeff);
            aCoeffVar.Changed += delegate { fractal.ACoeff = aCoeffVar.Value; };
            aCoeffVar.Group.Label = "Relaxation Coefficient";
            aCoeffVar.Step = 0.0002;
            aCoeffVar.Precision = 4;

            var kcoeff = new ComplexVariable(configsBar, fractal.KCoeff);
            kcoeff.Changed += delegate { fractal.KCoeff = kcoeff.Value; };
            kcoeff.Group.Label = "Nova Coefficient";
            kcoeff.Step = 0.0002;
            kcoeff.Precision = 4;

            /* Set up a bar for the user to play with the equation */

            var fractalBar = new Bar(context, "label='Fractal Equation' valueswidth=400");
            fractalBar.Contained = true;
            fractalBar.Position = new Point(Width - 500 - 20, 20);
            fractalBar.Size = new Size(500, 150);

            var poly = new PolynomialVariable(fractalBar, Polynomial.Parse("z^3 - 1"));
            var preset = new EnumVariable<FractalPreset>(fractalBar, FractalPreset.Cubic);
            poly.Changed += delegate { fractal.Polynomial = poly.Value; };
            poly.Label = "Equation";
            preset.Label = "Presets";
            preset.Changed += delegate
            {
                switch (preset.Value)
                {
                    case FractalPreset.Cubic:
                        poly.Value = Polynomial.Parse("z^3 - 1", poly.Symbols);
                        break;
                    case FractalPreset.OtherCubic:
                        poly.Value = Polynomial.Parse("z^3 - 2z + 2", poly.Symbols);
                        break;
                    case FractalPreset.SineTaylor:
                        poly.Value = Polynomial.Parse("z - 1/6z^3 + 1/120z^5 - 1/5040z^7 + 1/362880z^9", poly.Symbols);
                        break;
                    case FractalPreset.ExpIz:
                        poly.Value = Polynomial.Parse("1 + iz - 1/2z^2 + (1/6i)z^3", poly.Symbols);
                        break;
                }

                fractal.Polynomial = poly.Value;
            };

            /* This is where you can use the Changed event to great advantage: we are
             * going to create variables to manipulate symbols in the polynomial formula,
             * and directly bind them to the "poly" variable's symbol dictionary, without
             * keeping a reference to them. That way we don't need to track them at all.
            */

            var symbolicVarGroup = new Group(fractalBar);

            for (char symbol = 'A'; symbol <= 'F'; ++symbol)
            {
                var symbolVar = new DoubleVariable(fractalBar);
                symbolVar.Label = symbol.ToString();
                symbolVar.Group = symbolicVarGroup;
                symbolVar.Step = 0.0002f;
                symbolVar.Precision = 4;

                symbolVar.Changed += delegate
                {
                    poly[symbolVar.Label] = symbolVar.Value; // update symbol
                    fractal.Polynomial = poly.Value; // update fractal
                };

                // also add the symbol initially, so that it exists on startup
                poly[symbolVar.Label] = symbolVar.Value;
            }

            /* Start the symbol list closed to avoid clutter */
            symbolicVarGroup.Label = "Symbolic Variables";
            symbolicVarGroup.Open = false;
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

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (HandleMouseClick(context, e)) {
                return;
            }

            if (e.Button == MouseButton.Left)
                fractal.Pan(0.5f * (2.0f * e.X -  Width) / Height,
                            0.5f * (2.0f * e.Y - Height) / Height);
            else if (e.Button == MouseButton.Right)
                fractal.ZoomIn(1);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (HandleMouseClick(context, e)) {
                return;
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            if (context.HandleMouseMove(e.Position)) {
                return;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (context.HandleMouseWheel(e.Value)) {
                return;
            }

            fractal.ZoomIn(e.DeltaPrecise);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (HandleKeyInput(context, e, true)) {
                return;
            }

            if (e.Key == Key.Escape) {
                Close();
            }
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (HandleKeyInput(context, e, false)) {
                return;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (context.HandleKeyPress(e.KeyChar)) {
                return;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            fractal.Dimensions = ClientSize;
            context.HandleResize(ClientSize);
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
