using System;
using System.Linq;
using System.Drawing;
using NUnit.Framework;

using AntTweakBar;

namespace Tests
{
    #region Test Types

    public enum MyEnum
    {
        Choice1 = 1,
        Choice2 = 1894,
        Choice3 = 2005,
    }

    public struct MyColor : IEquatable<MyColor>, ColorType
    {
        private float r, g, b;

        public float R { get { return r; } }
        public float G { get { return g; } }
        public float B { get { return b; } }

        public MyColor(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public bool Equals(MyColor other)
        {
            return (R == other.R && G == other.G && B == other.B);
        }

        // etc...
    }

    public struct MyVector : IEquatable<MyVector>, VectorType
    {
        private float x, y, z;

        public float X { get { return x; } }
        public float Y { get { return y; } }
        public float Z { get { return z; } }

        public MyVector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool Equals(MyVector other)
        {
            return (X == other.X && Y == other.Y && Z == other.Z);
        }

        // etc...
    }

    public struct MyQuaternion : IEquatable<MyQuaternion>, QuaternionType
    {
        private float x, y, z, w;

        public float X { get { return x; } }
        public float Y { get { return y; } }
        public float Z { get { return z; } }
        public float W { get { return w; } }

        public MyQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public bool Equals(MyQuaternion other)
        {
            return (X == other.X && Y == other.Y && Z == other.Z && W == other.W);
        }

        // etc...
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
            context = new Context(TW.GraphicsAPI.OpenGL);
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
            Bar.Alpha = 0.4f;
            Assert.AreEqual(0.4f, Bar.Alpha);
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
        private ColorVariable<MyColor> Variable { get; set; }
        [SetUp] public new void Init() { Variable = new ColorVariable<MyColor>(Bar); }

        [Test()]
        public void Value()
        {
            Variable.Value = new MyColor(0.5f, 0.3f, 0.1f);
            Assert.AreEqual(new MyColor(0.5f, 0.3f, 0.1f), Variable.Value);
        }

        [Test()]
        public void Mode()
        {
            Variable.Mode = ColorMode.HLS;
            Assert.AreEqual(ColorMode.HLS, Variable.Mode);
        }
    }

    [TestFixture()]
    public class ColorWrapper : RequiresEnvironment
    {
        private ColorVariable<ColorWrapper<MyColor>> Variable { get; set; }
        [SetUp] public new void Init() { Variable = new ColorVariable<ColorWrapper<MyColor>>(Bar); }

        [Test()]
        public void Value()
        {
            Variable.Value = new MyColor(0.5f, 0.3f, 0.1f);
            Assert.IsTrue(Variable.Value == new MyColor(0.5f, 0.3f, 0.1f));
        }
    }

    [TestFixture()]
    public class VectorVariableProperties : RequiresEnvironment
    {
        private VectorVariable<MyVector> Variable { get; set; }
        [SetUp] public new void Init() { Variable = new VectorVariable<MyVector>(Bar); }

        [Test()]
        public void Value()
        {
            Variable.Value = new MyVector(0.5f, 0.2f, 0.3f);
            Assert.AreEqual(new MyVector(0.5f, 0.2f, 0.3f), Variable.Value);
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
    }

    [TestFixture()]
    public class VectorWrapper : RequiresEnvironment
    {
        private VectorVariable<VectorWrapper<MyVector>> Variable { get; set; }
        [SetUp] public new void Init() { Variable = new VectorVariable<VectorWrapper<MyVector>>(Bar); }

        [Test()]
        public void Value()
        {
            Variable.Value = new MyVector(0.5f, 0.2f, 0.3f);
            Assert.IsTrue(Variable.Value == new MyVector(0.5f, 0.2f, 0.3f));
        }
    }

    [TestFixture()]
    public class QuaternionVariableProperties : RequiresEnvironment
    {
        private QuaternionVariable<MyQuaternion> Variable { get; set; }
        [SetUp] public new void Init() { Variable = new QuaternionVariable<MyQuaternion>(Bar); }

        [Test()]
        public void Value()
        {
            Variable.Value = new MyQuaternion(0.5f, 0.2f, 0.3f, 1.5f);
            Assert.AreEqual(new MyQuaternion(0.5f, 0.2f, 0.3f, 1.5f), Variable.Value);
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
    }

    [TestFixture()]
    public class QuaternionWrapper : RequiresEnvironment
    {
        private QuaternionVariable<QuaternionWrapper<MyQuaternion>> Variable { get; set; }
        [SetUp] public new void Init() { Variable = new QuaternionVariable<QuaternionWrapper<MyQuaternion>>(Bar); }

        [Test()]
        public void Value()
        {
            Variable.Value = new MyQuaternion(0.5f, 0.2f, 0.3f, 1.5f);
            Assert.IsTrue(Variable.Value == new MyQuaternion(0.5f, 0.2f, 0.3f, 1.5f));
        }
    }

    [TestFixture()]
    public class EnumVariableProperties : RequiresEnvironment
    {
        private EnumVariable<MyEnum> Variable { get; set; }
        [SetUp] public new void Init() { Variable = new EnumVariable<MyEnum>(Bar, MyEnum.Choice1); }

        [Test()]
        public void Value()
        {
            Variable.Value = MyEnum.Choice2;
            Assert.AreEqual(MyEnum.Choice2, Variable.Value);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValueCheck()
        {
            Assert.IsFalse(Enum.IsDefined(typeof(MyEnum), (int)397528781));
            Variable.Value = (MyEnum)397528781; // won't be in enum range
        }
    }
}
