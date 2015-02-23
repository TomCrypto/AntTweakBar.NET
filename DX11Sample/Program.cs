// basic D3D11 + AntTweakBar sample

using System;
using System.Windows.Forms;

using SharpDX;
using SharpDX.DXGI;
using SharpDX.Windows;

using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using DriverType = SharpDX.Direct3D.DriverType;

using AntTweakBar;

namespace DX11Sample
{
    /// <summary>
    /// This form is just like a RenderForm, except it has an AntTweakBar
    /// context and AntTweakBar events are handled transparently to the
    /// user. It does nothing special until you assign a Context to it.
    /// 
    /// Feel free to use it in your own applications, it's quite handy.
    /// </summary>
    public class ATBRenderForm : RenderForm
    {
        public Context Context { get; set; }

        public ATBRenderForm() : base() {}
        public ATBRenderForm(String text) : base(text) {}

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            /* We could have handled all the mouse and keyboard events
             * separately, but AntTweakBar has a handy function that
             * can automatically hook into the Windows message pump.
            */

            if ((Context == null) || !Context.EventHandlerWin(m.HWnd, m.Msg, m.WParam, m.LParam)) {
                base.WndProc(ref m);
            }
        }
    };

    /// <summary>
    /// All this class does is render a cube.
    /// </summary>
    public class Renderer : IDisposable
    {
        private readonly Form window;

        private readonly Device device;
        private readonly SwapChain swapChain;
        private readonly DeviceContext context;
        private readonly Factory factory;

        private Texture2D backbuffer;
        private RenderTargetView rtv;

        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private InputLayout layout;
        private Buffer vertexBuffer;

        public IntPtr DevicePointer
        {
            get { return device.NativePointer; }
        }

        public Vector3 P1Color { get; set; }
        public Vector3 P2Color { get; set; }
        public Vector3 P3Color { get; set; }
        public bool Wireframe { get; set; }
        public float Scale { get; set; }

        public void CreateGraphicsResources()
        {
            if (rtv != null) rtv.Dispose();
            if (backbuffer != null) backbuffer.Dispose();

            swapChain.ResizeBuffers(0, 0, 0, Format.Unknown, SwapChainFlags.None);
            backbuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            rtv = new RenderTargetView(device, backbuffer);
        }

        public Renderer(Form window)
        {
            // Default parameter values

            P1Color = new Vector3(1.0f, 0.0f, 0.0f);
            P2Color = new Vector3(0.0f, 1.0f, 0.0f);
            P3Color = new Vector3(0.0f, 0.0f, 1.0f);
            Wireframe = false;
            Scale = 0.5f;

            // Create device and swapchain

            this.window = window;

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, new SwapChainDescription()
            {
                BufferCount = 1,
                IsWindowed = true,
                Flags = SwapChainFlags.None,
                OutputHandle = window.Handle,
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
                SampleDescription = new SampleDescription(1, 0),
                ModeDescription = new ModeDescription()
                {
                    Width = 0,
                    Height = 0,
                    Format = Format.R8G8B8A8_UNorm,
                    RefreshRate = new Rational(60, 1),
                }
            }, out device, out swapChain);

            context = device.ImmediateContext;
            factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(window.Handle, WindowAssociationFlags.IgnoreAll);

            // Load shaders, create vertex buffers and stuff

            vertexBuffer = new Buffer(device, new BufferDescription()
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = (32 * 3),
                StructureByteStride = 16,
                Usage = ResourceUsage.Dynamic
            });

            var vertexShaderByteCode = ShaderBytecode.CompileFromFile("MiniTri.fx", "VS", "vs_4_0");
            vertexShader = new VertexShader(device, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("MiniTri.fx", "PS", "ps_4_0");
            pixelShader = new PixelShader(device, pixelShaderByteCode);

            // Get input layout from the vertex shader
            
            layout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
            });

            Resize(window.ClientSize); // first time resize
        }

        public void Resize(System.Drawing.Size size)
        {
            CreateGraphicsResources();
        }

        public void Render()
        {
            // Update the triangle vertex position/colors based on current parameters

            {
                var p1 = new Vector3(1.0f, 0.0f, 0.5f);
                var p2 = new Vector3(-0.5f, 0.866f, 0.5f);
                var p3 = new Vector3(-0.5f, -0.866f, 0.5f);

                DataStream vertexStream;
                context.MapSubresource(vertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out vertexStream);
                vertexStream.Write<Vector4>(new Vector4(p1 * Scale, 1.0f));
                vertexStream.Write<Vector4>(new Vector4(P1Color, 1.0f));
                vertexStream.Write<Vector4>(new Vector4(p2 * Scale, 1.0f));
                vertexStream.Write<Vector4>(new Vector4(P2Color, 1.0f));
                vertexStream.Write<Vector4>(new Vector4(p3 * Scale, 1.0f));
                vertexStream.Write<Vector4>(new Vector4(P3Color, 1.0f));
                context.UnmapSubresource(vertexBuffer, 0);
                vertexStream.Dispose();
            }

            // Set up the pipeline state in order to render our triangle

            context.Rasterizer.SetViewport(new Viewport(0, 0, window.Width, window.Height, 0.0f, 1.0f));
            context.Rasterizer.State = new RasterizerState(device, new RasterizerStateDescription()
            {
                FillMode = Wireframe ? FillMode.Wireframe : FillMode.Solid,
                CullMode = CullMode.None,
            });

            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 32, 0));

            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);
            context.OutputMerger.SetTargets(rtv);

            // Now ready to draw the triangle! (clear the background first)

            context.ClearRenderTargetView(rtv, new Color(0.1f, 0.1f, 0.45f));
            context.Draw(3, 0);
        }

        public void Present()
        {
            swapChain.Present(1, PresentFlags.None);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {

                rtv.Dispose();
                backbuffer.Dispose();
                vertexShader.Dispose();
                pixelShader.Dispose();
                layout.Dispose();
                vertexBuffer.Dispose();
                factory.Dispose();
                context.Dispose();
                swapChain.Dispose();
                device.Dispose();
            }
        }
    }

    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            using (var form = new ATBRenderForm("DX11 AntTweakBar Sample"))
            {
                using (var renderer = new Renderer(form))
                {
                    // At this point we can initialize the AntTweakBar context
                    // (since the renderer has now created the D3D device)

                    using (var context = new Context(Tw.GraphicsAPI.D3D11, renderer.DevicePointer))
                    {
                        form.Context = context;

                        // Add a bar with some variables in it

                        var exampleBar = new Bar(form.Context);
                        exampleBar.Label = "Example Bar";
                        exampleBar.Contained = true;

                        // here we bind the variables to the renderer variables with their Changed event
                        // (their initial value is whatever the renderer currently has set them to)

                        var color1Var = new ColorVariable(exampleBar, renderer.P1Color.X,
                                                                      renderer.P1Color.Y,
                                                                      renderer.P1Color.Z);
                        color1Var.Label = "P1 color";
                        color1Var.Changed += delegate { renderer.P1Color = new Vector3(color1Var.R,
                                                                                       color1Var.G,
                                                                                       color1Var.G); };

                        var color2Var = new ColorVariable(exampleBar, renderer.P2Color.X,
                                                                      renderer.P2Color.Y,
                                                                      renderer.P2Color.Z);
                        color2Var.Label = "P2 color";
                        color2Var.Changed += delegate { renderer.P2Color = new Vector3(color2Var.R,
                                                                                       color2Var.G,
                                                                                       color2Var.G); };

                        var color3Var = new ColorVariable(exampleBar, renderer.P3Color.X,
                                                                      renderer.P3Color.Y,
                                                                      renderer.P3Color.Z);
                        color3Var.Label = "P3 color";
                        color3Var.Changed += delegate { renderer.P3Color = new Vector3(color3Var.R,
                                                                                       color3Var.G,
                                                                                       color3Var.G); };

                        // Put the color selection variables in their own group

                        var colorGroup = new Group(exampleBar, "Colors", color1Var, color2Var, color3Var);

                        var wireframeVar = new BoolVariable(exampleBar);
                        wireframeVar.Label = "Wireframe";
                        wireframeVar.Changed += delegate { renderer.Wireframe = wireframeVar.Value; };

                        var scaleVar = new FloatVariable(exampleBar, 1.0f);
                        scaleVar.Label = "Scale";
                        scaleVar.Changed += delegate { renderer.Scale = scaleVar.Value; };
                        scaleVar.SetDefinition("min=-2 max=2 step=0.01 precision=2");

                        var separator = new Separator(exampleBar);

                        var testButton = new AntTweakBar.Button(exampleBar);
                        testButton.Label = "Click me!";
                        testButton.Clicked += delegate { MessageBox.Show("Button Clicked!"); };

                        // The renderer needs to recreate some resources on window resize

                        form.Resize += delegate { renderer.Resize(form.ClientSize); };

                        // In the main loop, render stuff, then the bar(s), then present to screen

                        RenderLoop.Run(form, () =>
                        {
                            renderer.Render();
                            form.Context.Draw();
                            renderer.Present();
                        });
                    }
                }
            }
        }
    }
}
