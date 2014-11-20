using System;
using System.Drawing;
using System.Numerics;
using System.Collections.Generic;

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

        private bool hardcodePolynomial;
        public bool HardcodePolynomial
        {
            get { return hardcodePolynomial; }
            set
            {
                hardcodePolynomial = value;
                
                SetupShaders();

                if (!hardcodePolynomial)
                    UploadPolynomial();
            }
        }

        private Polynomial polynomial;
        public Polynomial Polynomial
        {
            get { return polynomial; }
            set
            {
                polynomial = value;

                if (!hardcodePolynomial)
                    UploadPolynomial();
                else
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
        
        private void UploadPolynomial()
        {
            var polyRoots = Polynomial.Roots(polynomial);

            GL.Uniform1(GL.GetUniformLocation(shHandle, "polyCoeffCount"), polyRoots.Item1.Count + 1);
            GL.Uniform2(GL.GetUniformLocation(shHandle, "polyCoeffs[0]"), new Vector2((float)polyRoots.Item2.Real, (float)polyRoots.Item2.Imaginary));
            
            int t = 1;
            
            foreach (var root in polyRoots.Item1) {
                GL.Uniform2(GL.GetUniformLocation(shHandle, String.Format("polyCoeffs[{0}]", t)), new Vector2((float)root.Real, (float)root.Imaginary));
                
                ++t;
            }

            var dervRoots = Polynomial.Roots(Polynomial.Derivative(polynomial));

            GL.Uniform1(GL.GetUniformLocation(shHandle, "dervCoeffCount"), dervRoots.Item1.Count + 1);
            GL.Uniform2(GL.GetUniformLocation(shHandle, "dervCoeffs[0]"), new Vector2((float)dervRoots.Item2.Real, (float)dervRoots.Item2.Imaginary));
            
            t = 1;
            
            foreach (var root in dervRoots.Item1) {
                GL.Uniform2(GL.GetUniformLocation(shHandle, String.Format("dervCoeffs[{0}]", t)), new Vector2((float)root.Real, (float)root.Imaginary));
                
                ++t;
            }
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
            GL.ShaderSource(fsHandle, Shader.FragShader(polynomial, shading, aa, hardcodePolynomial));

            GL.CompileShader(vsHandle);
            GL.CompileShader(fsHandle);

            String log;

            GL.GetShaderInfoLog(vsHandle, out log);
            if (log.Trim() != "") {
                Console.WriteLine("====  COMPILATION LOG FOR VERTEX SHADER  ====\n");
                Console.WriteLine(log.Trim() + "\n");
                Console.WriteLine("====");
            }

            GL.GetShaderInfoLog(fsHandle, out log);
            if (log.Trim() != "") {
                Console.WriteLine("====  COMPILATION LOG FOR FRAGMENT SHADER  ====\n");
                Console.WriteLine(log.Trim() + "\n");
                Console.WriteLine("====");
            }
        }

        private void CreateProgram()
        {
            shHandle = GL.CreateProgram();

            GL.AttachShader(shHandle, vsHandle);
            GL.AttachShader(shHandle, fsHandle);

            GL.LinkProgram(shHandle);

            String log;

            GL.GetShaderInfoLog(shHandle, out log);
            if (log.Trim() != "") {
                Console.WriteLine("====   LINKAGE LOG FOR FRACTAL PROGRAM   ====\n");
                Console.WriteLine(log.Trim() + "\n");
                Console.WriteLine("====");
            }
        }

        private void SetupOptions()
        {
            zoom = 2.4f;
            offset = Vector2.Zero;
            iterations = 128;
            aCoeff = Complex.One;
            kCoeff = Complex.Zero;
            aa = AAQuality.AAx1;
            hardcodePolynomial = true;

            intensity = 1;
            palette = Color4.Red;
            shading = ShadingType.Standard;

            polynomial = Polynomial.Parse(DefaultPolynomial, new Dictionary<String, Double>());
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

