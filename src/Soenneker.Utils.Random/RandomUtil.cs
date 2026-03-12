using Microsoft.Extensions.Logging;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Utils.Random;

/// <summary>
/// A thread-safe random utility library
/// </summary>
/// <remarks>
/// Wraps <see cref="System.Random.Shared"/> and implements additional methods
/// </remarks>
public static class RandomUtil
{
    private const int _decimalScale = 28;
    private const int _cUpperExclusive = 542_101_087;

    /// <summary>Returns a non-negative random integer that is less than the specified maximum.</summary>
    /// <param name="maxValue">The exclusive upper bound of the random number to be generated. <paramref name="maxValue"/> must be greater than or equal to 0.</param>
    /// <returns>
    /// A 32-bit signed integer that is greater than or equal to 0, and less than <paramref name="maxValue"/>; that is, the range of return values ordinarily
    /// includes 0 but not <paramref name="maxValue"/>. However, if <paramref name="maxValue"/> equals 0, <paramref name="maxValue"/> is returned.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxValue"/> is less than 0.</exception>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next(int minValue, int maxValue)
    {
        return System.Random.Shared.Next(minValue, maxValue);
    }

    /// <summary>Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.</summary>
    /// <returns>A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double NextDouble()
    {
        return System.Random.Shared.NextDouble();
    }

    /// <summary>Returns a random floating-point number that is between the range specified.</summary>
    /// <returns>A double-precision floating point number that is between the range specified.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        return (int)System.Random.Shared.NextInt64(int.MinValue, (long)int.MaxValue + 1);
    }

    /// <summary>
    /// Provides a random decimal value in the range with a uniform and discrete distribution.
    /// </summary>
    /// <returns>Values [0.0000000000000000000000000000, 0.9999999999999999999999999999)</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal NextDecimalUniform()
    {
#pragma warning disable CA2014 // stackalloc is fixed-size (12 bytes) and safe in bounded loop
        while (true)
        {
            Span<byte> bytes = stackalloc byte[12];
            System.Random.Shared.NextBytes(bytes);

            int a = BinaryPrimitives.ReadInt32LittleEndian(bytes);
            int b = BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(4));
            int c = BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(8));

            // Map c into [0, _cUpperExclusive)
            // (uint) makes negatives well-defined; modulo bias here is tiny
            c = (int)((uint)c % _cUpperExclusive);

            var result = new decimal(a, b, c, isNegative: false, scale: _decimalScale);

            if (result < 1m)
                return result;
        }
#pragma warning restore CA2014
    }

    /// <summary>
    /// Provides a random decimal value in the range with a uniform and discrete distribution.
    /// </summary>
    [Pure]
    public static decimal NextDecimalUniform(decimal minValue, decimal maxValue, int? roundingDigits = null)
    {
        decimal u = NextDecimalUniform();
        decimal result = minValue + (maxValue - minValue) * u;

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
        decimal result = (decimal)NextDouble() * (maxValue - minValue) + minValue;

        if (roundingDigits != null)
            result = Math.Round(result, roundingDigits.Value);

        return result;
    }

    [Pure]
    public static T WeightedRandomSelection<T>(IList<T> items, IList<double> weights)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(weights);

        int count = items.Count;
        if ((uint)count == 0 || count != weights.Count)
            throw new ArgumentException("Invalid input: items and weights must have the same length and not be empty.");

        double total = 0;
        int selectedIndex = -1;

        for (var i = 0; i < count; i++)
        {
            double w = weights[i];
            if (w < 0)
                throw new ArgumentException("All weights must be non-negative.");

            if (w == 0)
                continue;

            // Increase running total, then choose current item with probability w/total.
            total += w;

            // Equivalent to: if random in [0,total) falls in the newest slice (total-w, total)
            // Using NextDouble()*total < w avoids needing a second pass.
            if (NextDouble() * total < w)
                selectedIndex = i;
        }

        if (selectedIndex < 0)
            throw new ArgumentException("Total weight must be greater than zero.");

        return items[selectedIndex];
    }

    /// <summary>
    /// Asynchronously delays execution for a random duration between the specified minimum and maximum values.
    /// </summary>
    /// <param name="minValue">The minimum delay duration in milliseconds. Must be non-negative.</param>
    /// <param name="maxValue">The maximum delay duration in milliseconds. Must be greater than or equal to <paramref name="minValue"/>.</param>
    /// <param name="logger">An optional logger to log the delay duration and cancellation events.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the delay to complete.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous delay operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="minValue"/> is negative or if <paramref name="maxValue"/> is less than <paramref name="minValue"/>.
    /// </exception>
    /// <exception cref="TaskCanceledException">
    /// Thrown if the delay is canceled via the <paramref name="cancellationToken"/>.
    /// </exception>
    /// <remarks>
    /// This method uses a random delay duration generated between <paramref name="minValue"/> and <paramref name="maxValue"/> milliseconds.
    /// It logs the delay duration before initiating the delay and logs a cancellation message if the delay is canceled.
    /// 
    /// **Usage Considerations:**
    /// - **ValueTask Constraints:** Consumers should await the returned <see cref="ValueTask"/> only once and avoid storing it for later use to prevent undefined behavior.
    /// - **Logging:** Ensure that the provided <paramref name="logger"/> is appropriately configured to handle debug-level logs.
    /// - **Cancellation:** If the operation is canceled, a <see cref="TaskCanceledException"/> is rethrown to allow higher-level handlers to respond accordingly.
    /// </remarks>
    public static ValueTask Delay(int minValue, int maxValue, ILogger? logger = null, CancellationToken cancellationToken = default)
    {
        int ms = Next(minValue, maxValue);

        if (logger is null)
            return new ValueTask(Task.Delay(ms, cancellationToken));

        logger.LogDebug("Delaying for {ms}ms...", ms);

        return DelayLogged(ms, logger, cancellationToken);
    }

    private static async ValueTask DelayLogged(int ms, ILogger logger, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(ms, cancellationToken)
                      .ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            logger.LogDebug("Delay was canceled");
            throw;
        }
    }
}