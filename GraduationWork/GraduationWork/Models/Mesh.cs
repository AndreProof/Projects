using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GraduationWork.Models
{
    /// <summary>
    /// Меш.
    /// </summary>
    public class Mesh
    {
        /// <summary>
        /// Первая точка.
        /// </summary>
        public Vector3D FstPoint { get; set; } = new();

        /// <summary>
        /// Вторая точка.
        /// </summary>
        public Vector3D SndPoint { get; set; } = new();

        /// <summary>
        /// Третья точка.
        /// </summary>
        public Vector3D TrdPoint { get; set; } = new();

        /// <summary>
        /// Вектор нормали.
        /// </summary>
        public Vector3D Normal { get; set; } = new();

        /// <summary>
        /// Список всех точек..
        /// </summary>
        [JsonIgnore] public IEnumerable<Vector3D> Points => new List<Vector3D>
        {
            this.FstPoint, this.SndPoint, this.TrdPoint
        };

        public void RotateX(double angle)
        {
            this.FstPoint.RotateX(angle);
            this.SndPoint.RotateX(angle);
            this.TrdPoint.RotateX(angle);
            this.Normal.RotateX(angle);
        }

        public void RotateY(double angle)
        {
            this.FstPoint.RotateY(angle);
            this.SndPoint.RotateY(angle);
            this.TrdPoint.RotateY(angle);
            this.Normal.RotateY(angle);
        }

        public void RotateZ(double angle)
        {
            this.FstPoint.RotateZ(angle);
            this.SndPoint.RotateZ(angle);
            this.TrdPoint.RotateZ(angle);
            this.Normal.RotateZ(angle);
        }

        public Mesh Clone()
        {
            return new Mesh
            {
                FstPoint = this.FstPoint.Clone(),
                SndPoint = this.SndPoint.Clone(),
                TrdPoint = this.TrdPoint.Clone(),
                Normal = this.Normal.Clone()
            };
        }
    }
}
