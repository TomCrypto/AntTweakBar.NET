using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace AntTweakBar
{
    /// <summary>
    /// A wrapper which tries to accommodate most RGB color types.
    /// </summary>
    public struct ColorWrapper<T> : ColorType, IEquatable<ColorWrapper<T>>
    {
        private static readonly String[] RNames = { "R", "r", "Red",   "red",   "GetRed"   };
        private static readonly String[] GNames = { "G", "g", "Green", "green", "GetGreen" };
        private static readonly String[] BNames = { "B", "b", "Blue",  "blue",  "GetBlue"  };

        private readonly T color;

        public float R { get { return Helper.Lookup(color, RNames); } }
        public float G { get { return Helper.Lookup(color, GNames); } }
        public float B { get { return Helper.Lookup(color, BNames); } }

        public ColorWrapper(float r, float g, float b)
        {
            color = Helper.Create<T>(r, g, b);
        }

        public static implicit operator ColorWrapper<T>(T color)
        {
            return new ColorWrapper<T>(Helper.Lookup(color, RNames),
                                       Helper.Lookup(color, GNames),
                                       Helper.Lookup(color, BNames));
        }

        public static implicit operator T(ColorWrapper<T> wrapper)
        {
            return Helper.Create<T>(wrapper.R, wrapper.G, wrapper.B);
        }

        public override String ToString()
        {
            return color.ToString();
        }

        #region IEquatable

        public static bool operator ==(ColorWrapper<T> a, ColorWrapper<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ColorWrapper<T> a, ColorWrapper<T> b)
        {
            return !(a == b);
        }

        public bool Equals(ColorWrapper<T> other)
        {
            return color.Equals(other.color);
        }

        public override bool Equals(object obj)
        {
            if ((obj != null) && (obj is ColorWrapper<T>))
                return Equals((ColorWrapper<T>)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return color.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// A wrapper which tries to accommodate most vector types.
    /// </summary>
    public struct VectorWrapper<T> : VectorType, IEquatable<VectorWrapper<T>>
    {
        private static readonly String[] XNames = { "X", "x", "GetX" };
        private static readonly String[] YNames = { "Y", "y", "GetY" };
        private static readonly String[] ZNames = { "Z", "z", "GetZ" };

        private readonly T vector;

        public float X { get { return Helper.Lookup(vector, XNames); } }
        public float Y { get { return Helper.Lookup(vector, YNames); } }
        public float Z { get { return Helper.Lookup(vector, ZNames); } }

        public VectorWrapper(float x, float y, float z)
        {
            vector = Helper.Create<T>(x, y, z);
        }

        public static implicit operator VectorWrapper<T>(T color)
        {
            return new VectorWrapper<T>(Helper.Lookup(color, XNames),
                                        Helper.Lookup(color, YNames),
                                        Helper.Lookup(color, ZNames));
        }

        public static implicit operator T(VectorWrapper<T> wrapper)
        {
            return Helper.Create<T>(wrapper.X, wrapper.Y, wrapper.Z);
        }

        public override String ToString()
        {
            return vector.ToString();
        }

        #region IEquatable

        public static bool operator ==(VectorWrapper<T> a, VectorWrapper<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(VectorWrapper<T> a, VectorWrapper<T> b)
        {
            return !(a == b);
        }

        public bool Equals(VectorWrapper<T> other)
        {
            return vector.Equals(other.vector);
        }

        public override bool Equals(object obj)
        {
            if ((obj != null) && (obj is VectorWrapper<T>))
                return Equals((VectorWrapper<T>)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return vector.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// A wrapper which tries to accommodate most quaternion types.
    /// </summary>
    public struct QuaternionWrapper<T> : QuaternionType, IEquatable<QuaternionWrapper<T>>
    {
        private static readonly String[] XNames = { "X", "x", "GetX" };
        private static readonly String[] YNames = { "Y", "y", "GetY" };
        private static readonly String[] ZNames = { "Z", "z", "GetZ" };
        private static readonly String[] WNames = { "W", "w", "GetW" };

        private readonly T quat;

        public float X { get { return Helper.Lookup(quat, XNames); } }
        public float Y { get { return Helper.Lookup(quat, YNames); } }
        public float Z { get { return Helper.Lookup(quat, ZNames); } }
        public float W { get { return Helper.Lookup(quat, WNames); } }

        public QuaternionWrapper(float x, float y, float z, float w)
        {
            quat = Helper.Create<T>(x, y, z, w);
        }

        public static implicit operator QuaternionWrapper<T>(T color)
        {
            return new QuaternionWrapper<T>(Helper.Lookup(color, XNames),
                                            Helper.Lookup(color, YNames),
                                            Helper.Lookup(color, ZNames),
                                            Helper.Lookup(color, WNames));
        }

        public static implicit operator T(QuaternionWrapper<T> wrapper)
        {
            return Helper.Create<T>(wrapper.X, wrapper.Y, wrapper.Z, wrapper.W);
        }

        public override String ToString()
        {
            return quat.ToString();
        }

        #region IEquatable

        public static bool operator ==(QuaternionWrapper<T> a, QuaternionWrapper<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(QuaternionWrapper<T> a, QuaternionWrapper<T> b)
        {
            return !(a == b);
        }

        public bool Equals(QuaternionWrapper<T> other)
        {
            return quat.Equals(other.quat);
        }

        public override bool Equals(object obj)
        {
            if ((obj != null) && (obj is QuaternionWrapper<T>))
                return Equals((QuaternionWrapper<T>)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return quat.GetHashCode();
        }

        #endregion
    }
}

