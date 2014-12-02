using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AntTweakBar
{
    /// <summary>
    /// Specifies the possible color selection modes.
    /// </summary>
    public enum ColorMode
    {
        /// <summary>
        /// Color selection is in RGB mode.
        /// </summary>
        RGB,
        /// <summary>
        /// Color selection is in HLS (HSL) mode.
        /// </summary>
        HLS,
    }

    /// <summary>
    /// Specifies the possible (axis-aligned) axis orientations.
    /// </summary>
    public enum AxisOrientation
    {
        /// <summary>
        /// Right axis.
        /// </summary>
        PositiveX,
        /// <summary>
        /// Left axis.
        /// </summary>
        NegativeX,
        /// <summary>
        /// Upwards axis.
        /// </summary>
        PositiveY,
        /// <summary>
        /// Downwards axis.
        /// </summary>
        NegativeY,
        /// <summary>
        /// Front axis (towards the viewer).
        /// </summary>
        PositiveZ,
        /// <summary>
        /// Back axis (into the screen).
        /// </summary>
        NegativeZ,
    }

    /// <summary>
    /// A low-level wrapper to the AntTweakBar API.
    /// </summary>
    public static partial class Tw
    {
        /// <summary>
        /// Called by AntTweakBar when the user changes a variable's value.
        /// </summary>
        public delegate void SetVarCallback(
            [In] IntPtr value,
            [In] IntPtr clientData);

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs a variable's value.
        /// </summary>
        public delegate void GetVarCallback(
            [In] IntPtr value,
            [In] IntPtr clientData);

        /// <summary>
        /// Called by AntTweakBar when the user clicks on a button.
        /// </summary>
        public delegate void ButtonCallback(
            [In] IntPtr clientData);

        /// <summary>
        /// Called by AntTweakBar to retrieve a summary of a struct variable.
        /// </summary>
        public delegate void SummaryCallback(
            [In] IntPtr summaryString,
            [In] IntPtr summaryMaxLength,
            [In] IntPtr value,
            [In] IntPtr clientData);

        /// <summary>
        /// Called by AntTweakBar when an error occurs.
        /// </summary>
        public delegate void ErrorHandler(
            [In] IntPtr message);

        /// <summary>
        /// Specifies the graphics API's AntTweakBar supports.
        /// </summary>
        public enum GraphicsAPI
        {
            /// <summary>
            /// Graphics API previously provided.
            /// </summary>
            Unspecified                          = 0,
            /// <summary>
            /// OpenGL (compatibility profile).
            /// </summary>
            OpenGL                               = 1,
            /// <summary>
            /// Direct3D 9 (requires a IDirect3DDevice9 pointer).
            /// </summary>
            D3D9                                 = 2,
            /// <summary>
            /// Direct3D 10 (requires a ID3D10Device pointer).
            /// </summary>
            D3D10                                = 3,
            /// <summary>
            /// Direct3D 11 (requires a ID3D11Device pointer).
            /// </summary>
            D3D11                                = 4,
            /// <summary>
            /// OpenGL (core profile).
            /// </summary>
            OpenGLCore                           = 5,
        }

        /// <summary>
        /// Specifies the valid parameter value types to SetParam.
        /// </summary>
        public enum ParamValueType
        {
            /// <summary>
            /// A 32-bit signed integer parameter value.
            /// </summary>
            Int32                                = 0,
            /// <summary>
            /// A single-precision floating-point parameter value.
            /// </summary>
            Float                                = 1,
            /// <summary>
            /// A double-precision floating-point parameter value.
            /// </summary>
            Double                               = 2,
            /// <summary>
            /// A null-terminated C string (i.e. a char array).
            /// </summary>
            CString                              = 3,
        }

        /// <summary>
        /// Defines the maximum static string length.
        /// </summary>
        internal const int MaxStringLength = 4096;

        /// <summary>
        /// Specifies the different possible variable type, excluding enums and structs.
        /// </summary>
        public enum VariableType
        {
            /// <summary>
            /// A variable of an undefined type.
            /// </summary>
            Undefined                            = 0,
            /// <summary>
            /// A boolean variable on 8 bits.
            /// </summary>
            Bool8                                = 2,
            /// <summary>
            /// A boolean variable on 16 bits.
            /// </summary>
            Bool16                               = 3,
            /// <summary>
            /// A boolean variable on 32 bits.
            /// </summary>
            Bool32                               = 4,
            /// <summary>
            /// A single-byte character variable.
            /// </summary>
            Char                                 = 5,
            /// <summary>
            /// A signed 8-bit integer variable.
            /// </summary>
            Int8                                 = 6,
            /// <summary>
            /// An unsigned 8-bit integer variable.
            /// </summary>
            UInt8                                = 7,
            /// <summary>
            /// A signed 16-bit integer variable.
            /// </summary>
            Int16                                = 8,
            /// <summary>
            /// An unsigned 16-bit integer variable.
            /// </summary>
            UInt16                               = 9,
            /// <summary>
            /// A signed 32-bit integer variable.
            /// </summary>
            Int32                                = 10,
            /// <summary>
            /// An unsigned 32-bit integer variable.
            /// </summary>
            UInt32                               = 11,
            /// <summary>
            /// A single-precision floating-point variable.
            /// </summary>
            Float                                = 12,
            /// <summary>
            /// A double-precision floating-point variable.
            /// </summary>
            Double                               = 13,
            /// <summary>
            /// A 32-bit RGBA color variable on 4 bytes.
            /// </summary>
            Color32                              = 14,
            /// <summary>
            /// A 96-bit RGB floating-point color variable.
            /// </summary>
            Color3F                              = 15,
            /// <summary>
            /// A 128-bit RGBA floating-point color variable.
            /// </summary>
            Color4F                              = 16,
            /// <summary>
            /// A null-terminated C dynamically allocated string variable.
            /// </summary>
            CDString                             = 17,
            /// <summary>
            /// A quaternion variable on 4 floats (x, y, z, w).
            /// </summary>
            Quat4F                               = 19,
            /// <summary>
            /// A quaternion variable on 4 doubles (x, y, z, w).
            /// </summary>
            Quat4D                               = 20,
            /// <summary>
            /// A direction (vector) variable on 3 floats (x, y, z).
            /// </summary>
            Dir3F                                = 21,
            /// <summary>
            /// A direction (vector) variable on 3 doubles (x, y, z).
            /// </summary>
            Dir3D                                = 22,
            /// <summary>
            /// A null-terminated C string variable of length <see cref="MaxStringLength"/>.
            /// </summary>
            CSString                             = 0x30000000 + MaxStringLength,
        };

        /// <summary>
        /// Specifies information about a member of an AntTweakBar struct variable.
        /// </summary>
        public struct StructMemberInfo
        {
            /// <summary>
            /// Gets the AntTweakBar type of this member.
            /// </summary>
            public VariableType Type { get { return type; } }
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly VariableType type;

            /// <summary>
            /// Gets the offset in bytes of this member.
            /// </summary>
            public int Offset { get { return offset; } }
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly int offset;

            /// <summary>
            /// Gets the definition string to use for this member.
            /// </summary>
            public String Def { get { return def; } }
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly String def;

            /// <summary>
            /// Initializes a new instance of the <see cref="AntTweakBar.Tw.StructMemberInfo"/> struct.
            /// </summary>
            /// <param name="type">The struct member's AntTweakBar type.</param>
            /// <param name="offset">The struct member's offset in bytes.</param>
            /// <param name="def">A definition string for the struct member.</param>
            public StructMemberInfo(VariableType type, int offset, String def) : this()
            {
                this.type = type;
                this.offset = offset;
                this.def = def;
            }

            public override String ToString()
            {
                return String.Format("[StructMemberInfo: Type={0}, Offset={1}, Def={2}]", Type, Offset, Def);
            }
        }

        /// <summary>
        /// Specifies the possible mouse actions recognized by AntTweakBar.
        /// </summary>
        public enum MouseAction
        {
            /// <summary>
            /// The mouse button has been released.
            /// </summary>
            Released                             = 0,
            /// <summary>
            /// The mouse button has been pressed.
            /// </summary>
            Pressed                              = 1,
        }

        /// <summary>
        /// Specifies the possible mouse buttons recognized by AntTweakBar.
        /// </summary>
        public enum MouseButton
        {
            None                                 = 0,
            /// <summary>
            /// Represents the left mouse button.
            /// </summary>
            Left                                 = 1,
            /// <summary>
            /// Represents the middle mouse button.
            /// </summary>
            Middle                               = 2,
            /// <summary>
            /// Represents the right mouse button.
            /// </summary>
            Right                                = 3,
        }

        /// <summary>
        /// Specifies the possible key modifiers recognized by AntTweakBar.
        /// </summary>
        [Flags]
        public enum KeyModifier
        {
            /// <summary>
            /// Represents no key modifier.
            /// </summary>
            None                                 = 0x0000,
            /// <summary>
            /// Represents the shift key modifier.
            /// </summary>
            Shift                                = 0x0003,
            /// <summary>
            /// Represents the ctrl key modifier.
            /// </summary>
            Ctrl                                 = 0x00c0,
            /// <summary>
            /// Represents the alt key modifier.
            /// </summary>
            Alt                                  = 0x0100,
            /// <summary>
            /// Represents the meta key modifier.
            /// </summary>
            Meta                                 = 0x0c00,
        }

        /// <summary>
        /// Specifies the possible special keys recognized by AntTweakBar.
        /// </summary>
        public enum SpecialKey
        {
            None                                 = 0,
            Backspace                            = '\b',
            Tab                                  = '\t',
            Clear                                = 0x0c,
            Return                               = '\r',
            Pause                                = 0x13,
            Escape                               = 0x1b,
            Space                                = ' ',
            Delete                               = 0x7f,
            Up                                   = 273,
            Down                                 = 274,
            Right                                = 275,
            Left                                 = 276,
            Insert                               = 277,
            Home                                 = 278,
            End                                  = 279,
            PageUp                               = 280,
            PageDown                             = 281,
            F1                                   = 282,
            F2                                   = 283,
            F3                                   = 284,
            F4                                   = 285,
            F5                                   = 286,
            F6                                   = 287,
            F7                                   = 288,
            F8                                   = 289,
            F9                                   = 290,
            F10                                  = 291,
            F11                                  = 292,
            F12                                  = 293,
            F13                                  = 294,
            F14                                  = 295,
            F15                                  = 296,
        }
    }
}
