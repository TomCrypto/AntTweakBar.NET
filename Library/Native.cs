using System;
using System.Linq;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    /// <summary>
    /// The native AntTweakBar API.
    /// </summary>
    public static class TW
    {
        // Matches libAntTweakBar.so, AntTweakBar.dll
        private const String DLLName = "AntTweakBar";

        #region Native Types

        /// <summary>
        /// The graphics API's AntTweakBar can use.
        /// </summary>
        public enum GraphicsAPI
        {
            /// <summary>
            /// Graphics API already provided.
            /// </summary>
            Unspecified = 0,

            /// <summary>
            /// OpenGL (compatibility profile).
            /// </summary>
            OpenGL      = 1,

            /// <summary>
            /// Direct3D 9.
            /// </summary>
            D3D9        = 2,

            /// <summary>
            /// Direct3D 10.
            /// </summary>
            D3D10       = 3,

            /// <summary>
            /// Direct3D 11.
            /// </summary>
            D3D11       = 4,

            /// <summary>
            /// OpenGL core profile (OpenGL 3.2 and higher).
            /// </summary>
            OpenGLCore  = 5
        }

        /// <summary>
        /// Parameter value types for SetParam.
        /// </summary>
        internal enum ParamValueType
        {
            TW_PARAM_INT32,
            TW_PARAM_FLOAT,
            TW_PARAM_DOUBLE,
            TW_PARAM_CSTRING,
        }

        /// <summary>
        /// Maximum static string length.
        /// </summary>
        internal const int MaxStringLength = 2048;

        /// <summary>
        /// Variable type (excluding enums).
        /// </summary>
        internal enum VariableType
        {
            TW_TYPE_UNDEF   = 0,
            TW_TYPE_BOOL8   = 2,
            TW_TYPE_BOOL16,
            TW_TYPE_BOOL32,
            TW_TYPE_CHAR,
            TW_TYPE_INT8,
            TW_TYPE_UINT8,
            TW_TYPE_INT16,
            TW_TYPE_UINT16,
            TW_TYPE_INT32,
            TW_TYPE_UINT32,
            TW_TYPE_FLOAT,
            TW_TYPE_DOUBLE,
            TW_TYPE_COLOR32,
            TW_TYPE_COLOR3F,
            TW_TYPE_COLOR4F,
            TW_TYPE_CDSTRING,
            TW_TYPE_QUAT4F = TW_TYPE_CDSTRING+2,
            TW_TYPE_QUAT4D,
            TW_TYPE_DIR3F,
            TW_TYPE_DIR3D,
            TW_TYPE_CSSTRING = 0x30000000 + MaxStringLength,
        };

        /// <summary>
        /// Possible mouse actions.
        /// </summary>
        public enum MouseAction
        {
            Released,
            Pressed,
        }

        /// <summary>
        /// Possible mouse buttons.
        /// </summary>
        public enum MouseButton
        {
            Left = 1,
            Middle = 2,
            Right = 3,
        }

        /// <summary>
        /// Special keys.
        /// </summary>
        public enum SpecialKey
        {
            Backspace = '\b',
            Tab = '\t',
            Clear = 0x0c,
            Return = '\r',
            Pause = 0x13,
            Escape = 0x1b,
            Space = ' ',
            Delete = 0xf7,
            Up = 273,
            Down,
            Right,
            Left,
            Insert,
            Home,
            End,
            PageUp,
            PageDown,
            F1,
            F2,
            F3,
            F4,
            F5,
            F6,
            F7,
            F8,
            F9,
            F10,
            F11,
            F12,
            F13,
            F14,
            F15,
        }

        /// <summary>
        /// Possible key modifiers.
        /// </summary>
        [Flags]
        public enum KeyModifier
        {
            None = 0x0000,
            Shift = 0x0003,
            Ctrl = 0x00c0,
            Alt = 0x0100,
            Meta = 0x0c00,
        }

        #endregion

        #region Native Functions

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern IntPtr TwGetLastError();

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwInit(GraphicsAPI graphicsAPI, IntPtr device);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwTerminate();

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwDraw();

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern void TwHandleErrors(ErrorHandler handler);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwWindowSize(int width, int height);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwMouseMotion(int mouseX, int mouseY);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwMouseWheel(int pos);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwMouseButton(MouseAction action, MouseButton button);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwKeyPressed(int key, KeyModifier modifiers);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwEventSFML(IntPtr sfmlEvent, byte major, byte minor);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwEventWin(IntPtr wnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwSetCurrentWindow(int windowID);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwGetCurrentWindow();

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwWindowExists(int windowID);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern IntPtr TwNewBar(string barName);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwDeleteBar(IntPtr bar);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwSetTopBar(IntPtr bar);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwSetBottomBar(IntPtr bar);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwRefreshBar(IntPtr bar);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwDefine(String def);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwSetParam(IntPtr bar, String varName, String paramName, ParamValueType type, uint inValueCount, IntPtr inValues);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwSetParam(IntPtr bar, String varName, String paramName, ParamValueType type, uint inValueCount, String inValues);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwGetParam(IntPtr bar, String varName, String paramName, ParamValueType type, uint outValueMaxCount, IntPtr outValues);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwAddVarRO(IntPtr bar, String name, VariableType type, IntPtr var, String def);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwAddVarRW(IntPtr bar, String name, VariableType type, IntPtr var, String def);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwAddVarCB(IntPtr bar, String name, VariableType type, SetVarCallback setCallback, GetVarCallback getCallback, IntPtr clientData, String def);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwAddSeparator(IntPtr bar, String name, String def);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwAddButton(IntPtr bar, String name, ButtonCallback callback, IntPtr clientData, String def);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern VariableType TwDefineEnumFromString(String name, String enumString);

        [DllImport(DLLName, CharSet = CharSet.Ansi)]
        private static extern int TwRemoveVar(IntPtr bar, String name);

        #endregion

        #region Callback Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void SetVarCallback(IntPtr value, IntPtr clientData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void GetVarCallback(IntPtr value, IntPtr clientData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ButtonCallback(IntPtr clientData);

        #endregion

        #region Exception-Safe Wrappers

        internal static String GetLastError()
        {
            return Marshal.PtrToStringAuto(TwGetLastError());
        }

        internal static void Init(GraphicsAPI graphicsAPI, IntPtr device)
        {
            if (TwInit(graphicsAPI, device) == 0)
                throw new AntTweakBarException("TwInit failed");
        }

        internal static void Terminate()
        {
            if (TwTerminate() == 0)
                throw new AntTweakBarException("TwTerminate failed");
        }

        internal static void Draw()
        {
            if (TwDraw() == 0)
                throw new AntTweakBarException("TwDraw failed");
        }

        internal static void WindowSize(int width, int height)
        {
            if (TwWindowSize(width, height) == 0)
                throw new AntTweakBarException("TwWindowSize failed");
        }

        internal static bool MouseMotion(int mouseX, int mouseY)
        {
            return TwMouseMotion(mouseX, mouseY) != 0;
        }

        internal static bool MouseWheel(int pos)
        {
            return TwMouseWheel(pos) != 0;
        }

        internal static bool MouseClick(MouseAction action, MouseButton button)
        {
            return TwMouseButton(action, button) != 0;
        }

        internal static bool KeyPressed(int key, KeyModifier modifiers)
        {
            return TwKeyPressed(key, modifiers) != 0;
        }

        internal static bool EventSFML(IntPtr sfmlEvent, byte majorVersion, byte minorVersion)
        {
            return TwEventSFML(sfmlEvent, majorVersion, minorVersion) != 0;
        }

        internal static bool EventWin(IntPtr wnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            return TwEventWin(wnd, msg, wParam, lParam) != 0;
        }

        internal static void SetCurrentWindow(int windowID)
        {
            if (TwSetCurrentWindow(windowID) == 0)
                throw new AntTweakBarException("TwSetCurrentWindow failed");
        }

        internal static int GetCurrentWindow()
        {
            return TwGetCurrentWindow();
        }

        internal static bool WindowExists(int windowID)
        {
            return TwWindowExists(windowID) != 0;
        }

        internal static IntPtr NewBar(string barName)
        {
            IntPtr bar = TwNewBar(barName);

            if (bar == IntPtr.Zero)
                throw new AntTweakBarException("TwNewBar failed");
            else
                return bar;
        }

        internal static void DeleteBar(IntPtr bar)
        {
            if (TwDeleteBar(bar) == 0)
                throw new AntTweakBarException("TwDeleteBar failed");
        }

        internal static void SetTopBar(IntPtr bar)
        {
            if (TwSetTopBar(bar) == 0)
                throw new AntTweakBarException("TwSetTopBar failed");
        }

        internal static void SetBottomBar(IntPtr bar)
        {
            if (TwSetBottomBar(bar) == 0)
                throw new AntTweakBarException("TwSetBottomBar failed");
        }

        internal static void RefreshBar(IntPtr bar)
        {
            if (TwRefreshBar(bar) == 0)
                throw new AntTweakBarException("TwRefreshBar failed");
        }

        internal static void Define(String def)
        {
            if (TwDefine(def) == 0)
                throw new AntTweakBarException("TwDefine failed");
        }

        internal static void SetParam(IntPtr bar, String varName, String paramName, ParamValueType type, uint inValueCount, IntPtr inValues)
        {
            if (TwSetParam(bar, varName, paramName, type, inValueCount, inValues) == 0)
                throw new AntTweakBarException("TwSetParam failed");
        }

        #region SetParam Base Specializations

        internal unsafe static void SetParam(IntPtr bar, String varName, String paramName, String value)
        {
            /* I couldn't make the IntPtr one work with strings so I just added another overload. */
            if (TwSetParam(bar, varName, paramName, ParamValueType.TW_PARAM_CSTRING, 1, value) == 0)
                throw new AntTweakBarException("TwSetParam failed");
        }

        internal unsafe static void SetParam(IntPtr bar, String varName, String paramName, params Int32[] values)
        {
            fixed (int* array = values)
            {
                SetParam(bar, varName, paramName, ParamValueType.TW_PARAM_INT32, (uint)values.Length, (IntPtr)array);
            }
        }

        internal unsafe static void SetParam(IntPtr bar, String varName, String paramName, params Single[] values)
        {
            fixed (float* array = values)
            {
                SetParam(bar, varName, paramName, ParamValueType.TW_PARAM_FLOAT, (uint)values.Length, (IntPtr)array);
            }
        }

        internal unsafe static void SetParam(IntPtr bar, String varName, String paramName, params Double[] values)
        {
            fixed (double* array = values)
            {
                SetParam(bar, varName, paramName, ParamValueType.TW_PARAM_DOUBLE, (uint)values.Length, (IntPtr)array);
            }
        }

        #endregion

        #region SetParam Derived Specializations

        internal unsafe static void SetParam(IntPtr bar, String varName, String paramName, Color color)
        {
            SetParam(bar, varName, paramName, color.R, color.G, color.B);
        }

        internal unsafe static void SetParam(IntPtr bar, String varName, String paramName, Boolean boolean)
        {
            SetParam(bar, varName, paramName, boolean ? "true" : "false");
        }

        internal unsafe static void SetParam(IntPtr bar, String varName, String paramName, Point point)
        {
            SetParam(bar, varName, paramName, new Int32[] { point.X, point.Y });
        }

        internal unsafe static void SetParam(IntPtr bar, String varName, String paramName, Size size)
        {
            SetParam(bar, varName, paramName, new Int32[] { size.Width, size.Height });
        }

        #endregion

        internal static int GetParam(IntPtr bar, String varName, String paramName, ParamValueType type, uint outValueMaxCount, IntPtr outValues)
        {
            var retval = TwGetParam(bar, varName, paramName, type, outValueMaxCount, outValues);

            if (retval == 0)
                throw new AntTweakBarException("TwSetParam failed");
            else
                return retval;
        }

        #region GetParam Base Specializations

        // Max elements (per param, on stack)
        private const int MaxElems = 4096;

        internal unsafe static String GetStringParam(IntPtr bar, String varName, String paramName)
        {
            fixed (byte* array = new byte[MaxElems])
            {
                /*int count = */GetParam(bar, varName, paramName, ParamValueType.TW_PARAM_CSTRING, MaxElems - 1, (IntPtr)array);
                /* I don't know what to do with the (nonzero) return value. The documentation is inconsistent. */
                return Marshal.PtrToStringAnsi((IntPtr)array);
            }
        }

        internal unsafe static Int32[] GetIntParam(IntPtr bar, String varName, String paramName)
        {
            fixed (int* array = new int[MaxElems])
            {
                int count = GetParam(bar, varName, paramName, ParamValueType.TW_PARAM_INT32, MaxElems - 1, (IntPtr)array);

                var retval = new Int32[count];
                Marshal.Copy((IntPtr)array, retval, 0, count);
                return retval; /* Perform a direct memory block-copy. */
            }
        }

        internal unsafe static Single[] GetSingleParam(IntPtr bar, String varName, String paramName)
        {
            fixed (float* array = new float[MaxElems])
            {
                int count = GetParam(bar, varName, paramName, ParamValueType.TW_PARAM_FLOAT, MaxElems - 1, (IntPtr)array);

                var retval = new Single[count];
                Marshal.Copy((IntPtr)array, retval, 0, count);
                return retval;
            }
        }

        internal unsafe static Double[] GetDoubleParam(IntPtr bar, String varName, String paramName)
        {
            fixed (double* array = new double[MaxElems])
            {
                int count = GetParam(bar, varName, paramName, ParamValueType.TW_PARAM_DOUBLE, MaxElems - 1, (IntPtr)array);

                var retval = new Double[count];
                Marshal.Copy((IntPtr)array, retval, 0, count);
                return retval;
            }
        }

        #endregion

        #region GetParam Derived Specializations

        internal unsafe static Color GetColorParam(IntPtr bar, String varName, String paramName)
        {
            var components = GetIntParam(bar, varName, paramName);

            if (components.Length != 3)
                throw new ArgumentException("Parameter is not a color");

            for (var t = 0; t < 3; ++t)
                if ((components[t] < 0) || (components[t] > 255))
                    throw new ArgumentException("Parameter is not a color");

            return Color.FromArgb(components[0], components[1], components[2]);
        }

        internal unsafe static Boolean GetBooleanParam(IntPtr bar, String varName, String paramName)
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
                    throw new ArgumentException("Parameter is not a boolean");
            }
        }

        internal unsafe static Point GetPointParam(IntPtr bar, String varName, String paramName)
        {
            var components = GetIntParam(bar, varName, paramName);

            if (components.Length != 2)
                throw new ArgumentException("Parameter is not a point");

            return new Point(components[0], components[1]);
        }

        internal unsafe static Size GetSizeParam(IntPtr bar, String varName, String paramName)
        {
            var components = GetIntParam(bar, varName, paramName);

            if (components.Length != 2)
                throw new ArgumentException("Parameter is not a size");

            for (var t = 0; t < 2; ++t)
                if (components[t] <= 0)
                    throw new ArgumentException("Parameter is not a size");

            return new Size(components[0], components[1]);
        }

        #endregion

        internal static void AddVarRO(IntPtr bar, String name, VariableType type, IntPtr var, String def = "")
        {
            if (TwAddVarRO(bar, name, type, var, def) == 0)
                throw new AntTweakBarException("TwAddVarRO failed");
        }

        internal static void AddVarRW(IntPtr bar, String name, VariableType type, IntPtr var, String def = "")
        {
            if (TwAddVarRW(bar, name, type, var, def) == 0)
                throw new AntTweakBarException("TwAddVarRW failed");
        }

        internal static void AddVarCB(IntPtr bar, String name, VariableType type, SetVarCallback setCallback, GetVarCallback getCallback, IntPtr clientData, String def = "")
        {
            if (TwAddVarCB(bar, name, type, setCallback, getCallback, clientData, def) == 0)
                throw new AntTweakBarException("TwAddVarCB failed");
        }

        internal static void AddSeparator(IntPtr bar, String name, String def = "")
        {
            if (TwAddSeparator(bar, name, def) == 0)
                throw new AntTweakBarException("TwAddSeparator failed");
        }

        internal static void AddButton(IntPtr bar, String name, ButtonCallback callback, IntPtr clientData, String def = "")
        {
            if (TwAddButton(bar, name, callback, clientData, def) == 0)
                throw new AntTweakBarException("TwAddButton failed");
        }

        internal static VariableType DefineEnumFromString(String name, String enumString)
        {
            var retval = TwDefineEnumFromString(name, enumString);

            if ((int)retval == 0)
                throw new AntTweakBarException("TwDefineEnumFromString failed");
            else
                return retval;
        }

        internal static void RemoveVar(IntPtr bar, String name)
        {
            if (TwRemoveVar(bar, name) == 0)
                throw new AntTweakBarException("TwRemoveVar failed");
        }

        #endregion

        #region Deferred Error Management

        /// <summary>
        /// This handler takes a single error string.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ErrorHandler(String message);

        /// <summary>
        /// Sets an error handler for the AntTweakBar library.
        /// </summary>
        internal static void HandleErrors(ErrorHandler handler)
        {
            TwHandleErrors(handler);
        }

        /// <summary>
        /// Event arguments for an AntTweakBar error.
        /// </summary>
        public class ErrorEventArgs : EventArgs
        {
            private readonly String error;

            /// <summary>
            /// Initializes new event arguments for an AntTweakBar error.
            /// </summary>
            public ErrorEventArgs(String error)
            {
                this.error = error;
            }

            /// <summary>
            /// Gets the error string.
            /// </summary>
            public String Error { get { return error; } }
        }

        /// <summary>
        /// Occurs when an AntTweakBar error occurs.
        /// </summary>
        public static event EventHandler<ErrorEventArgs> Error;

        /// <summary>
        /// Initializes the AntTweakBar.NET wrapper.
        /// </summary>
        static TW()
        {
            HandleErrors(error =>
            {
                if (Error != null)
                    Error(null, new ErrorEventArgs(error));
            });
        }

        #endregion
    }
}
