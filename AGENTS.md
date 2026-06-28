# AGENTS.md

## Scope

- The only app in this repo is `client/`.
- Work from `client/` for build and run commands.

## Current Prototype Rules

- This is a local-only MonoGame prototype. Do not add real networking, backend services, or production persistence unless the user explicitly asks for it.
- Prefer mocked local data and code-first iteration. The current goal is validating movement, combat feel, world presentation, and progression feel.
- Optimize for fast iteration on Linux, not MMO-scale architecture yet.

## Development Principles

- Build new features as small vertical slices. Each slice should add one player-visible capability, keep the game runnable, and be testable locally.
- Do not batch multiple new mechanics into one change. Example: bounds, collisions, and map transitions should land as separate slices unless the user asks otherwise.
- Internal refactors are allowed when they do not change expected behavior and help the next slice stay simple.
- Apply KISS aggressively: prefer the smallest correct MonoGame implementation over framework-building.
- Apply YAGNI aggressively: do not add loaders, abstractions, services, or extension points before a slice actually needs them.
- Apply SOLID pragmatically for the prototype: separate responsibilities when a class is clearly doing two jobs, but do not over-split tiny prototype code just to satisfy the acronym.

## Verified Commands

- Restore: `dotnet restore BestWorld.Client.sln`
- Build: `dotnet build BestWorld.Client.sln`
- Run: `dotnet run --project src/BestWorld.Client/BestWorld.Client.csproj`

## Verification

- There are currently no test projects or CI workflows in the repo.
- For normal code changes, verify with `dotnet build BestWorld.Client.sln` from `client/`.

## Code Layout

- `client/src/BestWorld.Client/Program.cs` is the process entrypoint.
- `client/src/BestWorld.Client/GameClient.cs` owns the MonoGame loop, window setup, and shared rendering primitives.
- `client/src/BestWorld.Client/Input/InputState.cs` converts raw keyboard state into gameplay intent.
- `client/src/BestWorld.Client/Screens/WorldScreen.cs` is the current runtime world slice: player movement, world bounds, and rendering.
- `client/src/BestWorld.Client/World/MapDefinition.cs` is the current mocked world data model.
- `client/ARCHITECTURE.md` is the product and prototype direction reference. Follow the prototype sections, not the future MMO backend sections, unless the user explicitly switches focus.

## MonoGame-Specific Notes

- Current rendering is intentionally primitive-based: `GameClient.LoadContent()` creates a 1x1 white `Texture2D`, and `WorldScreen` stretches it to draw the world and player rectangles.
- Prefer extending this primitive rendering approach for early slices instead of introducing art assets or content-pipeline complexity too early.
- Keep gameplay rules in `Update` and rendering in `Draw`. The current code already follows that split.

## Change Guidance

- When adding a new slice, preserve the current playable path: launch window -> move player -> exit with `Escape`.
- Prefer changes in the existing flow before adding new subsystems: `GameClient` wires dependencies, `InputState` produces input intent, `WorldScreen` applies local simulation, and world data lives in `World/`.
