using BestWorld.Client.Input;
using BestWorld.Client.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BestWorld.Client.Screens;

public sealed class WorldScreen
{
    private readonly Rectangle _playerBounds = new(600, 320, 48, 64);
    private readonly Color _worldColor = new(51, 70, 57);
    private readonly Color _worldBorderColor = new(95, 126, 103);
    private readonly Color _playerColor = new(110, 191, 120);
    private readonly MapDefinition _map;

    private Vector2 _playerPosition;
    private float _playerSpeed = 240f;

    public WorldScreen(MapDefinition map)
    {
        _map = map;
        _playerPosition = map.PlayerSpawn;
    }

    public void Update(GameTime gameTime, InputState input)
    {
        var movement = input.GetMovement();
        var elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _playerPosition += movement * _playerSpeed * elapsedSeconds;
        ClampPlayerToWorld();
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixelTexture)
    {
        spriteBatch.Draw(pixelTexture, _map.Bounds, _worldColor);
        DrawWorldBorder(spriteBatch, pixelTexture);

        var playerRectangle = new Rectangle(
            (int)_playerPosition.X,
            (int)_playerPosition.Y,
            _playerBounds.Width,
            _playerBounds.Height);

        spriteBatch.Draw(pixelTexture, playerRectangle, _playerColor);
    }

    private void ClampPlayerToWorld()
    {
        var minX = _map.Bounds.Left;
        var maxX = _map.Bounds.Right - _playerBounds.Width;
        var minY = _map.Bounds.Top;
        var maxY = _map.Bounds.Bottom - _playerBounds.Height;

        _playerPosition.X = MathHelper.Clamp(_playerPosition.X, minX, maxX);
        _playerPosition.Y = MathHelper.Clamp(_playerPosition.Y, minY, maxY);
    }

    private void DrawWorldBorder(SpriteBatch spriteBatch, Texture2D pixelTexture)
    {
        const int borderThickness = 4;

        spriteBatch.Draw(pixelTexture, new Rectangle(_map.Bounds.Left, _map.Bounds.Top, _map.Bounds.Width, borderThickness), _worldBorderColor);
        spriteBatch.Draw(pixelTexture, new Rectangle(_map.Bounds.Left, _map.Bounds.Bottom - borderThickness, _map.Bounds.Width, borderThickness), _worldBorderColor);
        spriteBatch.Draw(pixelTexture, new Rectangle(_map.Bounds.Left, _map.Bounds.Top, borderThickness, _map.Bounds.Height), _worldBorderColor);
        spriteBatch.Draw(pixelTexture, new Rectangle(_map.Bounds.Right - borderThickness, _map.Bounds.Top, borderThickness, _map.Bounds.Height), _worldBorderColor);
    }
}
