using System;

namespace GraduationWork.Models
{
    /// <summary>
    /// Вектор трехмерных координат.
    /// </summary>
    public class Vector2D
    {
        /// <summary>
        /// Координата X.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Координата Y.
        /// </summary>
        public double Y { get; set; }

        public void Rotate(double angle)
        {
            var newX = Math.Cos(angle / (180.0 / Math.PI)) * this.X
                       - Math.Sin(angle / (180.0 / Math.PI)) * this.Y;
            var newY = Math.Sin(angle / (180.0 / Math.PI)) * this.X
                       + Math.Cos(angle / (180.0 / Math.PI)) * this.Y;

            this.X = newX;
            this.Y = newY;
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}";
        }
        
        public Vector2D Clone()
        {
            return new Vector2D
            {
                X = this.X,
                Y = this.Y
            };
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        public Vector2D()
        {
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="x">Координата Х.</param>
        /// <param name="y">Координата Y.</param>
        public Vector2D(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
