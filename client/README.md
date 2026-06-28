# Best World Client

Minimal MonoGame prototype client for the Best World MMORPG project.

## Current Scope

This client is intentionally local-only and connectionless.

Its current job is to provide a fast code-first prototype foundation for:

- movement feel
- combat feel
- world presentation
- UI and progression experiments

## Stack

- .NET 10
- C#
- MonoGame DesktopGL

## Run

```bash
dotnet restore BestWorld.Client.sln
dotnet build BestWorld.Client.sln
dotnet run --project src/BestWorld.Client/BestWorld.Client.csproj
```

## Current Behavior

- opens a desktop game window
- sets the window title to `Best World Prototype`
- shows the mouse cursor
- renders a visible playable area inside the window
- lets you move the player rectangle with `WASD` or arrow keys
- keeps the player inside the playable area bounds
- exits when you press `Escape`

## Project Layout

- `src/BestWorld.Client/Program.cs` - application entry point
- `src/BestWorld.Client/GameClient.cs` - main MonoGame game loop
- `src/BestWorld.Client/Content/Content.mgcb` - content pipeline manifest
