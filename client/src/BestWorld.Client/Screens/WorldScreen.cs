using BestWorld.Client.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BestWorld.Client.Screens;

public sealed class WorldScreen
{
    private readonly Rectangle _worldBounds = new(120, 80, 1040, 560);
    private readonly Rectangle _playerBounds = new(600, 320, 48, 64);
    private readonly Color _worldColor = new(51, 70, 57);
    private readonly Color _worldBorderColor = new(95, 126, 103);
    private readonly Color _playerColor = new(110, 191, 120);

    private Vector2 _playerPosition = new(600f, 320f);
    private float _playerSpeed = 240f;

    public void Update(GameTime gameTime, InputState input)
    {
        var movement = input.GetMovement();
        var elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _playerPosition += movement * _playerSpeed * elapsedSeconds;
        ClampPlayerToWorld();
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixelTexture)
    {
        spriteBatch.Draw(pixelTexture, _worldBounds, _worldColor);
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
        var minX = _worldBounds.Left;
        var maxX = _worldBounds.Right - _playerBounds.Width;
        var minY = _worldBounds.Top;
        var maxY = _worldBounds.Bottom - _playerBounds.Height;

        _playerPosition.X = MathHelper.Clamp(_playerPosition.X, minX, maxX);
        _playerPosition.Y = MathHelper.Clamp(_playerPosition.Y, minY, maxY);
    }

    private void DrawWorldBorder(SpriteBatch spriteBatch, Texture2D pixelTexture)
    {
        const int borderThickness = 4;

        spriteBatch.Draw(pixelTexture, new Rectangle(_worldBounds.Left, _worldBounds.Top, _worldBounds.Width, borderThickness), _worldBorderColor);
        spriteBatch.Draw(pixelTexture, new Rectangle(_worldBounds.Left, _worldBounds.Bottom - borderThickness, _worldBounds.Width, borderThickness), _worldBorderColor);
        spriteBatch.Draw(pixelTexture, new Rectangle(_worldBounds.Left, _worldBounds.Top, borderThickness, _worldBounds.Height), _worldBorderColor);
        spriteBatch.Draw(pixelTexture, new Rectangle(_worldBounds.Right - borderThickness, _worldBounds.Top, borderThickness, _worldBounds.Height), _worldBorderColor);
    }
}
