[![](https://img.shields.io/nuget/v/Soenneker.Utils.Random.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Utils.Random/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.random/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.random/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Utils.Random.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Utils.Random/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.random/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.random/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.Random
### Thread-safe convenience methods built on `Random.Shared`

## Installation

```
dotnet add package Soenneker.Utils.Random
```

This package is for simulation, sampling, jitter, and other non-secret randomness. `Random.Shared`
is not cryptographically secure. Use `RandomNumberGenerator` for passwords, tokens, keys, salts,
or any value whose predictability affects security.

## Numeric ranges

```csharp
int index = RandomUtil.Next(5, 20);           // [5, 20)
double sample = RandomUtil.NextDouble(5, 20); // [5, 20) for ordinary finite bounds
int anyInt = RandomUtil.NextInt32();          // full Int32 range
```

`Next(int)` and `Next(min, max)` use the same exclusive-upper-bound rules as `System.Random`.
`NextDouble()` returns `[0, 1)`. The ranged double overload applies
`sample * (maxValue - minValue) + minValue` and does not validate reversed, NaN, or infinite bounds.

## Decimal values

```csharp
decimal highResolution = RandomUtil.NextDecimalUniform(5m, 20m);
decimal faster = RandomUtil.NextDecimal(5m, 20m, roundingDigits: 2);
```

`NextDecimalUniform()` produces a high-resolution discrete value in `[0, 1)`. Its internal mapping
has a small modulo bias, so it should not be treated as mathematically exact uniform sampling.
The ranged overload scales that value into the requested interval.

`NextDecimal` converts a random double to decimal before scaling. Both ranged decimal methods can
round with `Math.Round`'s default midpoint-to-even behavior; rounding can produce an endpoint that
the unrounded sample would not reach. Neither method validates that minimum is less than maximum.

## Weighted selection

```csharp
string selected = RandomUtil.WeightedRandomSelection(
    new[] { "small", "medium", "large" },
    new[] { 1.0, 3.0, 1.0 });
```

Selection probability is proportional to each weight. Items and weights must have the same
nonzero count; weights must be finite and non-negative, with at least one positive value. Inputs
are read during the call and should not be mutated concurrently.

## Random delay

```csharp
await RandomUtil.Delay(
    minValue: 100,
    maxValue: 500,
    logger,
    cancellationToken);
```

Delay values are milliseconds and use `[minValue, maxValue)`, except equal bounds produce that
single value. Cancellation propagates. The optional logger records the chosen delay and canceled
waits at debug level.
