using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BestWorld.Client.Input;
using BestWorld.Client.Screens;
using BestWorld.Client.World;

namespace BestWorld.Client;

public sealed class GameClient : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly InputState _input;
    private readonly WorldScreen _worldScreen;

    private SpriteBatch? _spriteBatch;
    private Texture2D? _pixelTexture;

    public GameClient()
    {
        _graphics = new GraphicsDeviceManager(this);
        _input = new InputState();
        _worldScreen = new WorldScreen(new MapDefinition(
            Name: "Prototype Field",
            Bounds: new Rectangle(120, 80, 1040, 560),
            PlayerSpawn: new Vector2(600f, 320f),
            CollisionRectangles:
            [
                new Rectangle(280, 180, 180, 80),
                new Rectangle(760, 420, 220, 100)
            ]));

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
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTexture.SetData([Color.White]);
    }

    protected override void Update(GameTime gameTime)
    {
        _input.Update();

        if (_input.IsExitPressed())
        {
            Exit();
        }

        _worldScreen.Update(gameTime, _input);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(22, 28, 40));

        _spriteBatch!.Begin();
        _worldScreen.Draw(_spriteBatch, _pixelTexture!);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
