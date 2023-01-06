namespace GraduationWork.Models.Helpers
{
    public class MatrixHelper
    {
        public static Vector2D Position(int[,] area, int[,] model)
        {
            if (area.GetLength(0) < model.GetLength(0) || area.GetLength(1) < model.GetLength(1))
            {
                return new Vector2D(int.MinValue, int.MinValue);
            }
            var find = true;
            for (var j = 0; j < area.GetLength(1) - model.GetLength(1); j++)
            {
                for (var i = 0; i < area.GetLength(0) - model.GetLength(0); i++)
                {
                    for (var k = 0; k < model.GetLength(0); k++)
                    {
                        for (var s = 0; s < model.GetLength(1); s++)
                        {
                            if (area[i + k, j + s] == 1 && model[k, s] == 1)
                            {
                                find = false;
                                break;
                            }
                        }
                        if (!find)
                        {
                            break;
                        }
                    }
                    if (find)
                    {
                        return new Vector2D(j, i); ;
                    }
                    else
                    {
                        find = true;
                    }
                }
            }
            return new Vector2D(int.MinValue, int.MinValue);
        }

        public static int Rest(int[,] area, int[,] model, int X)
        {
            return area.GetLength(1) - (X + model.GetLength(1));
        }

        public static int Rest(int[,] area)
        {
            for (int i = area.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = area.GetLength(1) - 1; j >= 0; j--)
                {
                    if (area[i, j] == 1)
                        return (i + 1);
                }
            }
            return 0;
        }
    }
}
