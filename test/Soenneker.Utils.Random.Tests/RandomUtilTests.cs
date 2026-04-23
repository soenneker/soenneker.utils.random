using AwesomeAssertions;
using System.Collections.Generic;

namespace Soenneker.Utils.Random.Tests;

public class RandomUtilTests
{
    [Test]
    [Arguments(2, 5)]
    [Arguments(5.5, 7.2)]
    [Arguments(7.54, 10.25)]
    public void NextDecimal_should_give_result_in_range(decimal min, decimal max)
    {
        decimal result = RandomUtil.NextDecimal(min, max);

        result.Should().BeInRange(min, max);
    }

    [Test]
    [Arguments(0, 0)]
    [Arguments(5.5, 5.5)]
    [Arguments(10.25, 10.25)]
    public void NextDecimal_should_give_input_result_if_equal(decimal min, decimal max)
    {
        decimal result = RandomUtil.NextDecimal(min, max);

        result.Should().BeInRange(min, max);
    }

    [Test]
    [Arguments(0, 0)]
    [Arguments(5.5, 5.5)]
    [Arguments(10.25, 10.25)]
    public void NextDecimalUniform_should_give_input_result_if_equal(decimal min, decimal max)
    {
        decimal result = RandomUtil.NextDecimalUniform(min, max);

        result.Should().BeInRange(min, max);
    }

    [Test]
    [Arguments(2, 5)]
    [Arguments(5.5, 7.2)]
    [Arguments(7.54, 10.25)]
    public void NextDecimalUniform_should_give_result_in_range(decimal min, decimal max) {
        decimal result = RandomUtil.NextDecimalUniform(min, max);

        result.Should().BeInRange(min, max);
    }

    [Test]
    public void WeightedRandomSelection_should_give_back_result()
    {
        List<string> items =
        [
            "A",
            "B",
            "C"
        ];

        List<double> weights =
        [
            3,
            10,
            2
        ];

        for (var i = 0; i < 20; i++)
        {
            string selected = RandomUtil.WeightedRandomSelection(items, weights);
            selected.Should().NotBeNull();
        }
    }
}
