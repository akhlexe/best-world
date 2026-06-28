using Microsoft.Xna.Framework;

namespace BestWorld.Client.World;

public sealed record ZoneTransition(
    Rectangle TriggerBounds,
    string TargetMapName,
    Vector2 TargetSpawn);
