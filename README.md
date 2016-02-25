Set of railway-oriented C# extension methods which could be used as building blocks for data-flow style application.

Inspired by [Railway oriented programming](http://fsharpforfunandprofit.com/posts/recipe-part2/)

# Nuget

[![Nuget](https://img.shields.io/nuget/v/CSharpRailway.svg)](https://www.nuget.org/packages/CSharpRailway/) 
[![Nuget](https://img.shields.io/nuget/dt/CSharpRailway.svg)](https://www.nuget.org/packages/CSharpRailway/)

# Supported railway-oriented functions

**Function** | **Description**
--- | ---
```Compose``` | Normal composition. A combiner that takes two normal functions and creates a new function by connecting them in series.
```Bind``` | An adapter that takes a switch function and creates a new function that accepts two-track values as input.
```TryCatch``` | An adapter that takes a normal one-track function and turns it into a switch function, but also catches exceptions.
```Switch``` | An adapter that takes a normal one-track function and turns it into a switch function. (Also known as a "lift" in some contexts.)
```Tee``` | An adapter that takes a dead-end function and turns it into a one-track function that can be used in a data flow. (Also known as tap.)
```Map``` | An adapter that takes a normal one-track function and turns it into a two-track function. (Also known as a "lift" in some contexts.)
```DoubleMap``` | An adapter that takes two one-track functions and turns them into a single two-track function. (Also known as bimap.)

# Usage Example

```csharp
// combine validation functions
var combinedValidation = validateName
	.Compose(validateEmail.Bind())
	.Compose(logRequest.Tee().Switch().Bind())
	.Compose(throwException.TryCatch().Bind())
	.Compose(nameToUpper.Switch().Bind())
	.Compose(appendDashToName.DoubleMap(logFailure))
	.Compose(emailToUpper.Map());

// invoke combined function
//var result = combinedValidation (new Request { Name = "", Email = "a@b.c" });
//var result = combinedValidation (new Request { Name = "Kirill", Email = "" });
var result = combinedValidation (new Request { Name = "Kirill", Email = "a@b.c" });

// process result
switch (result.IsSuccess) {
case true:
	Console.WriteLine ("Success. {0} - {1}", result.Value.Name, result.Value.Email);
	break;
case false:
	Console.WriteLine ("Failure: {0}", result.Error);
	break;
}
```

[Full example source code](https://github.com/stormy-ua/Functional/blob/master/RailwayBuddy/Program.cs)
