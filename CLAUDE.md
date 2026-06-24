# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## About

Machina is a .NET state machine framework published as a NuGet package under the `DustInTheWind.Machina` namespace (organization: Dust in the Wind).

## Commands

```bash
# Build
dotnet build Machina.slnx

# Build (release)
dotnet build Machina.slnx -c Release

# Restore dependencies
dotnet restore Machina.slnx

# Run tests (when test projects exist)
dotnet test Machina.slnx

# Run a single test
dotnet test --filter "FullyQualifiedName~TestClassName"

# Pack NuGet package
dotnet pack Machina/Machina.csproj -c Release -o ./artifacts
```

## Architecture

The library exposes two types:

- **`IState<TState, TContext>`** — implement this for each state. `Id` returns the enum value identifying the state; `ExecuteAsync(context)` performs the state's work and returns the next state's enum value, or `null` to stop the machine.
- **`StateMachine<TState, TContext>`** — orchestrates execution. States are registered via `AddState()`. The first state added becomes the initial state unless `InitialState` is set explicitly. Call `ExecuteAllAsync(context)` to run to completion, or `Start()` + `MoveNextAsync()` for manual stepping.

`TState` must be a `struct, Enum`. `TContext` carries shared data passed through every state without restriction.

## C# Code Conventions

- Do not use `var`; use the explicit type.
- Use `new()` (target-typed) instead of `new TypeName()`.
- Name Linq lambda parameters `x`.
- Omit curly braces for single-line `if`, `for`, and `using` bodies.
- No underscores in field names.
- When using object initializer syntax with more than one property, put each property on its own line.
- XML doc comments only on public types that are part of the NuGet API; omit them for internal types.

## Test Conventions

- One test file per public method (including constructors): e.g., `QueryTests.cs` for `Query()`.
- All test files for a class go in a directory named after the class: e.g., `StateMachineTests/`.
- Test method naming: `Having<setup>_When<action>_Then<expectation>`.
- Use a block body (not expression body) in `Assert.Throws` lambdas.

## Releasing

NuGet packages are published automatically when a tag matching `vMAJOR.MINOR.PATCH` is pushed. The version flows from the tag; do not update `<Version>` in `Directory.Build.props` manually for releases. Version defaults to `0.0.0.0` in local builds.
