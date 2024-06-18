class Program
{
    enum Direction
    {
        Right, Left, Up, Down
    }

    static void Main(string[] args)
    {
        int playerPositionX = 1;
        int playerPositionY = 1;
        int playerMoveDistance = 1;
        bool isPlayerGhost;
        Direction playerMoveDir;

        int[] boxPositionX = { 3, 5, 2, 4, 8 };
        int[] boxPositionY = { 4, 6, 2, 10, 8 };
        int interactedBoxIndex;

        int[] blockPositionX = { 4, 5, 6, 9, 9 };
        int[] blockPositionY = { 4, 4, 5, 7, 8 };

        int[] goalPositionX = { 10, 10, 10, 10, 10 };
        int[] goalPositionY = { 10, 11, 12, 13, 14 };

        int itemPositionX = 15;
        int itemPositionY = 4;

        InitGameState();

        while (true)
        {
            Render();

            ConsoleKey key = Input();
            
            MovePlayer(key);

            if (CanPlayerMove() == false)
            {
                OnObjectBlocked(() => PushOut(ref playerPositionX, ref playerPositionY));
                continue;
            }

            interactedBoxIndex = GetIndexOfInteractedBox(playerPositionX, playerPositionY);
            if (interactedBoxIndex >= 0)
            {
                MoveBox();

                if (CanBoxMove() == false)
                {
                    OnObjectBlocked(() =>
                    {
                        PushOut(ref boxPositionX[interactedBoxIndex], ref boxPositionY[interactedBoxIndex]);
                        PushOut(ref playerPositionX, ref playerPositionY);
                    });
                    continue;
                }
            }

            CheckPlayerGetItem();

            if (IsAllBoxesOnTheGoals())
            {
                break;
            }
        }

        void OnObjectBlocked(Action action)
        {
            action.Invoke();
        }

        void PushOut(ref int x, ref int y)
        {
            Direction pushBackDirection;

            switch (playerMoveDir)
            {
                case Direction.Left:
                    pushBackDirection = Direction.Right;
                    break;
                case Direction.Right:
                    pushBackDirection = Direction.Left;
                    break;
                case Direction.Up:
                    pushBackDirection = Direction.Down;
                    break;
                case Direction.Down:
                    pushBackDirection = Direction.Up;
                    break;
                default:
                    pushBackDirection = Direction.Right;
                    break;
            }

            MoveObject(ref x, ref y, pushBackDirection, playerMoveDistance);
        }

        void InitGameState()
        {
            Console.ResetColor();
            Console.CursorVisible = false;
            Console.Title = "Sokoban";
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Clear();

            isPlayerGhost = false;
        }

        void Render()
        {
            string playerIcon = "▼";
            string blockIcon = "□";
            string playerInBoxIcon = "▣";
            string itemIcon = "♨";
            string goalIcon = "☆";
            string boxOnGoalIcon = "★";
            string boxIcon = "◇";

            Console.Clear();

            DrawObject(playerPositionX, playerPositionY, playerIcon);

            for (int i = 0; i < blockPositionX.Length; i++)
            {
                if (playerPositionX == blockPositionX[i] && playerPositionY == blockPositionY[i])
                {
                    DrawObject(blockPositionX[i], blockPositionY[i], playerInBoxIcon);
                }
                else
                {
                    DrawObject(blockPositionX[i], blockPositionY[i], blockIcon);
                }
            }

            if (!isPlayerGhost)
            {
                DrawObject(itemPositionX, itemPositionY, itemIcon);
            }


            for (int i = 0; i < goalPositionX.Length; i++)
            {
                DrawObject(goalPositionX[i], goalPositionY[i], goalIcon);
            }


            for (int i = 0; i < boxPositionX.Length; i++)
            {
                for (int j = 0; j < goalPositionX.Length; j++)
                {
                    if (isObjectsCollide(boxPositionX[i], boxPositionY[i], goalPositionX[j], goalPositionY[j]))
                    {
                        DrawObject(boxPositionX[i], boxPositionY[i], boxOnGoalIcon);
                        break;
                    }
                    else
                    {
                        DrawObject(boxPositionX[i], boxPositionY[i], boxIcon);
                    }
                }

            }
            void DrawObject(int x, int y, string icon)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(icon);
            }
        }

        ConsoleKey Input()
        {
            ConsoleKeyInfo inputKeyInfo = Console.ReadKey();
            ConsoleKey key = inputKeyInfo.Key;
            return key;
        }


        bool IsObjectOutOfBound(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool isObjectsCollide(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2 && y1 == y2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void MoveObject(ref int x, ref int y, Direction dir, int distance)
        {
            if (dir == Direction.Up)
            {
                y = y - distance;
            }
            else if (dir == Direction.Down)
            {
                y = y + distance;
            }
            else if (dir == Direction.Left)
            {
                x = x - distance;
            }
            else if (dir == Direction.Right)
            {
                x = x + distance;
            }
        }

        void MovePlayer(ConsoleKey key)
        {
            if (key == ConsoleKey.RightArrow)
            {
                playerMoveDir = Direction.Right;
            }
            else if (key == ConsoleKey.LeftArrow)
            {
                playerMoveDir = Direction.Left;
            }
            else if (key == ConsoleKey.UpArrow)
            {
                playerMoveDir = Direction.Up;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                playerMoveDir = Direction.Down;
            }
            else
            {
                playerMoveDir = Direction.Down;
            }

            MoveObject(ref playerPositionX, ref playerPositionY, playerMoveDir, playerMoveDistance);
        }

        bool CanPlayerMove()
        {
            if (IsObjectOutOfBound(playerPositionX, playerPositionY))
            {
                return false;
            }

            if (!isPlayerGhost)
            {
                for (int i = 0; i < blockPositionX.Length; i++)
                {
                    if (isObjectsCollide(playerPositionX, playerPositionY, blockPositionX[i], blockPositionY[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        int GetIndexOfInteractedBox(int interectObjectX, int interectObjectY)
        {
            for (int i = 0; i < boxPositionX.Length; i++)
            {
                if (isObjectsCollide(interectObjectX, interectObjectY, boxPositionX[i], boxPositionY[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        void MoveBox()
        {
            MoveObject(ref boxPositionX[interactedBoxIndex], ref boxPositionY[interactedBoxIndex], playerMoveDir, playerMoveDistance);
        }

        bool CanBoxMove()
        {
            if (IsObjectOutOfBound(boxPositionX[interactedBoxIndex], boxPositionY[interactedBoxIndex]))
            {
                return false;
            }

            for (int i = 0; i < blockPositionX.Length; i++)
            {
                if (isObjectsCollide(boxPositionX[interactedBoxIndex], boxPositionY[interactedBoxIndex], blockPositionX[i], blockPositionY[i]))
                {
                    return false;
                }
            }

            for (int i = 0; i < boxPositionX.Length; i++)
            {
                if (interactedBoxIndex == i) continue;

                if (isObjectsCollide(boxPositionX[interactedBoxIndex], boxPositionY[interactedBoxIndex], boxPositionX[i], boxPositionY[i]))
                {
                    return false;
                }
            }
            return true;
        }

        void CheckPlayerGetItem()
        {
            if (!isPlayerGhost)
            {
                if (isObjectsCollide(playerPositionX, playerPositionY, itemPositionX, itemPositionY))
                {
                    isPlayerGhost = true;
                }
            }
        }

        bool IsAllBoxesOnTheGoals()
        {
            int countOfBoxOnTheGoal = 0;

            for (int i = 0; i < goalPositionX.Length; i++)
            {
                for (int j = 0; j < boxPositionX.Length; j++)
                {
                    if (isObjectsCollide(goalPositionX[i], goalPositionY[i], boxPositionX[j], boxPositionY[j]))
                    {
                        countOfBoxOnTheGoal++;
                    }
                }
            }
            if (countOfBoxOnTheGoal == goalPositionX.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}