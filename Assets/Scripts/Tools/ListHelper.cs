using Spacetaurant;
using System;
using System.Collections.Generic;

public static class ListHelper
{
    #region Sorting
    public static Comparison<T> CombineComparers<T>(List<Comparison<T>> comparisons) => CombineComparers(comparisons.ToArray());
    public static Comparison<T> CombineComparers<T>(params Comparison<T>[] comparisons)
    {  
        return Comparer;

        int Comparer(T value1, T value2)
        {
            for (int i = 0; i < comparisons.Length; i++)
            {
                int result = comparisons[i](value1, value2);
                if (result != 0)
                    return result;
            }
            return 0;
        }
    }
    #endregion
}
