using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;

namespace GraduationWork.Models
{
    public class Figure
    {
        public int Id { get; set; }

        public bool IsSelected { get; set; }

        public Vector3D Rotation { get; set; } = new Vector3D(90, 90, 0);

        private Color color = System.Drawing.Color.HotPink;

        public string Color
        {
            get => "0x" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
            set
            {
                var argb = Convert.ToInt32(value);
                this.color = System.Drawing.Color.FromArgb(argb);
            }
        }

        public string Name { get; set; }

        [JsonIgnore] public double MaxX => this.Meshes.Any() ? this.Meshes.SelectMany(x => x.Points).Max(x => x.X) : double.MinValue;
        [JsonIgnore] public double MaxY => this.Meshes.Any() ? this.Meshes.SelectMany(x => x.Points).Max(x => x.Y) : double.MinValue;
        [JsonIgnore] public double MaxZ => this.Meshes.Any() ? this.Meshes.SelectMany(x => x.Points).Max(x => x.Z) : double.MinValue;
        [JsonIgnore] public double MinX => this.Meshes.Any() ? this.Meshes.SelectMany(x => x.Points).Min(x => x.X) : double.MinValue;
        [JsonIgnore] public double MinY => this.Meshes.Any() ? this.Meshes.SelectMany(x => x.Points).Min(x => x.Y) : double.MinValue;
        [JsonIgnore] public double MinZ => this.Meshes.Any() ? this.Meshes.SelectMany(x => x.Points).Min(x => x.Z) : double.MinValue;
        [JsonIgnore] public List<Vector2D> PlacedEcv { get; set; }

        /// <summary>
        /// Меши фигуры.
        /// </summary>
        public IEnumerable<Mesh> Meshes { get; set; } = new List<Mesh>();

        public void SetPosition(Vector3D point)
        {
            var minX = this.MinX;
            var minY = this.MinY;
            var minZ = this.MinZ;

            var deltaX = point.X - minX;
            var deltaY = point.Y - minY;
            var deltaZ = point.Z - minZ;

            this.ShiftMeshes(deltaX, deltaY, deltaZ);
        }

        public void SetPosition(Vector3D point, double angle, List<Vector2D> figureEcv)
        {
            var minEcv = new Vector2D(figureEcv.Min(x => x.X), figureEcv.Min(x => x.Y));
            this.PlacedEcv = figureEcv.Select(x => x.Clone()).ToList();

            this.RotateY(-angle);

            var minX = minEcv.X;
            var minY = this.MinY;
            var minZ = minEcv.Y;

            var deltaX = point.X - minX;
            var deltaY = point.Y - minY;
            var deltaZ = point.Z - minZ;

            this.ShiftLines(deltaX, deltaZ);
            this.ShiftMeshes(deltaX, deltaY, deltaZ);
        }

        public void ShiftLines(double deltaX, double deltaY)
        {
            foreach (var line in this.PlacedEcv)
            {
                line.X += deltaX;
                line.Y += deltaY;
            }
        }

        public void ShiftMeshes(double deltaX, double deltaY, double deltaZ)
        {
            foreach (var mesh in this.Meshes)
            {
                mesh.FstPoint.X += deltaX;
                mesh.FstPoint.Y += deltaY;
                mesh.FstPoint.Z += deltaZ;

                mesh.SndPoint.X += deltaX;
                mesh.SndPoint.Y += deltaY;
                mesh.SndPoint.Z += deltaZ;

                mesh.TrdPoint.X += deltaX;
                mesh.TrdPoint.Y += deltaY;
                mesh.TrdPoint.Z += deltaZ;
            }
        }

        public string Serialize()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return System.Text.Json.JsonSerializer.Serialize(this, options);
        }

        public Figure Clone(bool isPartial = false)
        {
            return new Figure()
            {
                Color = this.color.ToArgb().ToString(),
                Rotation = this.Rotation.Clone(),
                Meshes = isPartial ? new List<Mesh>() : this.Meshes.Select(x => x.Clone()).ToList(),
                Name = this.Name
            };
        }

        /// <summary>
        /// Поворот модели вокруг оси X
        /// </summary>
        /// <param name="angle">Угол поворота</param>
        public void RotateX(double angle)
        {
            foreach (var mesh in this.Meshes)
            {
                mesh.RotateX(angle);
            }
        }

        /// <summary>
        /// Поворот модели вокруг оси Y
        /// </summary>
        /// <param name="angle">Угол поворота</param>
        public void RotateY(double angle)
        {
            foreach (var mesh in this.Meshes)
            {
                mesh.RotateY(angle);
            }
        }

        /// <summary>
        /// Поворот модели вокруг оси Z
        /// </summary>
        /// <param name="angle">Угол поворота</param>
        public void RotateZ(double angle)
        {
            foreach (var mesh in this.Meshes)
            {
                mesh.RotateZ(angle);
            }
        }

        /// <summary>
        /// Получить эквидистанту.
        /// </summary>
        public List<Vector2D> GetEcv(double dist)
        {
            this.RotateX(this.Rotation.X);
            this.RotateY(this.Rotation.Y);
            this.RotateZ(this.Rotation.Z);

            var mch = new List<Vector2D>();

            var tmpPoints = this.Meshes.SelectMany(x => x.Points).ToList();
            var min = double.MaxValue; 
            var max = double.MinValue;
            int index = -1;
            for (var i = 0; i < tmpPoints.Count; i++)
            {
                if (tmpPoints[i].Z <= min)
                {
                    if (tmpPoints[i].Z == min)
                    {
                        if (tmpPoints[i].X > max)
                        {
                            index = i; min = tmpPoints[i].Z; max = tmpPoints[i].X;
                        }
                    }
                    else
                    {
                        index = i; min = tmpPoints[i].Z; max = tmpPoints[i].X;
                    }
                }
            }
            mch.Add(new Vector2D(tmpPoints[index].X, tmpPoints[index].Z));

            var fst = new Vector2D();
            var snd = new Vector2D();
            while (true)
            {
                min = double.MaxValue; 
                max = double.MinValue; 
                index = -1;

                var prelates = mch.Count == 1 ? new Vector2D(0, 0) : mch[^2];

                for (var i = 0; i < tmpPoints.Count; i++)
                {
                    fst.X = mch.Last().X - prelates.X;
                    fst.Y = mch.Last().Y - prelates.Y;
                    snd.X = tmpPoints[i].X - mch.Last().X;
                    snd.Y = tmpPoints[i].Z - mch.Last().Y;
                    var angle = this.Angle(fst, snd);
                    if (angle <= min)
                    {
                        if (angle == min)
                        {
                            if (tmpPoints[i].X > max)
                            {
                                index = i; max = tmpPoints[i].X;
                            }
                        }
                        else
                        {
                            index = i; min = angle; max = tmpPoints[i].X;
                        }
                    }
                }
                if (tmpPoints[index].X == mch.First().X && tmpPoints[index].Z == mch.First().Y)
                    break;

                mch.Add(new Vector2D(tmpPoints[index].X, tmpPoints[index].Z));
                tmpPoints.RemoveAt(index);
            }

            return mch.Select(x =>
            {
                var newX = Convert.ToSingle(x.X + dist * x.X / Math.Sqrt(x.X * x.X + x.Y * x.Y));
                var newY = Convert.ToSingle(x.Y + dist * x.Y / Math.Sqrt(x.X * x.X + x.Y * x.Y));
                return new Vector2D(newX, newY);
            }).ToList();
        }

        /// <summary>
        /// Угол между точками
        /// </summary>
        /// <param name="vec1">Координаты точки №1</param>
        /// <param name="vec2">Координаты точки №2</param>
        /// <returns></returns>
        private double Angle(Vector2D vec1, Vector2D vec2)
        {
            return Math.Acos((vec1.X * vec2.X + vec1.Y * vec2.Y) / (Math.Sqrt(vec1.X * vec1.X + vec1.Y * vec1.Y) * Math.Sqrt(vec2.X * vec2.X + vec2.Y * vec2.Y))) * 180 / Math.PI;
        }

        /// <summary>
        /// Возвращает матрицу экидистанты модели, повернутой на указанный угол
        /// </summary>
        /// <param name="angle">Угол поворота модели</param>
        /// <returns></returns>
        public int[,] MatrixOfModel(List<Vector2D> ecv, double dist, double angle)
        {
            var tmpPoints = ecv.Select(x => x.Clone()).ToList();

            if (angle % 360 > 0)
            {
                foreach (var tmpPoint in tmpPoints)
                {
                    tmpPoint.Rotate(angle);
                }
            }

            var shiftMinX = tmpPoints.Min(x => x.X);
            var shiftMinY = tmpPoints.Min(x => x.Y);

            foreach (var tmpPoint in tmpPoints)
            {
                tmpPoint.X -= shiftMinX;
                tmpPoint.Y -= shiftMinY;
            }

            var minX = tmpPoints.Select(x => (int)Math.Ceiling(x.X)).Min();
            var minY = tmpPoints.Select(x => (int)Math.Ceiling(x.Y)).Min();

            var maxX = tmpPoints.Select(x => (int)Math.Ceiling(x.X)).Max();
            var maxY = tmpPoints.Select(x => (int)Math.Ceiling(x.Y)).Max();

            var matrix = new int[maxY - minY, maxX - minX];

            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    if (this.PointInEcv(new Vector2D(i, j), tmpPoints)
                    || this.PointInEcv(new Vector2D(i + 1, j), tmpPoints)
                    || this.PointInEcv(new Vector2D(i, j + 1), tmpPoints)
                    || this.PointInEcv(new Vector2D(i + 1, j + 1), tmpPoints))
                    {
                        matrix[i, j] = 1;
                    }
                }
            }

            return matrix;
        }

        private bool PointInEcv(Vector2D point, List<Vector2D> ecv)
        {
            var result = false;
            var j = ecv.Count - 1;
            for (var i = 0; i < ecv.Count; i++)
            {
                if ((ecv[i].X < point.Y && ecv[j].X >= point.Y || ecv[j].X < point.Y && ecv[i].X >= point.Y) &&
                    (ecv[i].Y + (point.Y - ecv[i].X) / (ecv[j].X - ecv[i].X) * (ecv[j].Y - ecv[i].Y) < point.X))
                    result = !result;
                j = i;
            }
            return result;
        }
    }
}
