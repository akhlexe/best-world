using BestWorld.Client.Input;
using BestWorld.Client.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BestWorld.Client.Screens;

public sealed class WorldScreen
{
    private const int InteractionRange = 28;
    private const string GuideNpcName = "Town Guide";
    private const string ObjectiveHintText = "Objective: Talk to the field scout";

    private readonly Rectangle _playerBounds = new(600, 320, 48, 64);
    private readonly Color _worldBorderColor = new(95, 126, 103);
    private readonly Color _collisionColor = new(112, 87, 61);
    private readonly Color _transitionColor = new(83, 121, 173);
    private readonly Color _npcColor = new(189, 143, 92);
    private readonly Color _playerColor = new(110, 191, 120);
    private readonly Dictionary<string, MapDefinition> _maps;

    private MapDefinition _currentMap;
    private NpcDefinition? _activeDialogueNpc;
    private bool _hasSpokenToGuide;
    private Vector2 _playerPosition;
    private float _playerSpeed = 240f;

    public WorldScreen(IReadOnlyList<MapDefinition> maps, string initialMapName)
    {
        _maps = new Dictionary<string, MapDefinition>();

        foreach (var map in maps)
        {
            _maps.Add(map.Name, map);
        }

        _currentMap = _maps[initialMapName];
        _playerPosition = _currentMap.PlayerSpawn;
    }

    public void Update(GameTime gameTime, InputState input)
    {
        if (_activeDialogueNpc is not null)
        {
            if (input.IsInteractPressed())
            {
                if (_activeDialogueNpc.Name == GuideNpcName)
                {
                    _hasSpokenToGuide = true;
                }

                _activeDialogueNpc = null;
            }

            return;
        }

        var movement = input.GetMovement();
        var elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var nextPosition = _playerPosition + movement * _playerSpeed * elapsedSeconds;
        var nextPlayerRectangle = new Rectangle(
            (int)nextPosition.X,
            (int)nextPosition.Y,
            _playerBounds.Width,
            _playerBounds.Height);

        if (!IsCollisionAt(nextPlayerRectangle))
        {
            _playerPosition = nextPosition;
        }

        ClampPlayerToWorld();
        TryHandleZoneTransition();

        if (input.IsInteractPressed())
        {
            _activeDialogueNpc = GetNearbyNpc();
        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixelTexture, SpriteFont font)
    {
        spriteBatch.Draw(pixelTexture, _currentMap.Bounds, _currentMap.BackgroundColor);
        DrawCollisionRectangles(spriteBatch, pixelTexture);
        DrawNpcs(spriteBatch, pixelTexture);
        DrawTransitionTriggers(spriteBatch, pixelTexture);
        DrawWorldBorder(spriteBatch, pixelTexture);

        var playerRectangle = new Rectangle(
            (int)_playerPosition.X,
            (int)_playerPosition.Y,
            _playerBounds.Width,
            _playerBounds.Height);

        spriteBatch.Draw(pixelTexture, playerRectangle, _playerColor);
        DrawHud(spriteBatch, pixelTexture, font);

        if (_activeDialogueNpc is not null)
        {
            DrawDialogueBox(spriteBatch, pixelTexture, font, _activeDialogueNpc);
        }
    }

    private void ClampPlayerToWorld()
    {
        var minX = _currentMap.Bounds.Left;
        var maxX = _currentMap.Bounds.Right - _playerBounds.Width;
        var minY = _currentMap.Bounds.Top;
        var maxY = _currentMap.Bounds.Bottom - _playerBounds.Height;

        _playerPosition.X = MathHelper.Clamp(_playerPosition.X, minX, maxX);
        _playerPosition.Y = MathHelper.Clamp(_playerPosition.Y, minY, maxY);
    }

    private void DrawWorldBorder(SpriteBatch spriteBatch, Texture2D pixelTexture)
    {
        const int borderThickness = 4;

        spriteBatch.Draw(pixelTexture, new Rectangle(_currentMap.Bounds.Left, _currentMap.Bounds.Top, _currentMap.Bounds.Width, borderThickness), _worldBorderColor);
        spriteBatch.Draw(pixelTexture, new Rectangle(_currentMap.Bounds.Left, _currentMap.Bounds.Bottom - borderThickness, _currentMap.Bounds.Width, borderThickness), _worldBorderColor);
        spriteBatch.Draw(pixelTexture, new Rectangle(_currentMap.Bounds.Left, _currentMap.Bounds.Top, borderThickness, _currentMap.Bounds.Height), _worldBorderColor);
        spriteBatch.Draw(pixelTexture, new Rectangle(_currentMap.Bounds.Right - borderThickness, _currentMap.Bounds.Top, borderThickness, _currentMap.Bounds.Height), _worldBorderColor);
    }

    private void DrawCollisionRectangles(SpriteBatch spriteBatch, Texture2D pixelTexture)
    {
        foreach (var collisionRectangle in _currentMap.CollisionRectangles)
        {
            spriteBatch.Draw(pixelTexture, collisionRectangle, _collisionColor);
        }
    }

    private void DrawTransitionTriggers(SpriteBatch spriteBatch, Texture2D pixelTexture)
    {
        foreach (var transitionTrigger in _currentMap.TransitionTriggers)
        {
            spriteBatch.Draw(pixelTexture, transitionTrigger.TriggerBounds, _transitionColor);
        }
    }

    private void DrawNpcs(SpriteBatch spriteBatch, Texture2D pixelTexture)
    {
        foreach (var npc in _currentMap.Npcs)
        {
            spriteBatch.Draw(pixelTexture, npc.Bounds, _npcColor);
        }
    }

    private bool IsCollisionAt(Rectangle playerRectangle)
    {
        foreach (var collisionRectangle in _currentMap.CollisionRectangles)
        {
            if (playerRectangle.Intersects(collisionRectangle))
            {
                return true;
            }
        }

        return false;
    }

    private void TryHandleZoneTransition()
    {
        var playerRectangle = new Rectangle(
            (int)_playerPosition.X,
            (int)_playerPosition.Y,
            _playerBounds.Width,
            _playerBounds.Height);

        foreach (var transitionTrigger in _currentMap.TransitionTriggers)
        {
            if (playerRectangle.Intersects(transitionTrigger.TriggerBounds))
            {
                _currentMap = _maps[transitionTrigger.TargetMapName];
                _playerPosition = transitionTrigger.TargetSpawn;
                return;
            }
        }
    }

    private void DrawHud(SpriteBatch spriteBatch, Texture2D pixelTexture, SpriteFont font)
    {
        const string labelPrefix = "Map: ";
        var mapLabel = labelPrefix + _currentMap.Name;
        var hudPosition = new Vector2(24f, 24f);
        var textSize = font.MeasureString(mapLabel);
        var backgroundRectangle = new Rectangle(
            (int)hudPosition.X - 10,
            (int)hudPosition.Y - 8,
            (int)textSize.X + 20,
            (int)textSize.Y + 16);

        spriteBatch.Draw(pixelTexture, backgroundRectangle, new Color(12, 16, 24, 200));
        spriteBatch.DrawString(font, mapLabel, hudPosition, Color.White);

        if (_hasSpokenToGuide)
        {
            DrawObjectiveHint(spriteBatch, pixelTexture, font);
        }

        var nearbyNpc = GetNearbyNpc();

        if (nearbyNpc is not null && _activeDialogueNpc is null)
        {
            DrawInteractionPrompt(spriteBatch, pixelTexture, font, nearbyNpc);
        }
    }

    private NpcDefinition? GetNearbyNpc()
    {
        var interactionBounds = new Rectangle(
            (int)_playerPosition.X - InteractionRange,
            (int)_playerPosition.Y - InteractionRange,
            _playerBounds.Width + (InteractionRange * 2),
            _playerBounds.Height + (InteractionRange * 2));

        foreach (var npc in _currentMap.Npcs)
        {
            if (interactionBounds.Intersects(npc.Bounds))
            {
                return npc;
            }
        }

        return null;
    }

    private void DrawInteractionPrompt(SpriteBatch spriteBatch, Texture2D pixelTexture, SpriteFont font, NpcDefinition nearbyNpc)
    {
        var prompt = $"Press E to talk to {nearbyNpc.Name}";
        var textSize = font.MeasureString(prompt);
        var promptPosition = new Vector2(24f, _hasSpokenToGuide ? 92f : 56f);
        var backgroundRectangle = new Rectangle(
            (int)promptPosition.X - 10,
            (int)promptPosition.Y - 8,
            (int)textSize.X + 20,
            (int)textSize.Y + 16);

        spriteBatch.Draw(pixelTexture, backgroundRectangle, new Color(12, 16, 24, 200));
        spriteBatch.DrawString(font, prompt, promptPosition, new Color(255, 230, 170));
    }

    private void DrawObjectiveHint(SpriteBatch spriteBatch, Texture2D pixelTexture, SpriteFont font)
    {
        var textSize = font.MeasureString(ObjectiveHintText);
        var hintPosition = new Vector2(24f, 56f);
        var backgroundRectangle = new Rectangle(
            (int)hintPosition.X - 10,
            (int)hintPosition.Y - 8,
            (int)textSize.X + 20,
            (int)textSize.Y + 16);

        spriteBatch.Draw(pixelTexture, backgroundRectangle, new Color(12, 16, 24, 200));
        spriteBatch.DrawString(font, ObjectiveHintText, hintPosition, new Color(190, 220, 140));
    }

    private void DrawDialogueBox(SpriteBatch spriteBatch, Texture2D pixelTexture, SpriteFont font, NpcDefinition activeDialogueNpc)
    {
        var dialogueBounds = new Rectangle(140, 520, 1000, 140);
        var namePosition = new Vector2(dialogueBounds.X + 24, dialogueBounds.Y + 20);
        var textPosition = new Vector2(dialogueBounds.X + 24, dialogueBounds.Y + 56);
        var closeHint = "Press E to close";
        var closeHintSize = font.MeasureString(closeHint);
        var closeHintPosition = new Vector2(
            dialogueBounds.Right - closeHintSize.X - 24,
            dialogueBounds.Bottom - closeHintSize.Y - 18);

        spriteBatch.Draw(pixelTexture, dialogueBounds, new Color(10, 14, 22, 230));
        spriteBatch.Draw(pixelTexture, new Rectangle(dialogueBounds.X, dialogueBounds.Y, dialogueBounds.Width, 4), new Color(122, 170, 220));
        spriteBatch.DrawString(font, activeDialogueNpc.Name, namePosition, new Color(255, 230, 170));
        spriteBatch.DrawString(font, activeDialogueNpc.DialogueText, textPosition, Color.White);
        spriteBatch.DrawString(font, closeHint, closeHintPosition, new Color(190, 200, 220));
    }
}
