using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GraduationWork.Models.Helpers;

namespace GraduationWork.Models
{
    public class Scene
    {
        private readonly List<Figure> figures = new List<Figure>();
        public List<Figure> Figures => this.figures;

        public Figure SelectedFigure => this.Figures.FirstOrDefault(x => x.IsSelected);

        [JsonIgnore] public int FigureCount;

        [JsonIgnore] public int MaxWidth { get; set; }

        [JsonIgnore] public int MaxHeight { get; set; }

        [JsonIgnore] public int RotationAngle { get; set; }

        [JsonIgnore] public double TechDist { get; set; }

        [JsonIgnore] public List<Figure> PlacedFigures { get; set; }

        [JsonIgnore] public bool IsPlaced { get; set; }

        [JsonIgnore] public int Rest { get; set; }

        public string Serialize()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(this, options);
        }

        public string SerializeFigure()
        {
            return this.SelectedFigure?.Serialize() ?? string.Empty;
        }

        public string DeserializeFigure(string jsonString)
        {
            var figure = JsonSerializer.Deserialize<Figure>(jsonString);
            figure.Id = FigureCount++;
            this.figures.Add(figure);
            this.PlaceFigure(figure);
            this.SelectFigure(figure.Id);
            return figure.Name + " " + figure.Id;
        }

        public void PlaceFigure(Figure figure)
        {
            var newX = - (figure.MaxX - figure.MinX) / 2;
            var newY = - (figure.MaxY - figure.MinY) / 2;
            var newZ = - (figure.MaxZ - figure.MinZ) / 2;
            figure.SetPosition(new Vector3D(newX, newY, newZ));
        }

        public void SelectFigure(int id)
        {
            var oldSelected = this.SelectedFigure;
            if (oldSelected != null)
            {
                oldSelected.IsSelected = false;
            }

            var figure = this.Figures.FirstOrDefault(x => x.Id == id);

            if (figure != null)
            {
                figure.IsSelected = true;
            }
        }

        public void Placing()
        {
            try
            {
                var placingFigures = this.Figures.OrderByDescending(x => (x.MaxX - x.MinX) * (x.MaxY - x.MinY))
                    .Select(x => x.Clone()).ToList();
                var placingArea = new int[this.MaxHeight, this.MaxWidth];

                this.PlacedFigures = new List<Figure>();

                foreach (var placingFigure in placingFigures)
                {
                    var figureEcv = placingFigure.GetEcv(this.TechDist);

                    var bP = new Vector2D(int.MinValue, int.MinValue);
                    var bR = int.MinValue;
                    var bA = 0;

                    for (var currentAngle = 0; currentAngle < 360; currentAngle += this.RotationAngle)
                    {
                        var modelArea = placingFigure.MatrixOfModel(figureEcv, this.TechDist, currentAngle);

                        var pos = MatrixHelper.Position(placingArea, modelArea);

                        if (pos.X != int.MinValue)
                        {
                            var res = MatrixHelper.Rest(placingArea, modelArea, (int) pos.X);
                            if (res > bR)
                            {
                                bR = res;
                                bA = currentAngle;
                                bP.X = pos.X;
                                bP.Y = pos.Y;
                            }
                        }

                        if (this.RotationAngle == 0)
                        {
                            break;
                        }
                    }

                    if (bP.X != int.MinValue)
                    {
                        var modelArea = placingFigure.MatrixOfModel(figureEcv, this.TechDist, bA);
                        for (int j = 0; j < modelArea.GetLength(0); j++)
                        {
                            for (int k = 0; k < modelArea.GetLength(1); k++)
                            {
                                if (placingArea[(int) bP.Y + j, (int) bP.X + k] == 1 && modelArea[j, k] == 0)
                                {
                                    continue;
                                }

                                placingArea[(int) bP.Y + j, (int) bP.X + k] = modelArea[j, k];
                            }
                        }

                        figureEcv.ForEach(x => x.Rotate(bA));

                        placingFigure.SetPosition(new Vector3D(bP.X, 0.0, bP.Y), bA, figureEcv);
                        this.PlacedFigures.Add(placingFigure);
                    }
                    else
                    {
                        this.PlacedFigures.Clear();
                        this.IsPlaced = false;
                        throw new Exception("Размещение одной или нескольких моделей невозможно");
                    }
                }

                this.IsPlaced = true;
                this.Rest = MatrixHelper.Rest(placingArea);
            }
            catch
            {
                this.IsPlaced = false;
                throw new Exception("При размещении произошла ошибка");
            }
        }

        private void PrintMatrix(int[,] matrix)
        {
            var result = string.Empty;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    result += $"{matrix[i, j]} ";
                }

                result += "\n";
            }
        }
    }
}
