using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BestWorld.Client;

public sealed class GameClient : Game
{
    private readonly GraphicsDeviceManager _graphics;

    public GameClient()
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Window.Title = "Best World Prototype";
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(22, 28, 40));

        base.Draw(gameTime);
    }
}
