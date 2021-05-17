namespace Simulator.Engine.Types
{
    public enum RotationMode
    {
        None, //Almost free.
        Clip, //Expensive...
        Upsize //Hella expensive!
    }
    public enum KeyPressState
    {
        Up,
        Down
    }
    public enum PlayerKey
    {
        SpeedBoost,
        Forward,
        Reverse,
        Fire,
        RotateClockwise,
        RotateCounterClockwise,
        Escape,
        Left,
        Right,
        Up,
        Down,
        Enter
    }
}
