class ObjectManager
{
    public static Direction GetReverseDir(Direction dir)
    {
        Direction pushOutDirection;

        switch (dir)
        {
            case Direction.Left:
                pushOutDirection = Direction.Right;
                break;
            case Direction.Right:
                pushOutDirection = Direction.Left;
                break;
            case Direction.Up:
                pushOutDirection = Direction.Down;
                break;
            case Direction.Down:
                pushOutDirection = Direction.Up;
                break;
            default:
                pushOutDirection = Direction.Right;
                break;
        }
        return pushOutDirection;
    }
}