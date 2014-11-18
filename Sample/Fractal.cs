using System;
using System.Drawing;
using System.Numerics;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Sample
{
    public enum ShadingType
    {
        Standard,
        Negative,
        Flat
    }

    public enum AAQuality
    {
        AAx1 = 1,
        AAx4 = 2,
        AAx9 = 3,
        AAx16 = 4
    }

    public class Fractal : IDisposable
    {
        public const String DefaultPolynomial = "z^3 - 1";

        int vsHandle, fsHandle, shHandle;

        private Vector2 offset;
        public Vector2 Offset
        {
            get { return offset; }
            set { GL.Uniform2(GL.GetUniformLocation(shHandle, "offset"), offset = value); }
        }

        private float zoom;
        public float Zoom
        {
            get { return zoom; }
            set { GL.Uniform1(GL.GetUniformLocation(shHandle, "zoom"), zoom = value); }
        }

        private Size dimensions;
        public Size Dimensions
        {
            get { return dimensions; }
            set
            {
                GL.Viewport(dimensions = value);
                GL.Uniform2(GL.GetUniformLocation(shHandle, "dims"), new Vector2(dimensions.Width, dimensions.Height));
            }
        }

        private int iterations;
        public int Iterations
        {
            get { return iterations; }
            set { GL.Uniform1(GL.GetUniformLocation(shHandle, "iters"), (uint)(iterations = value)); }
        }

        private Polynomial polynomial;
        public Polynomial Polynomial
        {
            get { return polynomial; }
            set
            {
                polynomial = value;
                SetupShaders();
            }
        }

        private Color4 palette;
        public Color4 Palette
        {
            get { return palette; }
            set { GL.Uniform4(GL.GetUniformLocation(shHandle, "palette"), palette = value); }
        }

        private ShadingType shading;
        public ShadingType Shading
        {
            get { return shading; }
            set
            {
                shading = value;
                SetupShaders();
            }
        }

        private AAQuality aa;
        public AAQuality AA
        {
            get { return aa; }
            set
            {
                aa = value;
                SetupShaders();
            }
        }

        private Complex aCoeff;
        public Complex ACoeff
        {
            get { return aCoeff; }
            set
            {
                aCoeff = value;
                GL.Uniform2(GL.GetUniformLocation(shHandle, "aCoeff"), new Vector2((float)aCoeff.Real, (float)aCoeff.Imaginary));
            }
        }

        private Complex kCoeff;
        public Complex KCoeff
        {
            get { return kCoeff; }
            set
            {
                kCoeff = value;
                GL.Uniform2(GL.GetUniformLocation(shHandle, "kCoeff"), new Vector2((float)kCoeff.Real, (float)kCoeff.Imaginary));
            }
        }

        private float intensity;
        public float Intensity
        {
            get { return intensity; }
            set { GL.Uniform1(GL.GetUniformLocation(shHandle, "intensity"), intensity = value); }
        }

        private void SetShaderVariables()
        {
            Zoom = zoom;
            Offset = offset;
            Intensity = intensity;
            Dimensions = dimensions;
            ACoeff = aCoeff;
            KCoeff = kCoeff;

            Iterations = iterations;
            Palette = palette;
        }

        public void ZoomIn(float amount)
        {
            Zoom *= (float)Math.Pow(1.1, -amount);
        }

        public void Pan(float dx, float dy)
        {
            Offset += new Vector2(dx, dy) * zoom;
        }

        public Fractal()
        {
            SetupOptions();
            SetupShaders();
        }

        private void SetupShaders()
        {
            if (shHandle != 0)
            {
                GL.DeleteProgram(shHandle);
                GL.DeleteShader(vsHandle);
                GL.DeleteShader(fsHandle);
            }

            CreateShaders();
            CreateProgram();

            GL.UseProgram(shHandle);

            SetShaderVariables();
        }

        private void CreateShaders()
        {
            vsHandle = GL.CreateShader(ShaderType.VertexShader);
            fsHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vsHandle, Shader.VertShader());
            GL.ShaderSource(fsHandle, Shader.FragShader(polynomial, shading, aa));

            GL.CompileShader(vsHandle);
            GL.CompileShader(fsHandle);

            String log;

            Console.WriteLine("====  COMPILATION LOG FOR VERTEX SHADER  ====\n");
            GL.GetShaderInfoLog(vsHandle, out log);
            if (log == "") Console.WriteLine ("(empty)\n");
            else Console.WriteLine(log + "\n");

            Console.WriteLine("==== COMPILATION LOG FOR FRAGMENT SHADER ====\n");
            GL.GetShaderInfoLog(fsHandle, out log);
            if (log == "") Console.WriteLine ("(empty)\n");
            else Console.WriteLine(log + "\n");
        }

        private void CreateProgram()
        {
            shHandle = GL.CreateProgram();

            GL.AttachShader(shHandle, vsHandle);
            GL.AttachShader(shHandle, fsHandle);

            GL.LinkProgram(shHandle);

            String log;

            Console.WriteLine("====   LINKAGE LOG FOR FRACTAL PROGRAM   ====\n");
            GL.GetShaderInfoLog(shHandle, out log);
            if (log == "") Console.WriteLine ("(empty)\n");
            else Console.WriteLine(log + "\n");
            Console.WriteLine("====");
        }

        private void SetupOptions()
        {
            zoom = 2.4f;
            offset = Vector2.Zero;
            iterations = 128;
            aCoeff = Complex.One;
            kCoeff = Complex.Zero;
            aa = AAQuality.AAx1;

            intensity = 1;
            palette = Color4.Red;
            shading = ShadingType.Standard;

            polynomial = Polynomial.Parse(DefaultPolynomial);
        }

        public void Draw()
        {
            GL.Begin(PrimitiveType.Quads);
            {
                GL.Vertex2(-1.0f, -1.0f);
                GL.Vertex2(+1.0f, -1.0f);
                GL.Vertex2(+1.0f, +1.0f);
                GL.Vertex2(-1.0f, +1.0f);
            }
            GL.End();
        }

        #region IDisposable

        ~Fractal()
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

            if (disposing)
            {
                GL.DeleteProgram(shHandle);
                GL.DeleteShader(vsHandle);
                GL.DeleteShader(fsHandle);
            }

            disposed = true;
        }

        private bool disposed = false;

        #endregion
    }
}

