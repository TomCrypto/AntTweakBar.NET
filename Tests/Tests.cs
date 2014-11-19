using System;
using System.Linq;
using System.Drawing;
using NUnit.Framework;

using AntTweakBar;

namespace Tests
{
    #region Test Types

    public enum TestEnum
    {
        Choice1 = 1,
        Choice2 = 1894,
        Choice3 = 2005,
    }

    #endregion

    #region Support Code

    [TestFixture()]
    public class RequiresEnvironment
    {
        private Context context;
        private Bar bar;

        protected Context Context { get { return context; } }
        protected Bar Bar { get { return bar; } }

        [SetUp]
        public void Init()
        {
            context = new Context(Tw.GraphicsAPI.OpenGL);
            bar = new Bar(context);
        }

        [TearDown]
        public void Dispose()
        {
            context.Dispose();
        }
    }

    #endregion

    [TestFixture()]
    public class ContextBarManagement : RequiresEnvironment
    {
        [Test()]
        public void AddBar()
        {
            var bar = new Bar(Context);
            Assert.AreEqual(2, Context.Count()); // new bar + default one
            bar.Dispose();
        }

        [Test()]
        public void DeleteBar()
        {
            var bar = new Bar(Context);
            bar.Dispose();
            Assert.AreEqual(1, Context.Count());
        }

        [Test()]
        public void AddVariable()
        {
            var variable = new IntVariable(Bar);
            Assert.AreEqual(1, Bar.Count());
            variable.Dispose();
        }

        [Test()]
        public void DeleteVariable()
        {
            var variable = new IntVariable(Bar);
            variable.Dispose();
            Assert.AreEqual(0, Bar.Count());
        }
    }

    [TestFixture()]
    public class BarProperties : RequiresEnvironment
    {
        [Test()]
        public void Label()
        {
            Bar.Label = "testing";
            Assert.AreEqual("testing", Bar.Label);
        }

        [Test()]
        public void Help()
        {
            Bar.Help = "testing";
            Assert.AreEqual("testing", Bar.Help);
        }

        [Test()]
        public void Color()
        {
            Bar.Color = System.Drawing.Color.FromArgb(75, 30, 50);
            Assert.AreEqual(System.Drawing.Color.FromArgb(75, 30, 50), Bar.Color);
        }

        [Test()]
        public void Alpha()
        {
            Bar.Alpha = 101;
            Assert.AreEqual(101, Bar.Alpha);
        }

        [Test()]
        public void Position()
        {
            Bar.Position = new Point(15, 15);
            Assert.AreEqual(new Point(15, 15), Bar.Position);
        }

        [Test()]
        public void Size()
        {
            Bar.Size = new Size(200, 100);
            Assert.AreEqual(new Size(200, 100), Bar.Size);
        }

        [Test()]
        public void Iconifiable()
        {
            Bar.Iconifiable = false;
            Assert.AreEqual(false, Bar.Iconifiable);
        }

        [Test()]
        public void Movable()
        {
            Bar.Movable = false;
            Assert.AreEqual(false, Bar.Movable);
        }

        [Test()]
        public void Resizable()
        {
            Bar.Resizable = false;
            Assert.AreEqual(false, Bar.Resizable);
        }

        [Test()]
        public void Contained()
        {
            Bar.Contained = true;
            Assert.AreEqual(true, Bar.Contained);
        }

        [Test()]
        public void Visible()
        {
            Bar.Visible = false;
            Assert.AreEqual(false, Bar.Visible);
        }

        [Test()]
        public void ShowHelpBar()
        {
            Context.ShowHelpBar(true);
        }
    }

    [TestFixture()]
    public class VariableProperties : RequiresEnvironment 
    {
        private IntVariable Variable { get; set; } // any variable would do
        [SetUp] public new void Init() { Variable = new IntVariable(Bar); }

        [Test()]
        public void Label()
        {
            Variable.Label = "testing";
            Assert.AreEqual("testing", Variable.Label);
        }

        [Test()]
        public void Help()
        {
            Variable.Help = "testing";
            Assert.AreEqual("testing", Variable.Help);
        }

        [Test()]
        public void Group()
        {
            Variable.Group = "testing";
            Assert.AreEqual("testing", Variable.Group);
        }

        [Test()]
        public void Visible()
        {
            Variable.Visible = false;
            Assert.AreEqual(false, Variable.Visible);
        }

        [Test()]
        public void ReadOnly()
        {
            Variable.ReadOnly = false;
            Assert.AreEqual(false, Variable.ReadOnly);
        }

        [Test()]
        public void KeyShortcut()
        {
            Variable.KeyShortcut = "CTRL+A";
            Assert.AreEqual("CTRL+A", Variable.KeyShortcut);
        }

        [Test()]
        public void KeyIncrementShortcut()
        {
            Variable.KeyIncrementShortcut = "CTRL+A";
            Assert.AreEqual("CTRL+A", Variable.KeyIncrementShortcut);
        }

        [Test()]
        public void KeyDecrementShortcut()
        {
            Variable.KeyDecrementShortcut = "CTRL+A";
            Assert.AreEqual("CTRL+A", Variable.KeyDecrementShortcut);
        }
    }

    [TestFixture()]
    public class BooleanVariableProperties : RequiresEnvironment
    {
        private BoolVariable Variable { get; set; }
        [SetUp] public new void Init() { Variable = new BoolVariable(Bar); }

        [Test()]
        public void Value()
        {
            Variable.Value = true;
            Assert.AreEqual(true, Variable.Value);
        }

        [Test()]
        public void LabelTrue()
        {
            Variable.LabelTrue = "testing";
            Assert.AreEqual("testing", Variable.LabelTrue);
        }

        [Test()]
        public void LabelFalse()
        {
            Variable.LabelFalse = "testing";
            Assert.AreEqual("testing", Variable.LabelFalse);
        }
    }

    [TestFixture()]
    public class Int32VariableProperties : RequiresEnvironment
    {
        private IntVariable Variable { get; set; }
        [SetUp] public new void Init() { Variable = new IntVariable(Bar); }

        [Test()]
        public void Value()
        {
            Variable.Value = 42;
            Assert.AreEqual(42, Variable.Value);
        }

        [Test()]
        public void Min()
        {
            Variable.Min = -5;
            Assert.AreEqual(-5, Variable.Min);
        }

        [Test()]
        public void Max()
        {
            Variable.Max = 30;
            Assert.AreEqual(30, Variable.Max);
        }

        [Test()]
        public void Step()
        {
            Variable.Step = 3;
            Assert.AreEqual(3, Variable.Step);
        }

        [Test()]
        public void Hexadecimal()
        {
            Variable.Hexadecimal = true;
            Assert.AreEqual(true, Variable.Hexadecimal);
        }

        [Test()]
        public void MinConstraint()
        {
            Variable.Value = 42;
            Variable.Min = 50;
            Assert.AreEqual(50, Variable.Value);
        }

        [Test()]
        public void MaxConstraint()
        {
            Variable.Value = 42;
            Variable.Max = 30;
            Assert.AreEqual(30, Variable.Value);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MinCheck()
        {
            Variable.Min = 50;
            Variable.Value = 49;
        }

        [Test()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MaxCheck()
        {
            Variable.Max = 50;
            Variable.Value = 51;
        }
    }

    [TestFixture()]
    public class SingleVariableProperties : RequiresEnvironment
    {
        private FloatVariable Variable { get; set; }
        [SetUp] public new void Init() { Variable = new FloatVariable(Bar); }

        [Test()]
        public void Value()
        {
            Variable.Value = 3.5f;
            Assert.AreEqual(3.5f, Variable.Value);
        }

        [Test()]
        public void Min()
        {
            Variable.Min = -5;
            Assert.AreEqual(-5, Variable.Min);
        }

        [Test()]
        public void Max()
        {
            Variable.Max = 30;
            Assert.AreEqual(30, Variable.Max);
        }

        [Test()]
        public void Step()
        {
            Variable.Step = 3;
            Assert.AreEqual(3, Variable.Step);
        }

        [Test()]
        public void Precision()
        {
            Variable.Precision = 5;
            Assert.AreEqual(5, Variable.Precision);
        }

        [Test()]
        public void MinConstraint()
        {
            Variable.Value = 3.5f;
            Variable.Min = 3.7f;
            Assert.AreEqual(3.7f, Variable.Value);
        }

        [Test()]
        public void MaxConstraint()
        {
            Variable.Value = 3.5f;
            Variable.Max = 2.5f;
            Assert.AreEqual(2.5f, Variable.Value);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MinCheck()
        {
            Variable.Min = 3.5f;
            Variable.Value = 3.4f;
        }

        [Test()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MaxCheck()
        {
            Variable.Max = 3.5f;
            Variable.Value = 3.6f;
        }
    }

    [TestFixture()]
    public class DoubleVariableProperties : RequiresEnvironment
    {
        private DoubleVariable Variable { get; set; }
        [SetUp] public new void Init() { Variable = new DoubleVariable(Bar); }

        [Test()]
        public void Value()
        {
            Variable.Value = 3.5;
            Assert.AreEqual(3.5, Variable.Value);
        }

        [Test()]
        public void Min()
        {
            Variable.Min = -5;
            Assert.AreEqual(-5, Variable.Min);
        }

        [Test()]
        public void Max()
        {
            Variable.Max = 30;
            Assert.AreEqual(30, Variable.Max);
        }

        [Test()]
        public void Step()
        {
            Variable.Step = 3;
            Assert.AreEqual(3, Variable.Step);
        }

        [Test()]
        public void Precision()
        {
            Variable.Precision = 5;
            Assert.AreEqual(5, Variable.Precision);
        }

        [Test()]
        public void MinConstraint()
        {
            Variable.Value = 3.5;
            Variable.Min = 3.7;
            Assert.AreEqual(3.7, Variable.Value);
        }

        [Test()]
        public void MaxConstraint()
        {
            Variable.Value = 3.5;
            Variable.Max = 2.5;
            Assert.AreEqual(2.5, Variable.Value);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MinCheck()
        {
            Variable.Min = 3.5;
            Variable.Value = 3.4;
        }

        [Test()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MaxCheck()
        {
            Variable.Max = 3.5;
            Variable.Value = 3.6;
        }
    }

    [TestFixture()]
    public class ColorVariableProperties : RequiresEnvironment
    {
        private ColorVariable Variable { get; set; }
        [SetUp] public new void Init() { Variable = new ColorVariable(Bar); }

        [Test()]
        public void Value()
        {
            Variable.R = 0.5f;
            Variable.G = 0.3f;
            Variable.B = 0.1f;
            Assert.AreEqual(0.5f, Variable.R);
            Assert.AreEqual(0.3f, Variable.G);
            Assert.AreEqual(0.1f, Variable.B);
        }

        [Test()]
        public void Mode()
        {
            Variable.Mode = ColorMode.HLS;
            Assert.AreEqual(ColorMode.HLS, Variable.Mode);
        }
    }

    [TestFixture()]
    public class VectorVariableProperties : RequiresEnvironment
    {
        private VectorVariable Variable { get; set; }
        [SetUp] public new void Init() { Variable = new VectorVariable(Bar); }

        [Test()]
        public void Value()
        {
            Variable.X = 0.5f;
            Variable.Y = 0.2f;
            Variable.Z = 0.3f;

            Assert.AreEqual(0.5f, Variable.X);
            Assert.AreEqual(0.2f, Variable.Y);
            Assert.AreEqual(0.3f, Variable.Z);
        }

        [Test()]
        public void ArrowColor()
        {
            Variable.ArrowColor = Color.FromArgb(75, 30, 50);
            Assert.AreEqual(Color.FromArgb(75, 30, 50), Variable.ArrowColor);
        }

        [Test()]
        public void ShowValue()
        {
            Variable.ShowValue = false;
            Assert.AreEqual(false, Variable.ShowValue);
        }

        [Test()]
        public void Coordinates()
        {
            var coords = new CoordinateSystem(AxisOrientation.PositiveX,
                                              AxisOrientation.NegativeZ,
                                              AxisOrientation.NegativeY);

            Variable.Coordinates = coords;
            Assert.AreEqual(coords, Variable.Coordinates);
        }

        [Test()]
        public void Coordinates2()
        {
            var coords = new CoordinateSystem(AxisOrientation.NegativeY,
                                              AxisOrientation.PositiveZ,
                                              AxisOrientation.PositiveX);

            Variable.Coordinates = coords;
            Assert.AreEqual(coords, Variable.Coordinates);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentException))]
        public void BadCoordinates()
        {
            var coords = new CoordinateSystem(AxisOrientation.NegativeY,
                                              AxisOrientation.NegativeZ,
                                              AxisOrientation.PositiveZ);

            Variable.Coordinates = coords;
        }
    }

    [TestFixture()]
    public class QuaternionVariableProperties : RequiresEnvironment
    {
        private QuaternionVariable Variable { get; set; }
        [SetUp] public new void Init() { Variable = new QuaternionVariable(Bar); }

        [Test()]
        public void Value()
        {
            Variable.X = 0.5f;
            Variable.Y = 0.2f;
            Variable.Z = 0.3f;
            Variable.W = -0.9f;

            Assert.AreEqual(0.5f, Variable.X);
            Assert.AreEqual(0.2f, Variable.Y);
            Assert.AreEqual(0.3f, Variable.Z);
            Assert.AreEqual(-0.9f, Variable.W);
        }

        [Test()]
        public void ArrowColor()
        {
            Variable.ArrowColor = Color.FromArgb(75, 30, 50);
            Assert.AreEqual(Color.FromArgb(75, 30, 50), Variable.ArrowColor);
        }

        [Test()]
        public void ShowValue()
        {
            Variable.ShowValue = false;
            Assert.AreEqual(false, Variable.ShowValue);
        }

        [Test()]
        public void Coordinates()
        {
            var coords = new CoordinateSystem(AxisOrientation.PositiveX,
                                              AxisOrientation.NegativeZ,
                                              AxisOrientation.NegativeY);

            Variable.Coordinates = coords;
            Assert.AreEqual(coords, Variable.Coordinates);
        }

        [Test()]
        public void Coordinates2()
        {
            var coords = new CoordinateSystem(AxisOrientation.NegativeY,
                                              AxisOrientation.PositiveZ,
                                              AxisOrientation.PositiveX);

            Variable.Coordinates = coords;
            Assert.AreEqual(coords, Variable.Coordinates);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentException))]
        public void BadCoordinates()
        {
            var coords = new CoordinateSystem(AxisOrientation.NegativeY,
                                              AxisOrientation.NegativeZ,
                                              AxisOrientation.PositiveZ);

            Variable.Coordinates = coords;
        }
    }

    [TestFixture()]
    public class EnumVariableProperties : RequiresEnvironment
    {
        private EnumVariable<TestEnum> Variable { get; set; }
        [SetUp] public new void Init() { Variable = new EnumVariable<TestEnum>(Bar, TestEnum.Choice1); }

        [Test()]
        public void Value()
        {
            Variable.Value = TestEnum.Choice2;
            Assert.AreEqual(TestEnum.Choice2, Variable.Value);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValueCheck()
        {
            Assert.IsFalse(Enum.IsDefined(typeof(TestEnum), (int)397528781));
            Variable.Value = (TestEnum)397528781; // won't be in enum range
        }
    }
}
