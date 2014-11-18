using System;

namespace AntTweakBar
{
    /// <summary>
    /// An axis-aligned coordinate system.
    /// </summary>
    public struct CoordinateSystem : IEquatable<CoordinateSystem>
    {
        /// <summary>
        /// The different axis types.
        /// </summary>
        public enum Axis
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
            NegativeZ
        }

        public readonly Axis AxisX;
        public readonly Axis AxisY;
        public readonly Axis AxisZ;

        /// <summary>
        /// Creates a new CoordinateSystem object.
        /// </summary>
        /// <param name="axisX">The x-axis.</param>
        /// <param name="axisY">The y-axis.</param>
        /// <param name="axisZ">The z-axis.</param>
        public CoordinateSystem(Axis axisX, Axis axisY, Axis axisZ)
        {
            if (!((axisX == Axis.PositiveX) || (axisX == Axis.NegativeX)
               || (axisY == Axis.PositiveX) || (axisY == Axis.NegativeX)
               || (axisZ == Axis.PositiveX) || (axisZ == Axis.NegativeX)))
                throw new ArgumentException("Invalid coordinate system");

            if (!((axisX == Axis.PositiveY) || (axisX == Axis.NegativeY)
               || (axisY == Axis.PositiveY) || (axisY == Axis.NegativeY)
               || (axisZ == Axis.PositiveY) || (axisZ == Axis.NegativeY)))
                throw new ArgumentException("Invalid coordinate system");

            if (!((axisX == Axis.PositiveZ) || (axisX == Axis.NegativeZ)
               || (axisY == Axis.PositiveZ) || (axisY == Axis.NegativeZ)
               || (axisZ == Axis.PositiveZ) || (axisZ == Axis.NegativeZ)))
                throw new ArgumentException("Invalid coordinate system");

            this.AxisX = axisX;
            this.AxisY = axisY;
            this.AxisZ = axisZ;
        }

        private static String Convert(Axis axis)
        {
            switch (axis)
            {
                case Axis.PositiveX: return "x";
                case Axis.NegativeX: return "-x";
                case Axis.PositiveY: return "y";
                case Axis.NegativeY: return "-y";
                case Axis.PositiveZ: return "z";
                case Axis.NegativeZ: return "-z";
                default: throw new ArgumentException();
            }
        }

        internal String GetAxis(int t)
        {
            switch (t)
            {
                case 0: return Convert(AxisX);
                case 1: return Convert(AxisY);
                case 2: return Convert(AxisZ);
                default: throw new ArgumentException();
            }
        }

        public bool Equals(CoordinateSystem other)
        {
            return (this.AxisX == other.AxisX)
                && (this.AxisY == other.AxisY)
                && (this.AxisZ == other.AxisZ);
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is CoordinateSystem))
                return false;

            return Equals((CoordinateSystem)obj);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 67 + AxisX.GetHashCode();
            hash = hash * 67 + AxisY.GetHashCode();
            hash = hash * 67 + AxisZ.GetHashCode();
            return hash;
        }

        public override String ToString()
        {
            return String.Format("[CoordinateSystem: AxisX={0}, AxisY={1}, AxisZ={2}]", AxisX, AxisY, AxisZ);
        }
    }
}