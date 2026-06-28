using Microsoft.Xna.Framework;

namespace BestWorld.Client.World;

public sealed record MapDefinition(
    string Name,
    Rectangle Bounds,
    Vector2 PlayerSpawn);
