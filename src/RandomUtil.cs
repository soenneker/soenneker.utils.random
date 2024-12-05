using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Soenneker.Utils.Random;

/// <summary>
/// A thread-safe random utility library
/// </summary>
/// <remarks>
/// Wraps <see cref="System.Random.Shared"/> and implements additional methods
/// </remarks>
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

    /// <summary>Returns a random floating-point number that is between the range specified.</summary>
    /// <returns>A double-precision floating point number that is between the range specified.</returns>
    [Pure]
    public static double NextDouble(double minValue, double maxValue)
    {
        return NextDouble() * (maxValue - minValue) + minValue;
    }

    /// <summary>
    /// Returns an Int32 with a random value across the entire range of possible values.
    /// </summary>
    [Pure]
    public static int NextInt32()
    {
        int firstBits = System.Random.Shared.Next(0, 1 << 4) << 28;
        int lastBits = System.Random.Shared.Next(0, 1 << 28);
        return firstBits | lastBits;
    }

    /// <summary>
    /// Provides a random decimal value in the range with a uniform and discrete distribution.
    /// </summary>
    /// <returns>Values [0.0000000000000000000000000000, 0.9999999999999999999999999999)</returns>
    [Pure]
    public static decimal NextDecimalUniform()
    {
        var result = 1m;

        // Essentially shouldn't ever happen that loops, but we have this here just in case
        while (result >= 1)
        {
            int a = NextInt32();
            int b = NextInt32();
            //The bits for 0.9999999999999999999999999999m are 542101086
            int c = System.Random.Shared.Next(542101087);
            result = new decimal(a, b, c, false, 28);
        }

        return result;
    }

    /// <summary>
    /// Provides a random decimal value in the range with a uniform and discrete distribution.
    /// </summary>
    [Pure]
    public static decimal NextDecimalUniform(decimal minValue, decimal maxValue, int? roundingDigits = null)
    {
        decimal nextDecimalUniform = NextDecimalUniform();
        decimal result = maxValue * nextDecimalUniform + minValue * (1 - nextDecimalUniform);

        if (roundingDigits != null)
            result = Math.Round(result, roundingDigits.Value);

        return result;
    }

    /// <summary>
    /// Provides a floating-point number between the range (using <see cref="NextDouble()"/>). For a uniform and discrete decimal, use <see cref="NextDecimalUniform()"/>. 
    /// </summary>
    /// 
    [Pure]
    public static decimal NextDecimal(decimal minValue, decimal maxValue, int? roundingDigits = null)
    {
        decimal result = (decimal) NextDouble() * (maxValue - minValue) + minValue;

        if (roundingDigits != null)
            result = Math.Round(result, roundingDigits.Value);

        return result;
    }

    [Pure]
    public static T WeightedRandomSelection<T>(IList<T> items, IList<double> weights)
    {
        ArgumentNullException.ThrowIfNull(items, nameof(items));
        ArgumentNullException.ThrowIfNull(weights, nameof(weights));

        if (items.Count == 0 || items.Count != weights.Count)
            throw new ArgumentException("Invalid input: items and weights must have the same length and not be empty.");

        double totalWeight = 0;

        for (var i = 0; i < weights.Count; i++)
        {
            double weight = weights[i];

            if (weight < 0)
                throw new ArgumentException("All weights must be non-negative.");

            totalWeight += weight;
        }

        // Total weight must be greater than zero for a valid selection. (All have 0 chance?)
        if (totalWeight == 0)
            throw new ArgumentException("Total weight must be greater than zero.");

        double randomValue = NextDouble() * totalWeight;

        double cumulativeWeight = 0;

        for (var i = 0; i < items.Count; i++)
        {
            cumulativeWeight += weights[i];

            if (randomValue < cumulativeWeight)
            {
                return items[i];
            }
        }

        // This line should never be reached, but included for compiler satisfaction
        return items[items.Count - 1];
    }
}