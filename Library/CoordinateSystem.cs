using System;

namespace AntTweakBar
{
    /// <summary>
    /// An axis-aligned coordinate system.
    /// </summary>
    public struct CoordinateSystem : IEquatable<CoordinateSystem>
    {
        public AxisOrientation AxisX { get; private set; }
        public AxisOrientation AxisY { get; private set; }
        public AxisOrientation AxisZ { get; private set; }

        /// <summary>
        /// Creates a new CoordinateSystem object.
        /// </summary>
        /// <param name="axisX">The x-axis orientation.</param>
        /// <param name="axisY">The y-axis orientation.</param>
        /// <param name="axisZ">The z-axis orientation.</param>
        public CoordinateSystem(AxisOrientation axisX, AxisOrientation axisY, AxisOrientation axisZ) : this()
        {
            if (!((axisX == AxisOrientation.PositiveX) || (axisX == AxisOrientation.NegativeX)
               || (axisY == AxisOrientation.PositiveX) || (axisY == AxisOrientation.NegativeX)
               || (axisZ == AxisOrientation.PositiveX) || (axisZ == AxisOrientation.NegativeX))) {
                throw new ArgumentException("Invalid coordinate system.");
            }

            if (!((axisX == AxisOrientation.PositiveY) || (axisX == AxisOrientation.NegativeY)
               || (axisY == AxisOrientation.PositiveY) || (axisY == AxisOrientation.NegativeY)
               || (axisZ == AxisOrientation.PositiveY) || (axisZ == AxisOrientation.NegativeY))) {
                throw new ArgumentException("Invalid coordinate system.");
            }

            if (!((axisX == AxisOrientation.PositiveZ) || (axisX == AxisOrientation.NegativeZ)
               || (axisY == AxisOrientation.PositiveZ) || (axisY == AxisOrientation.NegativeZ)
               || (axisZ == AxisOrientation.PositiveZ) || (axisZ == AxisOrientation.NegativeZ))) {
                throw new ArgumentException("Invalid coordinate system.");
            }

            AxisX = axisX;
            AxisY = axisY;
            AxisZ = axisZ;
        }

        private static String Convert(AxisOrientation axis)
        {
            switch (axis)
            {
                case AxisOrientation.PositiveX: return "x";
                case AxisOrientation.NegativeX: return "-x";
                case AxisOrientation.PositiveY: return "y";
                case AxisOrientation.NegativeY: return "-y";
                case AxisOrientation.PositiveZ: return "z";
                case AxisOrientation.NegativeZ: return "-z";
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

        #region Miscellaneous

        public bool Equals(CoordinateSystem other)
        {
            return (this.AxisX == other.AxisX)
                && (this.AxisY == other.AxisY)
                && (this.AxisZ == other.AxisZ);
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is CoordinateSystem)) {
                return false;
            }

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

        public static bool operator ==(CoordinateSystem a, CoordinateSystem b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(CoordinateSystem a, CoordinateSystem b)
        {
            return !(a == b);
        }

        #endregion

        public override String ToString()
        {
            return String.Format("[CoordinateSystem: AxisX={0}, AxisY={1}, AxisZ={2}]", AxisX, AxisY, AxisZ);
        }
    }
}
