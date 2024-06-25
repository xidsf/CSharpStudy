class Player
{
    public int positionX { get; private set; }
    public int positionY { get; private set; }
    public bool isGhost { get; private set; }
    public Direction playerMoveDir { get; private set; }
    public string playerIcon;

    public Player(int x = 1, int y = 1)
    {
        positionX = x;
        positionY = y;
        isGhost = false;
        playerMoveDir = Direction.Right;
        playerIcon = "▼";
    }

    public void Move(ConsoleKey key)
    {
        if (key == ConsoleKey.UpArrow)
        {
            playerMoveDir = Direction.Up;
            positionY = positionY - 1;
        }
        else if (key == ConsoleKey.DownArrow)
        {
            playerMoveDir = Direction.Down;
            positionY = positionY + 1;
        }
        else if (key == ConsoleKey.LeftArrow)
        {
            playerMoveDir = Direction.Left;
            positionX = positionX - 1;
        }
        else if (key == ConsoleKey.RightArrow)
        {
            playerMoveDir = Direction.Right;
            positionX = positionX + 1;
        }
        CheckOutOfBound();
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
        CheckOutOfBound();
    }

    private void CheckOutOfBound()
    {
        if (positionX < 0)
        {
            positionX = 0;
        }
        if (positionY < 0)
        {
            positionY = 0;
        }
    }


    public void PushBox(Box box)
    {
        box.Move(playerMoveDir);
    }

    public bool isCollide(int x, int y)
    {
        if(x == positionX && y == positionY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isCollide(Box box)
    {
        if (box.positionX == positionX && box.positionY == positionY)
        {
            box.Move(playerMoveDir);
            return true;
        }
        else
        {
            return false;
        }
    }

}