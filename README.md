[![](https://img.shields.io/nuget/v/Soenneker.Utils.Random.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Utils.Random/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.random/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.random/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Utils.Random.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Utils.Random/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.Random
### A thread-safe random utility library

## Installation

```
dotnet add package Soenneker.Utils.Random
```

## Usage

```csharp
// Returns an integer >= 5, and < 20
RandomUtil.Next(5, 20);
```

```csharp
// Returns a double >= 5, and < 20
RandomUtil.NextDouble(5, 20);
```

```csharp
// Returns a random number with a uniform and discrete distribution
RandomUtil.NextDecimalUniform(5, 20);
```