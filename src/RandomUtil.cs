using System;
using System.Diagnostics.Contracts;

namespace Soenneker.Utils.Random;

/// <summary>
/// Wraps <see cref="System.Random.Shared"/> (thread-safe) and implements a few additional methods
/// </summary>
public static class RandomUtil
{
    /// <summary>Returns a non-negative random integer that is less than the specified maximum.</summary>
    /// <param name="maxValue">The exclusive upper bound of the random number to be generated. <paramref name="maxValue"/> must be greater than or equal to 0.</param>
    /// <returns>
    /// A 32-bit signed integer that is greater than or equal to 0, and less than <paramref name="maxValue"/>; that is, the range of return values ordinarily
    /// includes 0 but not <paramref name="maxValue"/>. However, if <paramref name="maxValue"/> equals 0, <paramref name="maxValue"/> is returned.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxValue"/> is less than 0.</exception>
    [Pure]
    public static int Next(int maxValue)
    {
        return System.Random.Shared.Next(maxValue);
    }

    /// <summary>Returns a random integer that is within a specified range.</summary>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.</param>
    /// <returns>
    /// A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/>
    /// but not <paramref name="maxValue"/>. If minValue equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
    [Pure]
    public static int Next(int minValue, int maxValue)
    {
        return System.Random.Shared.Next(minValue, maxValue);
    }

    /// <summary>Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.</summary>
    /// <returns>A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
    [Pure]
    public static double NextDouble()
    {
        return System.Random.Shared.NextDouble();
    }

    [Pure]
    public static double NextDouble(double minValue, double maxValue)
    {
        double result = NextDouble() * (maxValue - minValue) + minValue;
        return result;
    }

    [Pure]
    public static decimal NextDecimal(decimal minValue, decimal maxValue, int? roundingDigits = null)
    {
        decimal result = (decimal) NextDouble() * (maxValue - minValue) + minValue;

        if (roundingDigits != null)
            result = Math.Round(result, roundingDigits.Value);

        return result;
    }
}