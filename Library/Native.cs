using System;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    /// <summary>
    /// The native AntTweakBar calls.
    /// </summary>
    internal static class NativeMethods
    {
        /* For libAntTweakBar.so, AntTweakBar.dll. */
        private const String DLLName = "AntTweakBar";

        [DllImport(DLLName, EntryPoint = "TwGetLastError")]
        public static extern IntPtr TwGetLastError();

        [DllImport(DLLName, EntryPoint = "TwInit")]
        public static extern Boolean TwInit(
            [In] TW.GraphicsAPI graphicsAPI,
            [In] IntPtr device);

        [DllImport(DLLName, EntryPoint = "TwTerminate")]
        public static extern Boolean TwTerminate();

        [DllImport(DLLName, EntryPoint = "TwDraw")]
        public static extern Boolean TwDraw();

        [DllImport(DLLName, EntryPoint = "TwHandleErrors")]
        public static extern void TwHandleErrors(
            [In] TW.ErrorHandler handler);

        [DllImport(DLLName, EntryPoint = "TwWindowSize")]
        public static extern Boolean TwWindowSize(
            [In] Int32 width,
            [In] Int32 height);

        [DllImport(DLLName, EntryPoint = "TwMouseMotion")]
        public static extern Boolean TwMouseMotion(
            [In] Int32 mouseX,
            [In] Int32 mouseY);

        [DllImport(DLLName, EntryPoint = "TwMouseWheel")]
        public static extern Boolean TwMouseWheel(
            [In] Int32 pos);

        [DllImport(DLLName, EntryPoint = "TwMouseButton")]
        public static extern Boolean TwMouseButton(
            [In] TW.MouseAction action,
            [In] TW.MouseButton button);

        [DllImport(DLLName, EntryPoint = "TwKeyPressed")]
        public static extern Boolean TwKeyPressed(
            [In] Int32 key,
            [In] TW.KeyModifier modifiers);

        [DllImport(DLLName, EntryPoint = "TwEventSFML")]
        public static extern Boolean TwEventSFML(
            [In] IntPtr sfmlEvent,
            [In] Byte major,
            [In] Byte minor);

        [DllImport(DLLName, EntryPoint = "TwEventSDL")]
        public static extern Boolean TwEventSDL(
            [In] IntPtr sdlEvent,
            [In] Byte major,
            [In] Byte minor);

        [DllImport(DLLName, EntryPoint = "TwEventWin")]
        public static extern Boolean TwEventWin(
            [In] IntPtr wnd,
            [In] Int32 msg,
            [In] IntPtr wParam,
            [In] IntPtr lParam);

        [DllImport(DLLName, EntryPoint = "TwSetCurrentWindow")]
        public static extern Boolean TwSetCurrentWindow(
            [In] Int32 windowID);

        [DllImport(DLLName, EntryPoint = "TwGetCurrentWindow")]
        public static extern Int32 TwGetCurrentWindow();

        [DllImport(DLLName, EntryPoint = "TwWindowExists")]
        public static extern Boolean TwWindowExists(
            [In] Int32 windowID);

        [DllImport(DLLName, EntryPoint = "TwNewBar", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern IntPtr TwNewBar(
            [In, MarshalAs(UnmanagedType.LPStr)] String barName);

        [DllImport(DLLName, EntryPoint = "TwDeleteBar")]
        public static extern Boolean TwDeleteBar(
            [In] IntPtr bar);

        [DllImport(DLLName, EntryPoint = "TwSetTopBar")]
        public static extern Boolean TwSetTopBar(
            [In] IntPtr bar);

        [DllImport(DLLName, EntryPoint = "TwSetBottomBar")]
        public static extern Boolean TwSetBottomBar(
            [In] IntPtr bar);

        [DllImport(DLLName, EntryPoint = "TwRefreshBar")]
        public static extern Boolean TwRefreshBar(
            [In] IntPtr bar);

        [DllImport(DLLName, EntryPoint = "TwDefine", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwDefine(
            [In, MarshalAs(UnmanagedType.LPStr)] String def);

        [DllImport(DLLName, EntryPoint = "TwSetParam", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwSetParamStr(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] TW.ParamValueType type,
            [In] uint reserved,
            [In, MarshalAs(UnmanagedType.LPStr)] String param);

        [DllImport(DLLName, EntryPoint = "TwSetParam", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwSetParamInt(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] TW.ParamValueType type,
            [In] uint elemCount,
            [In] Int32[] param);

        [DllImport(DLLName, EntryPoint = "TwSetParam", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwSetParamSingle(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] TW.ParamValueType type,
            [In] uint elemCount,
            [In] Single[] param);

        [DllImport(DLLName, EntryPoint = "TwSetParam", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwSetParamDouble(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] TW.ParamValueType type,
            [In] uint elemCount,
            [In] Double[] param);

        [DllImport(DLLName, EntryPoint = "TwGetParam", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Int32 TwGetParamStr(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] TW.ParamValueType type,
            [In] uint maxStrLength,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder param);

        [DllImport(DLLName, EntryPoint = "TwGetParam", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Int32 TwGetParamInt(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] TW.ParamValueType type,
            [In] uint maxElemCount,
            [Out] Int32[] param);

        [DllImport(DLLName, EntryPoint = "TwGetParam", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Int32 TwGetParamSingle(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] TW.ParamValueType type,
            [In] uint maxElemCount,
            [Out] Single[] param);

        [DllImport(DLLName, EntryPoint = "TwGetParam", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Int32 TwGetParamDouble(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] TW.ParamValueType type,
            [In] uint maxElemCount,
            [Out] Double[] param);

        [DllImport(DLLName, EntryPoint = "TwAddVarRO", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwAddVarRO(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String name,
            [In] TW.VariableType type,
            [In] IntPtr var,
            [In, MarshalAs(UnmanagedType.LPStr)] String def);

        [DllImport(DLLName, EntryPoint = "TwAddVarRW", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwAddVarRW(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String name,
            [In] TW.VariableType type,
            [In] IntPtr var,
            [In, MarshalAs(UnmanagedType.LPStr)] String def);

        [DllImport(DLLName, EntryPoint = "TwAddVarCB", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwAddVarCB(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String name,
            [In] TW.VariableType type,
            [In] TW.SetVarCallback setCallback,
            [In] TW.GetVarCallback getCallback,
            [In] IntPtr clientData,
            [In, MarshalAs(UnmanagedType.LPStr)] String def);

        [DllImport(DLLName, EntryPoint = "TwAddSeparator", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwAddSeparator(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String name,
            [In, MarshalAs(UnmanagedType.LPStr)] String def);

        [DllImport(DLLName, EntryPoint = "TwAddButton", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwAddButton(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String name,
            [In] TW.ButtonCallback callback,
            [In] IntPtr clientData,
            [In, MarshalAs(UnmanagedType.LPStr)] String def);

        [DllImport(DLLName, EntryPoint = "TwDefineEnumFromString", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern TW.VariableType TwDefineEnumFromString(
            [In, MarshalAs(UnmanagedType.LPStr)] String name,
            [In, MarshalAs(UnmanagedType.LPStr)] String enumString);

        [DllImport(DLLName, EntryPoint = "TwRemoveVar", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwRemoveVar(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String name);
    }

    /// <summary>
    /// A low-level wrapper to the AntTweakBar API.
    /// </summary>
    public static partial class TW
    {
        public static String GetLastError()
        {
            return Marshal.PtrToStringAnsi(NativeMethods.TwGetLastError());
        }

        public static void Init(GraphicsAPI graphicsAPI, IntPtr device)
        {
             if (!NativeMethods.TwInit(graphicsAPI, device)) {
                 throw new AntTweakBarException("TwInit failed.");
             }
        }

        public static void Terminate()
        {
            if (!NativeMethods.TwTerminate()) {
                throw new AntTweakBarException("TwTerminate failed.");
            }
        }

        public static void Draw()
        {
            if (!NativeMethods.TwDraw()) {
                throw new AntTweakBarException("TwDraw failed.");
            }
        }

        public static void WindowSize(int width, int height)
        {
            if (!NativeMethods.TwWindowSize(width, height))
                throw new AntTweakBarException("TwWindowSize failed.");
        }

        public static bool MouseMotion(int mouseX, int mouseY)
        {
            return NativeMethods.TwMouseMotion(mouseX, mouseY);
        }

        public static bool MouseWheel(int pos)
        {
            return NativeMethods.TwMouseWheel(pos);
        }

        public static bool MouseClick(MouseAction action, MouseButton button)
        {
            return NativeMethods.TwMouseButton(action, button);
        }

        public static bool KeyPressed(int key, KeyModifier modifiers)
        {
            return NativeMethods.TwKeyPressed(key, modifiers);
        }

        public static bool EventSFML(IntPtr sfmlEvent, byte majorVersion, byte minorVersion)
        {
            return NativeMethods.TwEventSFML(sfmlEvent, majorVersion, minorVersion);
        }

        public static bool EventSDL(IntPtr sdlEvent, byte majorVersion, byte minorVersion)
        {
            return NativeMethods.TwEventSDL(sdlEvent, majorVersion, minorVersion);
        }

        public static bool EventWin(IntPtr wnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            return NativeMethods.TwEventWin(wnd, msg, wParam, lParam);
        }

        public static void SetCurrentWindow(int windowID)
        {
            if (!NativeMethods.TwSetCurrentWindow(windowID)) {
                throw new AntTweakBarException("TwSetCurrentWindow failed.");
            }
        }

        public static int GetCurrentWindow()
        {
            return NativeMethods.TwGetCurrentWindow();
        }

        public static bool WindowExists(int windowID)
        {
            return NativeMethods.TwWindowExists(windowID);
        }

        public static IntPtr NewBar(string barName)
        {
            IntPtr bar;

            if ((bar = NativeMethods.TwNewBar(barName)) == IntPtr.Zero) {
                throw new AntTweakBarException("TwNewBar failed.");
            }
            
            return bar;
        }

        public static void DeleteBar(IntPtr bar)
        {
            if (!NativeMethods.TwDeleteBar(bar)) {
                throw new AntTweakBarException("TwDeleteBar failed.");
            }
        }

        public static void SetTopBar(IntPtr bar)
        {
            if (!NativeMethods.TwSetTopBar(bar)) {
                throw new AntTweakBarException("TwSetTopBar failed.");
            }
        }

        public static void SetBottomBar(IntPtr bar)
        {
            if (!NativeMethods.TwSetBottomBar(bar)) {
                throw new AntTweakBarException("TwSetBottomBar failed.");
            }
        }

        public static void RefreshBar(IntPtr bar)
        {
            if (!NativeMethods.TwRefreshBar(bar)) {
                throw new AntTweakBarException("TwRefreshBar failed.");
            }
        }

        public static void Define(String def)
        {
            if (!NativeMethods.TwDefine(def)) {
                throw new AntTweakBarException("TwDefine failed.");
            }
        }

        public static void SetParam(IntPtr bar, String varName, String paramName, String value)
        {
            if (!NativeMethods.TwSetParamStr(bar, varName, paramName, ParamValueType.CString, 1, value)) {
                throw new AntTweakBarException("TwSetParam failed.");
            }
        }

        public static void SetParam(IntPtr bar, String varName, String paramName, params Int32[] values)
        {
            if (!NativeMethods.TwSetParamInt(bar, varName, paramName, ParamValueType.Int32, (uint)values.Length, values)) {
                throw new AntTweakBarException("TwSetParam failed.");
            }
        }

        public static void SetParam(IntPtr bar, String varName, String paramName, params Single[] values)
        {
            if (!NativeMethods.TwSetParamSingle(bar, varName, paramName, ParamValueType.Float, (uint)values.Length, values)) {
                throw new AntTweakBarException("TwSetParam failed.");
            }
        }

        public static void SetParam(IntPtr bar, String varName, String paramName, params Double[] values)
        {
            if (!NativeMethods.TwSetParamDouble(bar, varName, paramName, ParamValueType.Double, (uint)values.Length, values)) {
                throw new AntTweakBarException("TwSetParam failed.");
            }
        }
        
        public static void SetParam(IntPtr bar, String varName, String paramName, Color color)
        {
            SetParam(bar, varName, paramName, color.R, color.G, color.B);
        }

        public static void SetParam(IntPtr bar, String varName, String paramName, Boolean boolean)
        {
            SetParam(bar, varName, paramName, boolean ? "true" : "false");
        }

        public static void SetParam(IntPtr bar, String varName, String paramName, Point point)
        {
            SetParam(bar, varName, paramName, point.X, point.Y);
        }

        public static void SetParam(IntPtr bar, String varName, String paramName, Size size)
        {
            SetParam(bar, varName, paramName, size.Width, size.Height);
        }

        public static String GetStringParam(IntPtr bar, String varName, String paramName)
        {
            var buffer = new StringBuilder(MaxStringLength);

            if (NativeMethods.TwGetParamStr(bar, varName, paramName, ParamValueType.CString, (uint)buffer.Capacity, buffer) == 0) {
                throw new AntTweakBarException("TwGetParam failed.");
            }

            return buffer.ToString();
        }

        public static Int32[] GetIntParam(IntPtr bar, String varName, String paramName, int paramCount = 0)
        {
            var buffer = new Int32[paramCount == 0 ? 32 : paramCount];

            int count = NativeMethods.TwGetParamInt(bar, varName, paramName, ParamValueType.Int32, (uint)buffer.Length, buffer);
            if (count == 0) throw new AntTweakBarException("TwGetParam failed.");

            if (paramCount == 0) {
                var outBuf = new Int32[count];
                Array.Copy(buffer, outBuf, count);
                return outBuf;
            } else {
                return buffer;
            }
        }

        public static Single[] GetSingleParam(IntPtr bar, String varName, String paramName, int paramCount = 0)
        {
            var buffer = new Single[paramCount == 0 ? 32 : paramCount];

            int count = NativeMethods.TwGetParamSingle(bar, varName, paramName, ParamValueType.Float, (uint)buffer.Length, buffer);
            if (count == 0) throw new AntTweakBarException("TwGetParam failed.");

            if (paramCount == 0) {
                var outBuf = new Single[count];
                Array.Copy(buffer, outBuf, count);
                return outBuf;
            } else {
                return buffer;
            }
        }

        public static Double[] GetDoubleParam(IntPtr bar, String varName, String paramName, int paramCount = 0)
        {
            var buffer = new Double[paramCount == 0 ? 32 : paramCount];

            int count = NativeMethods.TwGetParamDouble(bar, varName, paramName, ParamValueType.Double, (uint)buffer.Length, buffer);
            if (count == 0) throw new AntTweakBarException("TwGetParam failed.");

            if (paramCount == 0) {
                var outBuf = new Double[count];
                Array.Copy(buffer, outBuf, count);
                return outBuf;
            } else {
                return buffer;
            }
        }

        public static Color GetColorParam(IntPtr bar, String varName, String paramName)
        {
            var components = GetIntParam(bar, varName, paramName);

            if (components.Length != 3) {
                throw new ArgumentException("Parameter is not a color.");
            }

            for (var t = 0; t < 3; ++t) {
                if ((components[t] < 0) || (components[t] > 255)) {
                    throw new ArgumentException("Parameter is not a color.");
                }
            }

            return Color.FromArgb(components[0], components[1], components[2]);
        }

        public static Boolean GetBooleanParam(IntPtr bar, String varName, String paramName)
        {
            switch (GetStringParam(bar, varName, paramName))
            {
                case "true":
                    return true;
                case "false":
                    return false;
                case "1":
                    return true;
                case "0":
                    return false;
                default:
                    throw new ArgumentException("Parameter is not a boolean value.");
            }
        }

        public static Point GetPointParam(IntPtr bar, String varName, String paramName)
        {
            var components = GetIntParam(bar, varName, paramName);

            if (components.Length != 2) {
                throw new ArgumentException("Parameter is not a point.");
            }

            return new Point(components[0], components[1]);
        }

        public static Size GetSizeParam(IntPtr bar, String varName, String paramName)
        {
            var components = GetIntParam(bar, varName, paramName);

            if (components.Length != 2) {
                throw new ArgumentException("Parameter is not a size.");
            }

            for (var t = 0; t < 2; ++t) {
                if (components[t] <= 0) {
                    throw new ArgumentException("Parameter is not a size.");
                }
            }

            return new Size(components[0], components[1]);
        }

        public static void AddVarRO(IntPtr bar, String name, VariableType type, IntPtr var, String def)
        {
            if (!NativeMethods.TwAddVarRO(bar, name, type, var, def)) {
                throw new AntTweakBarException("TwAddVarRO failed.");
            }
        }

        public static void AddVarRW(IntPtr bar, String name, VariableType type, IntPtr var, String def)
        {
            if (!NativeMethods.TwAddVarRW(bar, name, type, var, def)) {
                throw new AntTweakBarException("TwAddVarRW failed.");
            }
        }

        public static void AddVarCB(IntPtr bar, String name, VariableType type, SetVarCallback setCallback, GetVarCallback getCallback, IntPtr clientData, String def)
        {
            if (!NativeMethods.TwAddVarCB(bar, name, type, setCallback, getCallback, clientData, def)) {
                throw new AntTweakBarException("TwAddVarCB failed.");
            }
        }

        public static void AddSeparator(IntPtr bar, String name, String def)
        {
            if (!NativeMethods.TwAddSeparator(bar, name, def)) {
                throw new AntTweakBarException("TwAddSeparator failed.");
            }
        }

        public static void AddButton(IntPtr bar, String name, ButtonCallback callback, IntPtr clientData, String def)
        {
            if (!NativeMethods.TwAddButton(bar, name, callback, clientData, def)) {
                throw new AntTweakBarException("TwAddButton failed.");
            }
        }

        public static VariableType DefineEnumFromString(String name, String enumString)
        {
            VariableType enumType;

            if ((enumType = NativeMethods.TwDefineEnumFromString(name, enumString)) == VariableType.Undefined) {
                throw new AntTweakBarException("TwDefineEnumFromString failed.");
            }
             
            return enumType;
        }

        public static void RemoveVar(IntPtr bar, String name)
        {
            if (!NativeMethods.TwRemoveVar(bar, name)) {
                throw new AntTweakBarException("TwRemoveVar failed.");
            }
        }
    }
}