using System.Collections.Generic;
using System.Diagnostics;
using GraduationWork.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GraduationWork.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Сцена.
        /// </summary>
        private static readonly Scene Scene = new();

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string GetFigures()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var mainInfo = Scene.Figures.Select(x =>
            {
                var clone = x.Clone(true);
                clone.IsSelected = x.IsSelected;
                clone.Id = x.Id;
                return clone;
            });

            return JsonSerializer.Serialize(mainInfo, options);
        }

        [HttpGet]
        public string GetSelectedFigure()
        {
            return Scene.SerializeFigure();
        }

        [HttpGet]
        public string GetEquidistantFigure()
        {
            if (!Scene.IsPlaced)
            {
                return string.Empty;
            }

            var list = Scene.PlacedFigures.Select(x => new KeyValuePair<string,List<Vector2D>>(x.Color, x.PlacedEcv));

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(list, options);
        }

        [HttpPost]
        public string PostFigure()
        {
            var content = GetContent();
            return Scene.DeserializeFigure(content);
        }

        [HttpPost]
        public void ClearScene()
        {
            Scene.Figures.Clear();
            Scene.FigureCount = 0;
        }

        [HttpPost]
        public void SetSelectedFigure()
        {
            var content = GetContent();

            if (int.TryParse(content, out var result))
            {
                Scene.SelectFigure(result);
            }
        }

        [HttpPost]
        public void SetRotationSelectedFigure()
        {
            var content = GetContent();
            if (!string.IsNullOrEmpty(content))
            {
                var words = content.Split(' ');
                var figure = Scene.SelectedFigure;
                figure.Rotation.X = double.Parse(words[0], CultureInfo.InvariantCulture);
                figure.Rotation.Y = double.Parse(words[1], CultureInfo.InvariantCulture);
                figure.Rotation.Z = double.Parse(words[2], CultureInfo.InvariantCulture);
            }
        }

        [HttpPost]
        public void SetColorSelectedFigure()
        {
            var content = GetContent();
            if (!string.IsNullOrEmpty(content))
            {
                var figure = Scene.SelectedFigure;
                figure.Color = content;
            }
        }

        [HttpPost]
        public void SetPlaceParameters()
        {
            var content = GetContent();
            if (!string.IsNullOrEmpty(content))
            {
                var words = content.Split(' ');
                Scene.MaxWidth = int.Parse(words[0]);
                Scene.MaxHeight = int.Parse(words[1]);
                Scene.RotationAngle = int.Parse(words[2]);
                Scene.TechDist = double.Parse(words[3], CultureInfo.InvariantCulture);
            }
        }

        [HttpGet]
        public string GetPlacedScene()
        {
            if (!Scene.IsPlaced)
            {
                return string.Empty;
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(Scene.PlacedFigures, options);
        }

        [HttpGet]
        public string GetPlacedEcv()
        {
            if (!Scene.IsPlaced)
            {
                return string.Empty;
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var dict = Scene.PlacedFigures.Select(x =>
                new KeyValuePair<string, IEnumerable<Vector2D>>(x.Color, x.PlacedEcv));

            return JsonSerializer.Serialize(dict, options);
        }

        [HttpPost]
        public string PlaceScene()
        {
            try
            {
                var watch = new Stopwatch();
                watch.Start();
                Scene.Placing();
                watch.Stop();

                var maxHeight = Scene.PlacedFigures.Max(x => x.MaxY - x.MinY);

                var resultTime = watch.Elapsed;
                return
                    $"Время размещения: {resultTime.Hours:00}:{resultTime.Minutes:00}:{resultTime.Seconds:00}.{resultTime.Milliseconds:000} " +
                    $"\nВысота блока: {maxHeight}" +
                    $"\nДлина незанятой части блока: {Scene.Rest}";
            }
            catch
            {
                return "Размещение не удалось";
            }
        }

        public string GetContent()
        {
            var auto = Request.Body;
            var streamReader = new StreamReader(auto);
            return streamReader.ReadToEndAsync().Result;
        }
    }
}
