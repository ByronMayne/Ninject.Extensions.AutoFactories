[![NuGet Version](http://img.shields.io/nuget/v/Ninject.Extension.AutoFactories.svg?style=flat)](https://www.nuget.org/packages/Ninject.Extension.AutoFactories/) 
[![NuGet Downloads](http://img.shields.io/nuget/dt/Ninject.Extension.AutoFactories.svg?style=flat)](https://www.nuget.org/packages/Ninject.Extension.AutoFactories/)
![Issues](https://img.shields.io/github/issues-closed/ByronMayne/Ninject.Extensions.AutoFactories)

## AutoFactories

### The Problem

When using Dependency Injection (DI) containers, you often need to create new instances of objects at runtime—not just at startup. However, doing this directly with the container in your application code defeats the whole purpose of DI: keeping your code decoupled from the container.

The typical solution is to use **factories**: wrapper classes that handle the complexity of creating instances while keeping the container logic isolated. Unfortunately, writing factories is tedious boilerplate code that gets repeated across your codebase.

### The Solution

AutoFactories automatically generates factory classes for you during compilation. Just add a single attribute to your class, and you get a fully-typed factory with all the boilerplate written for you—automatically.

## Quick Example

This code below uses this library to auto generate the `IShippingOrderFactory` and its `Get` method. Just add `[AutoFactory]` to your class, and the factory is generated automatically. Parameters marked with `[FromFactory]` come from your DI container, while others become factory method arguments.

```cs
namespace Operations 
{
    [AutoFactory] 
    public class ShippingOrder 
    {
        public ShippingOrder(
            string orderId,
            [FromFactory] IShippingProvider provider)
        {}
    }
    public class Program 
    {
        public static void Main(string[] args)
        {
            IShippingProvider provider = new ShippingProvider();
            // The factory is auto-generated with a constructor for DI dependencies
            IShippingOrderFactory factory = new ShippingOrderFactory(provider);
            // The Create method only takes user-provided parameters
            ShippingOrder shippingOrder = factory.Create(orderId:"A2123F")
        }
    }
}
```

### Using with Dependency Injection Frameworks

AutoFactories works seamlessly with popular DI containers:

#### Ninject
```cs
public static void Main(string[] args)
{
    IKernel kernel = new StandardKernel()
        .AddAutoFactories(); // Registers all generated factories
    IShippingOrderFactory factory = kernel.Get<IShippingOrderFactory>();
    ShippingOrder shippingOrder = factory.Create(orderId:"A2123F")
}
```

#### Microsoft.DependencyInjection
```cs
using Microsoft.Extensions.DependencyInjection;

public static void Main(string[] args)
{
    IServiceProvider serviceProvider = new ServiceCollection()
        .AddAutoFactories() // Registers all generated factories
        .BuildServiceProvider();

    IShippingOrderFactory factory = serviceProvider.GetRequiredService<IShippingOrderFactory>();
    ShippingOrder shippingOrder = factory.Create(orderId:"A2123F")
}
```

## How It Works

### What Gets Generated?

When you apply `[AutoFactory]` to a class, AutoFactories generates:

1. **A Factory Class** - contains the factory methods
2. **A Factory Interface** - for dependency injection
3. **Create Methods** - one for each constructor in your class

Parameters marked with `[FromFactory]` are automatically resolved by the container. All other parameters become method arguments, with compile-time type safety (no more magic string parameter names).

### Example

This class:

```csharp
[AutoFactory]
public class Coffee
{
    public Size Size { get; }
    public IMilkService Milk { get; }

    public Coffee(
        Size size,
        [FromFactory] IMilkService milkService)
    {}
}
```

Generates this factory (simplified):

```csharp
public interface ICoffeeFactory
{
    Coffee Create(Size size);
}

internal partial class CoffeeFactory : ICoffeeFactory
{
    private readonly IResolutionRoot m_resolutionRoot;

    public CoffeeFactory(IResolutionRoot resolutionRoot)
    {
        m_resolutionRoot = resolutionRoot;
    }

    public Coffee Create(Size size)
    {
        return m_resolutionRoot.Get<Coffee>(
            new ConstructorParameter(nameof(size), size));
    }
}
```

Notice: the parameter name is derived from your source code, so if someone refactors `size` to `coffeeSize`, you get a compile-time error—not a runtime crash.

## Why Use Factories?

### The Traditional Problem

Without factories, you have two bad choices:

1. **Reference the container everywhere** - Your business logic becomes tightly coupled to the DI framework
2. **Manually write factories** - Lots of repetitive boilerplate that's easy to get wrong

AutoFactories solves both problems by generating type-safe factories automatically.

## Attribute Options

The `[AutoFactory]` attribute is highly configurable, giving you control over how factories are generated:

### Default Behavior
```csharp
[AutoFactory]
public class Coffee { }
```
Generates a factory class named `CoffeeFactory` with an interface `ICoffeeFactory` and a method named `Create()`.

### Custom Method Name
Specify the name of the factory method:
```csharp
[AutoFactory]
public class Coffee { }

[AutoFactory]
public class Espresso { }

public partial class BeverageFactory { }

[AutoFactory(typeof(BeverageFactory), "MakeCoffee")]
public class Coffee { }

[AutoFactory(typeof(BeverageFactory), "MakeEspresso")]
public class Espresso { }
```
This generates both `MakeCoffee()` and `MakeEspresso()` methods in a single `BeverageFactory` class, eliminating naming collisions.

### Shared Factory for Multiple Types
Combine multiple types into one factory:
```csharp
public partial class AnimalFactory { }

[AutoFactory(typeof(AnimalFactory), "CreateCat")]
public class Cat
{
    public Cat(string name, [FromFactory] IVeterinarian vet) { }
}

[AutoFactory(typeof(AnimalFactory), "CreateDog")]
public class Dog
{
    public Dog(string breed, [FromFactory] IVeterinarian vet) { }
}
```
Generates:
```csharp
public partial class AnimalFactory : IAnimalFactory
{
    private readonly IVeterinarian m_vet;
    
    public AnimalFactory(IVeterinarian vet) => m_vet = vet;
    
    public Cat CreateCat(string name) => new Cat(name, m_vet);
    public Dog CreateDog(string breed) => new Dog(breed, m_vet);
}
```

## Advanced Usage

### Expose As
Change the return type of the generated factory method using `ExposeAs`:

```cs
public interface IClock 
{
    int Ticks { get; }
}

[AutoFactory(ExposeAs=typeof(IClock))]
public class Clock : IClock 
{
    public int Ticks { get; }

    public Clock(int ticks)
    {
        Ticks = ticks;
    }
}
```

Generates:

```cs
public interface IClockFactory
{
    // Returns the interface, not the concrete type
    IClock Create(int ticks);
}
```

## Technical Details

### What is AutoFactories?

AutoFactories is a [Source Generator](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) that generates factory code during compilation. The generated code is:

- **Type-safe** - Full IntelliSense and compile-time checking
- **Zero-overhead** - No reflection or runtime costs
- **Standard C#** - Works with any C# 9+ project

### Framework Support

AutoFactories provides special integrations for:

- **Generic factories** - No framework dependencies
- **Ninject** - Full integration with Ninject's kernel
- **Microsoft.DependencyInjection** - Works with ASP.NET Core and other frameworks

Don't see your framework? [Open a feature request](https://github.com/ByronMayne/AutoFactories/issues/new).