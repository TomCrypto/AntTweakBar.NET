using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace AntTweakBar
{
    /// <summary>
    /// The native AntTweakBar calls.
    /// </summary>
    internal static class NativeMethods
    {
        /* For libAntTweakBar.so, AntTweakBar.dll. */
        private const String DLLName = "AntTweakBar";

        [DllImport(DLLName, EntryPoint = "TwGetLastError", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TwGetLastError();

        [DllImport(DLLName, EntryPoint = "TwInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwInit(
            [In] Tw.GraphicsAPI graphicsAPI,
            [In] IntPtr device);

        [DllImport(DLLName, EntryPoint = "TwTerminate", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwTerminate();

        [DllImport(DLLName, EntryPoint = "TwDraw", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwDraw();

        [DllImport(DLLName, EntryPoint = "TwHandleErrors", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TwHandleErrors(
            [In] Tw.ErrorHandler handler);

        [DllImport(DLLName, EntryPoint = "TwWindowSize", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwWindowSize(
            [In] Int32 width,
            [In] Int32 height);

        [DllImport(DLLName, EntryPoint = "TwMouseMotion", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwMouseMotion(
            [In] Int32 mouseX,
            [In] Int32 mouseY);

        [DllImport(DLLName, EntryPoint = "TwMouseWheel", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwMouseWheel(
            [In] Int32 pos);

        [DllImport(DLLName, EntryPoint = "TwMouseButton", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwMouseButton(
            [In] Tw.MouseAction action,
            [In] Tw.MouseButton button);

        [DllImport(DLLName, EntryPoint = "TwKeyPressed", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwKeyPressed(
            [In] Int32 key,
            [In] Tw.KeyModifier modifiers);

        [DllImport(DLLName, EntryPoint = "TwEventSFML", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwEventSFML(
            [In] IntPtr sfmlEvent,
            [In] Byte major,
            [In] Byte minor);

        [DllImport(DLLName, EntryPoint = "TwEventSDL", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwEventSDL(
            [In] IntPtr sdlEvent,
            [In] Byte major,
            [In] Byte minor);

        [DllImport(DLLName, EntryPoint = "TwEventWin", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwEventWin(
            [In] IntPtr wnd,
            [In] Int32 msg,
            [In] IntPtr wParam,
            [In] IntPtr lParam);

        [DllImport(DLLName, EntryPoint = "TwSetCurrentWindow", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwSetCurrentWindow(
            [In] Int32 windowID);

        [DllImport(DLLName, EntryPoint = "TwGetCurrentWindow", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 TwGetCurrentWindow();

        [DllImport(DLLName, EntryPoint = "TwWindowExists", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwWindowExists(
            [In] Int32 windowID);

        [DllImport(DLLName, EntryPoint = "TwNewBar", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern IntPtr TwNewBar(
            [In, MarshalAs(UnmanagedType.LPStr)] String barName);

        [DllImport(DLLName, EntryPoint = "TwDeleteBar", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwDeleteBar(
            [In] IntPtr bar);

        [DllImport(DLLName, EntryPoint = "TwSetTopBar", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwSetTopBar(
            [In] IntPtr bar);

        [DllImport(DLLName, EntryPoint = "TwSetBottomBar", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwSetBottomBar(
            [In] IntPtr bar);

        [DllImport(DLLName, EntryPoint = "TwRefreshBar", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean TwRefreshBar(
            [In] IntPtr bar);

        [DllImport(DLLName, EntryPoint = "TwDefine", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwDefine(
            [In, MarshalAs(UnmanagedType.LPStr)] String def);

        [DllImport(DLLName, EntryPoint = "TwSetParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwSetParamStr(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] Tw.ParamValueType type,
            [In] uint reserved,
            [In, MarshalAs(UnmanagedType.LPStr)] String param);

        [DllImport(DLLName, EntryPoint = "TwSetParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwSetParamInt(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] Tw.ParamValueType type,
            [In] uint elemCount,
            [In] Int32[] param);

        [DllImport(DLLName, EntryPoint = "TwSetParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwSetParamSingle(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] Tw.ParamValueType type,
            [In] uint elemCount,
            [In] Single[] param);

        [DllImport(DLLName, EntryPoint = "TwSetParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwSetParamDouble(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] Tw.ParamValueType type,
            [In] uint elemCount,
            [In] Double[] param);

        [DllImport(DLLName, EntryPoint = "TwGetParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Int32 TwGetParamStr(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] Tw.ParamValueType type,
            [In] uint maxStrLength,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder param);

        [DllImport(DLLName, EntryPoint = "TwGetParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Int32 TwGetParamInt(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] Tw.ParamValueType type,
            [In] uint maxElemCount,
            [Out] Int32[] param);

        [DllImport(DLLName, EntryPoint = "TwGetParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Int32 TwGetParamSingle(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] Tw.ParamValueType type,
            [In] uint maxElemCount,
            [Out] Single[] param);

        [DllImport(DLLName, EntryPoint = "TwGetParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Int32 TwGetParamDouble(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String varName,
            [In, MarshalAs(UnmanagedType.LPStr)] String paramName,
            [In] Tw.ParamValueType type,
            [In] uint maxElemCount,
            [Out] Double[] param);

        [DllImport(DLLName, EntryPoint = "TwAddVarRO", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwAddVarRO(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String name,
            [In] Tw.VariableType type,
            [In] IntPtr var,
            [In, MarshalAs(UnmanagedType.LPStr)] String def);

        [DllImport(DLLName, EntryPoint = "TwAddVarRW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwAddVarRW(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String name,
            [In] Tw.VariableType type,
            [In] IntPtr var,
            [In, MarshalAs(UnmanagedType.LPStr)] String def);

        [DllImport(DLLName, EntryPoint = "TwAddVarCB", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwAddVarCB(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String name,
            [In] Tw.VariableType type,
            [In] Tw.SetVarCallback setCallback,
            [In] Tw.GetVarCallback getCallback,
            [In] IntPtr clientData,
            [In, MarshalAs(UnmanagedType.LPStr)] String def);

        [DllImport(DLLName, EntryPoint = "TwAddSeparator", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwAddSeparator(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String name,
            [In, MarshalAs(UnmanagedType.LPStr)] String def);

        [DllImport(DLLName, EntryPoint = "TwAddButton", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwAddButton(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String name,
            [In] Tw.ButtonCallback callback,
            [In] IntPtr clientData,
            [In, MarshalAs(UnmanagedType.LPStr)] String def);

        [DllImport(DLLName, EntryPoint = "TwDefineEnumFromString", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Tw.VariableType TwDefineEnumFromString(
            [In, MarshalAs(UnmanagedType.LPStr)] String name,
            [In, MarshalAs(UnmanagedType.LPStr)] String enumString);

        [DllImport(DLLName, EntryPoint = "TwRemoveVar", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern Boolean TwRemoveVar(
            [In] IntPtr bar,
            [In, MarshalAs(UnmanagedType.LPStr)] String name);
    }

    /// <summary>
    /// A low-level wrapper to the AntTweakBar API.
    /// </summary>
    public static partial class Tw
    {
        /// <summary>
        /// Returns the last error that has occured during a previous AntTweakBar function call.
        /// </summary>
        /// <returns>A constant string that describes the error.</returns>
        public static String GetLastError()
        {
            var ptr = NativeMethods.TwGetLastError();

            var strBytes = new List<byte>();
            var off = 0;
            while (true)
            {
                var ch = Marshal.ReadByte(ptr, off++);
                if (ch == 0) break;
                strBytes.Add(ch);
            }

            return Encoding.UTF8.GetString(strBytes.ToArray());
        }

        /// <summary>
        /// This function initializes the AntTweakBar library. It must be called once at the beginning of the program, just after graphic mode is initialized.
        /// </summary>
        /// <param name="graphicsAPI">This parameter specifies which graphic API is used: OpenGL, OpenGL core profile (3.2 and higher), Direct3D 9, Direct3D 10 or Direct3D 11.</param>
        /// <param name="device">Pointer to the Direct3D device, or IntPtr.Zero for OpenGL.</param>
        public static void Init(GraphicsAPI graphicsAPI, IntPtr device)
        {
            if (graphicsAPI == GraphicsAPI.Unspecified) {
                throw new ArgumentOutOfRangeException("graphicsAPI");
            }

             if (!NativeMethods.TwInit(graphicsAPI, device)) {
                 throw new AntTweakBarException("TwInit failed.");
             }
        }

        /// <summary>
        /// Uninitialize the AntTweakBar API. Must be called at the end of the program, before terminating the graphics API.
        /// </summary>
        public static void Terminate()
        {
            if (!NativeMethods.TwTerminate()) {
                throw new AntTweakBarException("TwTerminate failed.");
            }
        }

        /// <summary>
        /// Draws all the created tweak bars. This function must be called once per frame, after all the other drawing calls and just before the application presents (swaps) the frame buffer.
        /// </summary>
        public static void Draw()
        {
            if (!NativeMethods.TwDraw()) {
                throw new AntTweakBarException("TwDraw failed.");
            }
        }

        /// <summary>
        /// Call this function to inform AntTweakBar of the size of the application graphics window, or to restore AntTweakBar graphics resources (after a fullscreen switch for instance).
        /// </summary>
        /// <param name="width">Width of the graphics window.</param>
        /// <param name="height">Height of the graphics window.</param>
        public static void WindowSize(int width, int height)
        {
            if (width <= 0) {
                throw new ArgumentOutOfRangeException("width");
            } else if (height <= 0) {
                throw new ArgumentOutOfRangeException("height");
            }

            if (!NativeMethods.TwWindowSize(width, height)) {
                throw new AntTweakBarException("TwWindowSize failed.");
            }
        }

        /// <summary>
        /// Call this function to inform AntTweakBar that the mouse has moved.
        /// </summary>
        /// <param name="mouseX">The new X position of the mouse, relative to the left border of the graphics window.</param>
        /// <param name="mouseY">The new Y position of the mouse, relative to the top border of the graphics window.</param>
        /// <returns>Whether the mouse event has been handled by AntTweakBar.</returns>
        public static bool MouseMotion(int mouseX, int mouseY)
        {
            return NativeMethods.TwMouseMotion(mouseX, mouseY);
        }

        /// <summary>
        /// Call this function to inform AntTweakBar that the mouse wheel has been used.
        /// </summary>
        /// <param name="pos">The new position of the wheel.</param>
        /// <returns>Whether the mouse wheel event has been handled by AntTweakBar.</returns>
        public static bool MouseWheel(int pos)
        {
            return NativeMethods.TwMouseWheel(pos);
        }

        /// <summary>
        /// Call this function to inform AntTweakBar that a mouse button is pressed.
        /// </summary>
        /// <param name="action">Tells if the button is pressed or released. It is one of the <see cref="AntTweakBar.Tw.MouseAction"/> constants.</param>
        /// <param name="button">Tells which button is pressed. It is one of the <see cref="AntTweakBar.Tw.MouseButton"/> constants.</param>
        /// <returns>Whether the mouse event has been handled by AntTweakBar.</returns>
        public static bool MouseClick(MouseAction action, MouseButton button)
        {
            return NativeMethods.TwMouseButton(action, button);
        }

        /// <summary>
        /// Call this function to inform AntTweakBar when a keyboard event occurs.
        /// </summary>
        /// <param name="key">The ASCII code of the pressed key, or one of the <see cref="AntTweakBar.Tw.SpecialKey"/> codes.</param>
        /// <param name="modifiers">One or a combination of the <see cref="AntTweakBar.Tw.KeyModifier"/> constants.</param>
        /// <returns>Whether the key event has been handled by AntTweakBar.</returns>
        public static bool KeyPressed(int key, KeyModifier modifiers)
        {
            return NativeMethods.TwKeyPressed(key, modifiers);
        }

        /// <summary>
        /// The SFML event handler.
        /// </summary>
        /// <returns>Whether the event has been handled by AntTweakBar.</returns>
        public static bool EventSFML(IntPtr sfmlEvent, byte majorVersion, byte minorVersion)
        {
            if (sfmlEvent == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("sfmlEvent");
            }

            return NativeMethods.TwEventSFML(sfmlEvent, majorVersion, minorVersion);
        }

        /// <summary>
        /// The SDL event handler.
        /// </summary>
        /// <returns>Whether the event has been handled by AntTweakBar.</returns>
        public static bool EventSDL(IntPtr sdlEvent, byte majorVersion, byte minorVersion)
        {
            if (sdlEvent == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("sdlEvent");
            }

            return NativeMethods.TwEventSDL(sdlEvent, majorVersion, minorVersion);
        }

        /// <summary>
        /// The Windows event handler.
        /// </summary>
        /// <returns>Whether the event has been handled by AntTweakBar.</returns>
        public static bool EventWin(IntPtr wnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (wnd == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("wnd");
            }

            return NativeMethods.TwEventWin(wnd, msg, wParam, lParam);
        }

        /// <summary>
        /// This function is intended to be used by applications with multiple graphical windows. It tells AntTweakBar to switch its current context to the context associated to the identifier windowID.
        /// </summary>
        /// <param name="windowID">Window context identifier. This identifier could be any integer.</param>
        public static void SetCurrentWindow(int windowID)
        {
            if (!NativeMethods.TwSetCurrentWindow(windowID)) {
                throw new AntTweakBarException("TwSetCurrentWindow failed.");
            }
        }

        /// <summary>
        /// Returns the current window context identifier previously set by <see cref="AntTweakBar.Tw.SetCurrentWindow"/>.
        /// </summary>
        /// <returns>The current window context identifier.</returns>
        public static int GetCurrentWindow()
        {
            return NativeMethods.TwGetCurrentWindow();
        }

        /// <summary>
        /// Check if a window context associated to the identifier windowID exists. A window context exists if it has previously been created by <see cref="AntTweakBar.Tw.SetCurrentWindow"/>.
        /// </summary>
        /// <param name="windowID">Window context identifier.</param>
        /// <returns>Whether the window context exists.</returns>
        public static bool WindowExists(int windowID)
        {
            return NativeMethods.TwWindowExists(windowID);
        }

        /// <summary>
        /// Creates a new tweak bar.
        /// </summary>
        /// <param name="barName">Name of the new tweak bar.</param>
        /// <returns>Tweak bar identifier. It is a pointer to an internal TwBar structure.</returns>
        public static IntPtr NewBar(string barName)
        {
            IntPtr bar;

            if ((bar = NativeMethods.TwNewBar(barName)) == IntPtr.Zero) {
                throw new AntTweakBarException("TwNewBar failed.");
            }
            
            return bar;
        }

        /// <summary>
        /// This function deletes a tweak bar previously created by <see cref="AntTweakBar.Tw.NewBar"/>.
        /// </summary>
        /// <param name="bar">Identifier to the tweak bar to delete.</param>
        public static void DeleteBar(IntPtr bar)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            }

            if (!NativeMethods.TwDeleteBar(bar)) {
                throw new AntTweakBarException("TwDeleteBar failed.");
            }
        }
        
        /// <summary>
        /// Set the specified bar as the foreground bar. It will be displayed on top of the other bars.
        /// </summary>
        /// <param name="bar">Bar identifier.</param>
        public static void SetTopBar(IntPtr bar)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            }

            if (!NativeMethods.TwSetTopBar(bar)) {
                throw new AntTweakBarException("TwSetTopBar failed.");
            }
        }

        /// <summary>
        /// Set the specified bar as the background bar. It will be displayed behind the other bars.
        /// </summary>
        /// <param name="bar">Bar identifier.</param>
        public static void SetBottomBar(IntPtr bar)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            }

            if (!NativeMethods.TwSetBottomBar(bar)) {
                throw new AntTweakBarException("TwSetBottomBar failed.");
            }
        }

        /// <summary>
        /// Forces bar content to be updated. By default bar content is periodically refreshed when <see cref="AntTweakBar.Tw.Draw"/> is called (the update frequency is defined by the bar parameter refresh).
        /// </summary>
        /// <param name="bar">Bar identifier.</param>
        public static void RefreshBar(IntPtr bar)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            }

            if (!NativeMethods.TwRefreshBar(bar)) {
                throw new AntTweakBarException("TwRefreshBar failed.");
            }
        }

        /// <summary>
        /// This function defines optional parameters for tweak bars and variables. For instance, it allows you to change the color of a tweak bar, to set a min and a max value for a variable, to add an help message that inform users of the meaning of a variable, and so on...
        /// </summary>
        /// <param name="def">A string containing one or more parameter assignments (separated by newlines).</param>
        public static void Define(String def)
        {
            if (def == null) {
                throw new ArgumentNullException("def");
            }

            if (!NativeMethods.TwDefine(def)) {
                throw new AntTweakBarException("TwDefine failed.");
            }
        }

        /// <summary>
        /// This function modifies the value(s) of a bar or variable parameter.
        /// </summary>
        public static void SetParam(IntPtr bar, String varName, String paramName, String value)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            } else if (paramName == null) {
                throw new ArgumentNullException("paramName");
            } else if (value == null) {
                throw new ArgumentNullException("value");
            }

            if (!NativeMethods.TwSetParamStr(bar, varName, paramName, ParamValueType.CString, 1, value)) {
                throw new AntTweakBarException("TwSetParam failed.");
            }
        }

        /// <summary>
        /// This function modifies the value(s) of a bar or variable parameter.
        /// </summary>
        public static void SetParam(IntPtr bar, String varName, String paramName, params Int32[] values)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            } else if (paramName == null) {
                throw new ArgumentNullException("paramName");
            } else if (values == null) {
                throw new ArgumentNullException("value");
            } else if (values.Length == 0) {
                throw new ArgumentOutOfRangeException("values.Length");
            }

            if (!NativeMethods.TwSetParamInt(bar, varName, paramName, ParamValueType.Int32, (uint)values.Length, values)) {
                throw new AntTweakBarException("TwSetParam failed.");
            }
        }

        /// <summary>
        /// This function modifies the value(s) of a bar or variable parameter.
        /// </summary>
        public static void SetParam(IntPtr bar, String varName, String paramName, params Single[] values)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            } else if (paramName == null) {
                throw new ArgumentNullException("paramName");
            } else if (values == null) {
                throw new ArgumentNullException("value");
            } else if (values.Length == 0) {
                throw new ArgumentOutOfRangeException("values.Length");
            }

            if (!NativeMethods.TwSetParamSingle(bar, varName, paramName, ParamValueType.Float, (uint)values.Length, values)) {
                throw new AntTweakBarException("TwSetParam failed.");
            }
        }

        /// <summary>
        /// This function modifies the value(s) of a bar or variable parameter.
        /// </summary>
        public static void SetParam(IntPtr bar, String varName, String paramName, params Double[] values)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            } else if (paramName == null) {
                throw new ArgumentNullException("paramName");
            } else if (values == null) {
                throw new ArgumentNullException("value");
            } else if (values.Length == 0) {
                throw new ArgumentOutOfRangeException("values.Length");
            }

            if (!NativeMethods.TwSetParamDouble(bar, varName, paramName, ParamValueType.Double, (uint)values.Length, values)) {
                throw new AntTweakBarException("TwSetParam failed.");
            }
        }
        
        /// <summary>
        /// This function modifies the value(s) of a bar or variable parameter.
        /// </summary>
        public static void SetParam(IntPtr bar, String varName, String paramName, Color color)
        {
            SetParam(bar, varName, paramName, color.R, color.G, color.B);
        }

        /// <summary>
        /// This function modifies the value(s) of a bar or variable parameter.
        /// </summary>
        public static void SetParam(IntPtr bar, String varName, String paramName, Boolean boolean)
        {
            SetParam(bar, varName, paramName, boolean ? "true" : "false");
        }

        /// <summary>
        /// This function modifies the value(s) of a bar or variable parameter.
        /// </summary>
        public static void SetParam(IntPtr bar, String varName, String paramName, Point point)
        {
            SetParam(bar, varName, paramName, point.X, point.Y);
        }

        /// <summary>
        /// This function modifies the value(s) of a bar or variable parameter.
        /// </summary>
        public static void SetParam(IntPtr bar, String varName, String paramName, Size size)
        {
            SetParam(bar, varName, paramName, size.Width, size.Height);
        }

        /// <summary>
        /// This function returns the current value of a bar or variable parameter.
        /// </summary>
        public static String GetStringParam(IntPtr bar, String varName, String paramName)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            } else if (paramName == null) {
                throw new ArgumentNullException("paramName");
            }

            var buffer = new StringBuilder(MaxStringLength);

            if (NativeMethods.TwGetParamStr(bar, varName, paramName, ParamValueType.CString, (uint)buffer.Capacity, buffer) == 0) {
                throw new AntTweakBarException("TwGetParam failed.");
            }

            return buffer.ToString();
        }

        /// <summary>
        /// This function returns the current value of a bar or variable parameter.
        /// </summary>
        public static Int32[] GetIntParam(IntPtr bar, String varName, String paramName, int paramCount = 0)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            } else if (paramName == null) {
                throw new ArgumentNullException("paramName");
            }

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

        /// <summary>
        /// This function returns the current value of a bar or variable parameter.
        /// </summary>
        public static Single[] GetSingleParam(IntPtr bar, String varName, String paramName, int paramCount = 0)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            } else if (paramName == null) {
                throw new ArgumentNullException("paramName");
            }

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

        /// <summary>
        /// This function returns the current value of a bar or variable parameter.
        /// </summary>
        public static Double[] GetDoubleParam(IntPtr bar, String varName, String paramName, int paramCount = 0)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            } else if (paramName == null) {
                throw new ArgumentNullException("paramName");
            }

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

        /// <summary>
        /// This function returns the current value of a bar or variable parameter.
        /// </summary>
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

        /// <summary>
        /// This function returns the current value of a bar or variable parameter.
        /// </summary>
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

        /// <summary>
        /// This function returns the current value of a bar or variable parameter.
        /// </summary>
        public static Point GetPointParam(IntPtr bar, String varName, String paramName)
        {
            var components = GetIntParam(bar, varName, paramName);

            if (components.Length != 2) {
                throw new ArgumentException("Parameter is not a point.");
            }

            return new Point(components[0], components[1]);
        }

        /// <summary>
        /// This function returns the current value of a bar or variable parameter.
        /// </summary>
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

        /// <summary>
        /// This function adds a new variable to a tweak bar by specifying the variable’s pointer. The variable is declared Read-Only (RO), so it could not be modified interactively by the user.
        /// </summary>
        /// <param name="bar">The tweak bar to which adding a new variable.</param>
        /// <param name="name">The name of the variable. It will be displayed in the tweak bar if no label is specified for this variable. It will also be used to refer to this variable in other functions.</param>
        /// <param name="type">Type of the variable. It must be one of the <see cref="AntTweakBar.Tw.VariableType"/> constants or a user defined type.</param>
        /// <param name="var">Pointer to the variable linked to this entry.</param>
        /// <param name="def">An optional definition string used to modify the behavior of this new entry.</param>
        public static void AddVarRO(IntPtr bar, String name, VariableType type, IntPtr var, String def)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            } else if (type == VariableType.Undefined) {
                throw new ArgumentOutOfRangeException("type");
            } else if (var == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("var");
            } else if (name == null) {
                throw new ArgumentNullException("name");
            }

            if (!NativeMethods.TwAddVarRO(bar, name, type, var, def)) {
                throw new AntTweakBarException("TwAddVarRO failed.");
            }
        }

        /// <summary>
        /// This function adds a new variable to a tweak bar by specifying the variable’s pointer. The variable is declared Read-Write (RW), so it could be modified interactively by the user.
        /// </summary>
        /// <param name="bar">The tweak bar to which adding a new variable.</param>
        /// <param name="name">The name of the variable. It will be displayed in the tweak bar if no label is specified for this variable. It will also be used to refer to this variable in other functions.</param>
        /// <param name="type">Type of the variable. It must be one of the <see cref="AntTweakBar.Tw.VariableType"/> constants or a user defined type.</param>
        /// <param name="var">Pointer to the variable linked to this entry.</param>
        /// <param name="def">An optional definition string used to modify the behavior of this new entry.</param>
        public static void AddVarRW(IntPtr bar, String name, VariableType type, IntPtr var, String def)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            } else if (type == VariableType.Undefined) {
                throw new ArgumentOutOfRangeException("type");
            } else if (var == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("var");
            } else if (name == null) {
                throw new ArgumentNullException("name");
            }

            if (!NativeMethods.TwAddVarRW(bar, name, type, var, def)) {
                throw new AntTweakBarException("TwAddVarRW failed.");
            }
        }

        /// <summary>
        /// This function adds a new variable to a tweak bar by providing CallBack (CB) functions to access it.
        /// </summary>
        /// <param name="bar">The tweak bar to which adding a new variable.</param>
        /// <param name="name">The name of the variable. It will be displayed in the tweak bar if no label is specified for this variable. It will also be used to refer to this variable in other functions.</param>
        /// <param name="type">Type of the variable. It must be one of the <see cref="AntTweakBar.Tw.VariableType"/> constants or a user defined type.</param>
        /// <param name="setCallback">The callback function that will be called by AntTweakBar to change the variable’s value.</param>
        /// <param name="getCallback">The callback function that will be called by AntTweakBar to get the variable’s value.</param>
        /// <param name="clientData">For your convenience, this is a supplementary pointer that will be passed to the callback functions when they are called.</param>
        /// <param name="def">An optional definition string used to modify the behavior of this new entry.</param>
        public static void AddVarCB(IntPtr bar, String name, VariableType type, SetVarCallback setCallback, GetVarCallback getCallback, IntPtr clientData, String def)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            } else if (type == VariableType.Undefined) {
                throw new ArgumentOutOfRangeException("type");
            } else if (name == null) {
                throw new ArgumentNullException("name");
            } else if (getCallback == null) {
                throw new ArgumentNullException("getCallback");
            }

            if (!NativeMethods.TwAddVarCB(bar, name, type, setCallback, getCallback, clientData, def)) {
                throw new AntTweakBarException("TwAddVarCB failed.");
            }
        }

        /// <summary>
        /// This function adds a horizontal separator line to a tweak bar. It may be useful if one wants to separate several sets of variables inside a same group.
        /// </summary>
        /// <param name="bar">The tweak bar to which adding the separator.</param>
        /// <param name="name">The name of the separator. It is optional, this parameter can be set to NULL.</param>
        /// <param name="def">An optional definition string used to modify the behavior of this new entry.</param>
        public static void AddSeparator(IntPtr bar, String name, String def)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            } else if (name == null) {
                throw new ArgumentNullException("name");
            }

            if (!NativeMethods.TwAddSeparator(bar, name, def)) {
                throw new AntTweakBarException("TwAddSeparator failed.");
            }
        }

        /// <summary>
        /// This function adds a button entry to a tweak bar. When the button is clicked by a user, the callback function provided to this function is called.
        /// </summary>
        /// <param name="bar">The tweak bar to which adding a new variable.</param>
        /// <param name="name">The name of the button. It will be displayed in the tweak bar if no label is specified for this button. It will also be used to refer to this button in other functions.</param>
        /// <param name="callback">The callback function that will be called by AntTweakBar when the button is clicked.</param>
        /// <param name="clientData">For your convenience, this is a supplementary pointer that will be passed to the callback function when it is called.</param>
        /// <param name="def">An optional definition string used to modify the behavior of this new entry.</param>
        public static void AddButton(IntPtr bar, String name, ButtonCallback callback, IntPtr clientData, String def)
        {
            if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            } else if (name == null) {
                throw new ArgumentNullException("name");
            } else if (callback == null) {
                throw new ArgumentNullException("callback");
            }

            if (!NativeMethods.TwAddButton(bar, name, callback, clientData, def)) {
                throw new AntTweakBarException("TwAddButton failed.");
            }
        }

        /// <summary>
        /// This function creates a new variable type corresponding to an enum.
        /// </summary>
        /// <param name="name">Specify a name for the enum type (must be unique).</param>
        /// <param name="enumString">Comma-separated list of labels.</param>
        /// <returns></returns>
        public static VariableType DefineEnumFromString(String name, String enumString)
        {
            if (name == null) {
                throw new ArgumentNullException("name");
            } else if (enumString == null) {
                throw new ArgumentNullException("enumString");
            }

            VariableType enumType;

            if ((enumType = NativeMethods.TwDefineEnumFromString(name, enumString)) == VariableType.Undefined) {
                throw new AntTweakBarException("TwDefineEnumFromString failed.");
            }
             
            return enumType;
        }

        /// <summary>
        /// This function removes a variable, button or separator from a tweak bar.
        /// </summary>
        /// <param name="bar">The tweak bar from which to remove a variable.</param>
        /// <param name="name">The name of the variable.</param>
        public static void RemoveVar(IntPtr bar, String name)
        {
            if (name == null) {
                throw new ArgumentNullException("name");
            } else if (bar == IntPtr.Zero) {
                throw new ArgumentOutOfRangeException("bar");
            }

            if (!NativeMethods.TwRemoveVar(bar, name)) {
                throw new AntTweakBarException("TwRemoveVar failed.");
            }
        }

        /* These are used to keep strong references to the various unmanaged callbacks
         * used by AntTweakBar. It's probably possible to do this more elegantly but I
         * don't know how, and frankly, a garbage collected callback is so problematic
         * that it's easier to just make absolutely sure they are never collected.
         * 
         * (entries are removed as variables get disposed of, so it's not too bad)
        */

        internal static IDictionary<String, Tw.SetVarCallback> SetCallbacks = new ConcurrentDictionary<String, Tw.SetVarCallback>();
        internal static IDictionary<String, Tw.GetVarCallback> GetCallbacks = new ConcurrentDictionary<String, Tw.GetVarCallback>();
        internal static IDictionary<String, Tw.ButtonCallback> BtnCallbacks = new ConcurrentDictionary<String, Tw.ButtonCallback>();
    }
}
