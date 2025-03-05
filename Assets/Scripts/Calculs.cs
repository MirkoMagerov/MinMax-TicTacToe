using UnityEngine;

public static class Calculs
{
    public static float LinearDistance;
    public static Vector2 FirstPosition;
    private static float offset = 0.1f;

    public static void CalculateDistances(BoxCollider2D coll, float size)
    {
        LinearDistance = coll.size.x / size;
        FirstPosition = new Vector2(-size / 2f, size / 2f);
    }

    public static Vector2 CalculatePoint(int x, int y)
    {
        return FirstPosition + new Vector2(x * LinearDistance, -y* LinearDistance);
    }

    public static int EvaluateWin(int[,] matrix)
    {
        int counterX = 0;
        int counterY = 0;
        int counterD1 = 0;
        int counterD2 = 0;
        for(int i=0; i<matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1);j++)
            {
                counterY += matrix[i, j];
                counterX += matrix[j, i];
            }
            if (counterY == 3 || counterX == 3) return 1;
            else if (counterY == -3 || counterX == -3) return -1;
            counterX = 0;
            counterY = 0;
            counterD1 += matrix[i, i];
            counterD2 += matrix[2-i, i];
        }
        if (counterD1 == 3 || counterD2 == 3) return 1;
        else if(counterD1 == -3 || counterD2 == -3)  return -1;
        for(int i=0; i<matrix.GetLength(0);i++)
        {
            for(int j = 0; j < matrix.GetLength(1);j++)
            {
                if (matrix[i, j] == 0) return 2;
            }
        }
        return 0; // 0 empat, 1 guanya 1, -1 guanya 2, 2 no s'ha acabat
    }

    public static bool CheckIfValidClick(Vector2 mousePosition, int[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Vector2 point = CalculatePoint(i, j);
                if (Mathf.Abs(mousePosition.x - point.x) < LinearDistance / 2f - offset
                    && Mathf.Abs(mousePosition.y - point.y) < LinearDistance / 2f - offset)
                {
                    if (matrix[i, j] == 0)
                    {
                        GameManager.Instance.DoMove(i, j, 1);
                        return true;
                    }
                }
                //Debug.Log(CalculatePoint(i, j));
            }
        }
        return false;
    }
}
