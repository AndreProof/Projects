using System;

namespace GraduationWork.Models
{
    /// <summary>
    /// Вектор трехмерных координат.
    /// </summary>
    public class Vector3D
    {
        /// <summary>
        /// Координата X.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Координата Y.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Координата Z.
        /// </summary>
        public double Z { get; set; }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Z: {Z}";
        }

        public void RotateX(double angle)
        {
            var newY = Math.Cos(angle / (180.0 / Math.PI)) * this.Y
                       - Math.Sin(angle / (180.0 / Math.PI)) * this.Z;
            var newZ = Math.Sin(angle / (180.0 / Math.PI)) * this.Y
                       + Math.Cos(angle / (180.0 / Math.PI)) * this.Z;

            this.Y = newY;
            this.Z = newZ;
        }

        public void RotateY(double angle)
        {
            var newX = Math.Cos(angle / (180.0 / Math.PI)) * this.X
                       + Math.Sin(angle / (180.0 / Math.PI)) * this.Z;
            var newZ = -Math.Sin(angle / (180.0 / Math.PI)) * this.X
                       + Math.Cos(angle / (180.0 / Math.PI)) * this.Z;

            this.X = newX;
            this.Z = newZ;
        }

        public void RotateZ(double angle)
        {
            var newX = Math.Cos(angle / (180.0 / Math.PI)) * this.X 
                       - Math.Sin(angle / (180.0 / Math.PI)) * this.Y;
            var newY = Math.Sin(angle / (180.0 / Math.PI)) * this.X 
                       + Math.Cos(angle / (180.0 / Math.PI)) * this.Y;

            this.X = newX;
            this.Y = newY;
        }

        public Vector3D Clone()
        {
            return new Vector3D
            {
                X = this.X,
                Y = this.Y,
                Z = this.Z
            };
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        public Vector3D()
        {
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="x">Координата Х.</param>
        /// <param name="y">Координата Y.</param>
        /// <param name="z">Координата Z.</param>
        public Vector3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
