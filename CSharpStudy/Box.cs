class Box
{
    public int positionX { get; private set; }
    public int positionY { get; private set; }
    public bool isOnGoal = false;

    public string Icon = "◎";
    public string boxOnIcon = "●";

    public Box(int x, int y)
    {
        positionX = x;
        positionY = y;
        isOnGoal = false;
    }

    public void Move(Direction dir)
    {
        if (dir == Direction.Up)
        {
            positionY = positionY - 1;
        }
        else if (dir == Direction.Down)
        {
            positionY = positionY + 1;
        }
        else if (dir == Direction.Right)
        {
            positionX = positionX + 1;
        }
        else if (dir == Direction.Left)
        {
            positionX = positionX - 1;
        }

        if (positionX < 0)
        {
            positionX = 0;
        }
        if (positionY < 0)
        {
            positionY = 0;
        }
    }

    public bool isCollide(int x, int y)
    {
        if (x == positionX && y == positionY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}