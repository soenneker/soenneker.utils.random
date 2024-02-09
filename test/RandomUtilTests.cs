using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Soenneker.Utils.Random.Tests;

public class RandomUtilTests
{
    [Theory]
    [InlineData(2, 5)]
    [InlineData(5.5, 7.2)]
    [InlineData(7.54, 10.25)]
    public void NextDecimal_should_give_result_in_range(decimal min, decimal max)
    {
        decimal result = RandomUtil.NextDecimal(min, max);

        result.Should().BeInRange(min, max);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(5.5, 5.5)]
    [InlineData(10.25, 10.25)]
    public void NextDecimal_should_give_input_result_if_equal(decimal min, decimal max)
    {
        decimal result = RandomUtil.NextDecimal(min, max);

        result.Should().BeInRange(min, max);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(5.5, 5.5)]
    [InlineData(10.25, 10.25)]
    public void NextDecimalUniform_should_give_input_result_if_equal(decimal min, decimal max)
    {
        decimal result = RandomUtil.NextDecimalUniform(min, max);

        result.Should().BeInRange(min, max);
    }

    [Theory]
    [InlineData(2, 5)]
    [InlineData(5.5, 7.2)]
    [InlineData(7.54, 10.25)]
    public void NextDecimalUniform_should_give_result_in_range(decimal min, decimal max) {
        decimal result = RandomUtil.NextDecimalUniform(min, max);

        result.Should().BeInRange(min, max);
    }

    [Fact]
    public void WeightedRandomSelection_should_give_back_result()
    {
        List<string> items = new List<string> { "A", "B", "C" };
        List<int> weights = new List<int> { 3, 10, 2 };

        for (int i = 0; i < 20; i++)
        {
            string selected = RandomUtil.WeightedRandomSelection(items, weights);
            selected.Should().NotBeNull();
        }
    }
}