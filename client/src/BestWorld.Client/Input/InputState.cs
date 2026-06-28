using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BestWorld.Client.Input;

public sealed class InputState
{
    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;

    public void Update()
    {
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();
    }

    public bool IsExitPressed()
    {
        return WasKeyPressed(Keys.Escape);
    }

    public bool IsInteractPressed()
    {
        return WasKeyPressed(Keys.E);
    }

    public bool IsMoveUpDown()
    {
        return IsKeyDown(Keys.W) || IsKeyDown(Keys.Up);
    }

    public bool IsMoveDownDown()
    {
        return IsKeyDown(Keys.S) || IsKeyDown(Keys.Down);
    }

    public bool IsMoveLeftDown()
    {
        return IsKeyDown(Keys.A) || IsKeyDown(Keys.Left);
    }

    public bool IsMoveRightDown()
    {
        return IsKeyDown(Keys.D) || IsKeyDown(Keys.Right);
    }

    public bool WasKeyPressed(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
    }

    public Vector2 GetMovement()
    {
        var movement = Vector2.Zero;

        if (IsMoveLeftDown())
        {
            movement.X -= 1f;
        }

        if (IsMoveRightDown())
        {
            movement.X += 1f;
        }

        if (IsMoveUpDown())
        {
            movement.Y -= 1f;
        }

        if (IsMoveDownDown())
        {
            movement.Y += 1f;
        }

        if (movement != Vector2.Zero)
        {
            movement.Normalize();
        }

        return movement;
    }

    private bool IsKeyDown(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key);
    }
}
