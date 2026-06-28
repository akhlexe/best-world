using Microsoft.Xna.Framework;

namespace BestWorld.Client.World;

public sealed record NpcDefinition(
    string Name,
    Rectangle Bounds,
    string DialogueText);
