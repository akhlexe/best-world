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
    private SpriteFont? _debugFont;

    public GameClient()
    {
        _graphics = new GraphicsDeviceManager(this);
        _input = new InputState();
        _worldScreen = new WorldScreen(
        [
            new MapDefinition(
                Name: "Prototype Town",
                Bounds: new Rectangle(120, 80, 1040, 560),
                PlayerSpawn: new Vector2(220f, 300f),
                BackgroundColor: new Color(66, 76, 91),
                CollisionRectangles:
                [
                    new Rectangle(340, 180, 220, 100),
                    new Rectangle(700, 380, 160, 120)
                ],
                TransitionTriggers:
                [
                    new ZoneTransition(
                        TriggerBounds: new Rectangle(1080, 260, 80, 140),
                        TargetMapName: "Prototype Field",
                        TargetSpawn: new Vector2(240f, 300f))
                ],
                 Npcs:
                 [
                     new NpcDefinition(
                         Name: "Town Guide",
                         Bounds: new Rectangle(620, 240, 48, 64),
                         DialogueText: "Welcome to Best World. The east gate leads out to the prototype field."),
                     new NpcDefinition(
                         Name: "Apprentice Mira",
                         Bounds: new Rectangle(860, 220, 48, 64),
                         DialogueText: "I am practicing greetings. If you can read this, the second NPC works.")
                 ]),
            new MapDefinition(
                Name: "Prototype Field",
                Bounds: new Rectangle(120, 80, 1040, 560),
                PlayerSpawn: new Vector2(160f, 300f),
                BackgroundColor: new Color(51, 70, 57),
                CollisionRectangles:
                [
                    new Rectangle(280, 180, 180, 80),
                    new Rectangle(760, 420, 220, 100)
                ],
                TransitionTriggers:
                [
                    new ZoneTransition(
                        TriggerBounds: new Rectangle(120, 260, 80, 140),
                        TargetMapName: "Prototype Town",
                        TargetSpawn: new Vector2(1000f, 300f))
                ],
                Npcs:
                [
                    new NpcDefinition(
                        Name: "Field Scout",
                        Bounds: new Rectangle(640, 260, 48, 64),
                        DialogueText: "The field is quiet for now. Come back later when there is something worth fighting.")
                ])
        ],
        "Prototype Town");

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
        _debugFont = Content.Load<SpriteFont>("Fonts/DebugFont");
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
        _worldScreen.Draw(_spriteBatch, _pixelTexture!, _debugFont!);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
