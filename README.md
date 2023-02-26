[![](https://img.shields.io/nuget/v/Soenneker.Utils.Random.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Utils.Random/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.random/main.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.random/actions/workflows/main.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Utils.Random.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Utils.Random/)

# Soenneker.Utils.Random
### A tiny thread-safe random utility library

## Installation

```
Install-Package Soenneker.Utils.Random
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

etc.