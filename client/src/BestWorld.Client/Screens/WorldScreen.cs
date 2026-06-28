using BestWorld.Client.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BestWorld.Client.Screens;

public sealed class WorldScreen
{
    private readonly Rectangle _playerBounds = new(600, 320, 48, 64);
    private readonly Color _playerColor = new(110, 191, 120);

    private Vector2 _playerPosition = new(600f, 320f);
    private float _playerSpeed = 240f;

    public void Update(GameTime gameTime, InputState input)
    {
        var movement = input.GetMovement();
        var elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _playerPosition += movement * _playerSpeed * elapsedSeconds;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixelTexture)
    {
        var playerRectangle = new Rectangle(
            (int)_playerPosition.X,
            (int)_playerPosition.Y,
            _playerBounds.Width,
            _playerBounds.Height);

        spriteBatch.Draw(pixelTexture, playerRectangle, _playerColor);
    }
}
