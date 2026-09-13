using System;
using System.Collections.Generic;
using AwesomeAssertions;
using Microsoft.Extensions.Logging;

namespace Soenneker.Utils.Random.Tests;

public class PerformanceRegressionTests
{
    [Test]
    public void Weighted_selection_handles_overflow_subnormal_and_zero_weights()
    {
        for (int i = 0; i < 1000; i++)
        {
            RandomUtil.WeightedRandomSelection(new[] { 0, 1, 2 }, new[] { 0d, double.Epsilon, 0d }).Should().Be(1);
            RandomUtil.WeightedRandomSelection(new[] { 0, 1, 2 }, new[] { double.MaxValue, 0d, double.MaxValue }).Should().BeOneOf(0, 2);
        }
        foreach (double bad in new[] { double.NaN, double.PositiveInfinity, -1d })
        {
            Action select = () => RandomUtil.WeightedRandomSelection(new[] { 0, 1 }, new[] { 1d, bad });
            select.Should().Throw<ArgumentException>();
        }
    }

    [Test]
    public void Weighted_selection_approximately_tracks_relative_weights()
    {
        int[] counts = new int[3];
        int[] items = [0, 1, 2];
        double[] weights = [1, 3, 6];
        for (int i = 0; i < 100000; i++)
            counts[RandomUtil.WeightedRandomSelection(items, weights)]++;
        counts[0].Should().BeInRange(8500, 11500);
        counts[1].Should().BeInRange(28000, 32000);
        counts[2].Should().BeInRange(57500, 62500);

        int first = 0;
        int[] pair = [0, 1];
        double[] subnormalWeights = [double.Epsilon, double.Epsilon];
        for (int i = 0; i < 100000; i++)
            if (RandomUtil.WeightedRandomSelection(pair, subnormalWeights) == 0) first++;
        first.Should().BeInRange(47500, 52500);
    }

    [Test]
    public async System.Threading.Tasks.Task Delay_preserves_structured_logging()
    {
        var logger = new RecordingLogger();
        await RandomUtil.Delay(0, 0, logger);
        logger.Message.Should().Be("Delaying for 0ms...");
        logger.Milliseconds.Should().Be(0);
    }

    private sealed class RecordingLogger : ILogger
    {
        public string? Message;
        public int? Milliseconds;
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel level) => true;
        public void Log<TState>(LogLevel level, EventId id, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Message = formatter(state, exception);
            if (state is IEnumerable<KeyValuePair<string, object?>> properties)
                foreach (var property in properties)
                    if (property.Key == "ms") Milliseconds = (int)property.Value!;
        }
    }
}
