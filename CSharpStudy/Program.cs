using System;

class Program
{
    class Unit
    {
        public int positionX;
        public int positionY;
    }

    static void Main(string[] args)
    {
        Player player = new Player();

        Box[] box = new Box[5];
        box[0] = new Box { positionX = 3, positionY = 5 };
        box[1] = new Box { positionX = 5, positionY = 6 };
        box[2] = new Box { positionX = 2, positionY = 2 };
        box[3] = new Box { positionX = 4, positionY = 10 };
        box[4] = new Box { positionX = 8, positionY = 8 };
        int interactedBoxIndex;

        Block[] block = new Block[5];
        block[0] = new Block { positionX = 4, positionY = 4 };
        block[1] = new Block { positionX = 5, positionY = 4 };
        block[2] = new Block { positionX = 6, positionY = 5 };
        block[3] = new Block { positionX = 9, positionY = 7 };
        block[4] = new Block { positionX = 9, positionY = 8 };

        HiddenBlock[] hiddenBlock = new HiddenBlock[10];
        for (int i = 0; i < 5; i++)
        {
            hiddenBlock[i] = new HiddenBlock { positionX = 9, positionY = 10 + i, isHidden = true };
        }
        for (int i = 5; i < 10; i++)
        {
            hiddenBlock[i] = new HiddenBlock { positionX = 11, positionY = 10 + i - 5, isHidden = true };
        }
        int appearedHiddenBlockCount = 0;

        Goal[] goal = new Goal[5];
        for (int i = 0; i < goal.Length; i++)
        {
            goal[i] = new Goal { positionX = 10, positionY = 10 + i };
        }

        Item item = new Item { positionX = 15, positionY = 4 };


        InitGameState();

        while (true)
        {
            Render();

            ConsoleKey key = Input();
            
            SetPlayerMoveDir(key);

            if(TryMovePlayer() == false || TryMoveBox() == false)
            {
                continue;
            }

            CheckPlayerGetItem();

            if (IsAllBoxesOnTheGoals())
            {
                break;
            }
        }

        void InitGameState()
        {
            Console.ResetColor();
            Console.CursorVisible = false;
            Console.Title = "Sokoban";
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Clear();

            player.isGhost = false;
        }

        void Render()
        {
            string playerIcon = "▼";
            string blockIcon = "□";
            string playerInBoxIcon = "▣";
            string itemIcon = "♨";
            string goalIcon = "*";
            string boxOnGoalIcon = "#";
            string boxIcon = "◇";

            Console.Clear();

            DrawObject(player.positionX, player.positionY, playerIcon);

            for (int i = 0; i < block.Length; i++)
            {
                if (block[i].isInPlayer)
                {
                    DrawObject(block[i].positionX, block[i].positionY, playerInBoxIcon);
                }
                else
                {
                    DrawObject(block[i].positionX, block[i].positionY, blockIcon);
                }
            }

            for (int i = 0; i < hiddenBlock.Length; i++)
            {
                if (hiddenBlock[i].isHidden)
                {
                    continue;
                }

                if (hiddenBlock[i].isInPlayer)
                {
                    DrawObject(hiddenBlock[i].positionX, hiddenBlock[i].positionY, playerInBoxIcon);
                }
                else
                {
                    DrawObject(hiddenBlock[i].positionX, hiddenBlock[i].positionY, blockIcon);
                }
            }

            if (!player.isGhost)
            {
                DrawObject(item.positionX, item.positionY, itemIcon);
            }

            for (int i = 0; i < goal.Length; i++)
            {
                DrawObject(goal[i].positionX, goal[i].positionY, goalIcon);
            }

            for (int i = 0; i < box.Length; i++)
            {
                for (int j = 0; j < goal.Length; j++)
                {
                    if (box[i].isOnGoal)
                    {
                        DrawObject(box[i].positionX, box[i].positionY, boxOnGoalIcon);
                        break;
                    }
                    else
                    {
                        DrawObject(box[i].positionX, box[i].positionY, boxIcon);
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

        bool TryMovePlayer()
        {
            MoveObject(ref player.positionX, ref player.positionY, player.playerMoveDir);
            if (CheckPlayerMove() == false)
            {
                OnObjectBlocked(() => PushOut(ref player.positionX, ref player.positionY));
                return false;
            }

            for (int i = 0; i < block.Length; i++)
            {
                if (isObjectsCollide(player.positionX, player.positionY, block[i].positionX, block[i].positionY))
                {
                    block[i].isInPlayer = true;
                }
                else
                {
                    block[i].isInPlayer = false;
                }
            }

            for (int i = 0; i < hiddenBlock.Length; i++)
            {
                if (isObjectsCollide(player.positionX, player.positionY, hiddenBlock[i].positionX, hiddenBlock[i].positionY))
                {
                    hiddenBlock[i].isInPlayer = true;
                }
                else
                {
                    hiddenBlock[i].isInPlayer = false;
                }
            }

            return true;
        }

        bool TryMoveBox()
        {
            interactedBoxIndex = GetIndexOfInteractedBox(player.positionX, player.positionY);
            if (interactedBoxIndex >= 0)
            {
                MoveObject(ref box[interactedBoxIndex].positionX, ref box[interactedBoxIndex].positionY, player.playerMoveDir);

                if (CheckBoxMove() == false)
                {
                    OnObjectBlocked(() =>
                    {
                        PushOut(ref box[interactedBoxIndex].positionX, ref box[interactedBoxIndex].positionY);
                        PushOut(ref player.positionX, ref player.positionY);
                    });
                    return false;
                }
            }

            for (int i = 0; i < box.Length; i++)
            {
                for (int j = 0; j < goal.Length; j++)
                {
                    if (isObjectsCollide(box[i].positionX, box[i].positionY, goal[j].positionX, goal[j].positionY))
                    {
                        box[i].isOnGoal = true;
                    }
                    else
                    {
                        box[i].isOnGoal = false;
                    }
                }
            }

            return true;
        }

        void SetPlayerMoveDir(ConsoleKey key)
        {
            if (key == ConsoleKey.RightArrow)
            {
                player.playerMoveDir = Direction.Right;
            }
            else if (key == ConsoleKey.LeftArrow)
            {
                player.playerMoveDir = Direction.Left;
            }
            else if (key == ConsoleKey.UpArrow)
            {
                player.playerMoveDir = Direction.Up;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                player.playerMoveDir = Direction.Down;
            }
            else
            {
                player.playerMoveDir = Direction.Down;
            }
        }

        bool CheckPlayerMove()
        {
            if (IsObjectOutOfBound(player.positionX, player.positionY))
            {
                return false;
            }

            if (!player.isGhost)
            {
                for (int i = 0; i < block.Length; i++)
                {
                    if (isObjectsCollide(player.positionX, player.positionY, block[i].positionX, block[i].positionY))
                    {
                        return false;
                    }
                }

                for (int i = 0; i < hiddenBlock.Length; i++)
                {
                    if (hiddenBlock[i].isHidden)
                    {
                        continue;
                    }
                    if (isObjectsCollide(player.positionX, player.positionY, hiddenBlock[i].positionX, hiddenBlock[i].positionY))
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        int GetIndexOfInteractedBox(int interectObjectX, int interectObjectY)
        {
            for (int i = 0; i < box.Length; i++)
            {
                if (isObjectsCollide(interectObjectX, interectObjectY, box[i].positionX, box[i].positionY))
                {
                    return i;
                }
            }
            return -1;
        }


        bool CheckBoxMove()
        {
            if (IsObjectOutOfBound(box[interactedBoxIndex].positionX, box[interactedBoxIndex].positionY))
            {
                return false;
            }

            for (int i = 0; i < block.Length; i++)
            {
                if (isObjectsCollide(box[interactedBoxIndex].positionX, box[interactedBoxIndex].positionY, block[i].positionX, block[i].positionY))
                {
                    return false;
                }
            }

            for (int i = 0; i < box.Length; i++)
            {
                if (interactedBoxIndex == i)
                {
                    continue;
                }

                if (isObjectsCollide(box[interactedBoxIndex].positionX, box[interactedBoxIndex].positionY, box[i].positionX, box[i].positionY))
                {
                    return false;
                }
            }

            for (int i = 0; i < box.Length; i++)
            {
                if (hiddenBlock[i].isHidden)
                {
                    continue;
                }

                if (isObjectsCollide(box[interactedBoxIndex].positionX, box[interactedBoxIndex].positionY, hiddenBlock[i].positionX, hiddenBlock[i].positionY))
                {
                    return false;
                }
            }

            return true;
        }

        void CheckPlayerGetItem()
        {
            if (!player.isGhost)
            {
                if (isObjectsCollide(player.positionX, player.positionY, item.positionX, item.positionY))
                {
                    player.isGhost = true;
                }
            }
        }

        bool IsAllBoxesOnTheGoals()
        {
            int countOfBoxOnTheGoal = 0;

            for (int i = 0; i < goal.Length; i++)
            {
                for (int j = 0; j < box.Length; j++)
                {
                    if (isObjectsCollide(goal[i].positionX, goal[i].positionY, box[j].positionX, box[j].positionY))
                    {
                        countOfBoxOnTheGoal++;
                    }
                }
            }
            if (countOfBoxOnTheGoal == goal.Length)
            {
                return true;
            }
            else
            {
                while(true)
                {
                    if(TryMakeRandomBlock(countOfBoxOnTheGoal))
                    {
                        break;
                    }
                }
                return false;
            }
        }

        bool TryMakeRandomBlock(int boxOnGoalCount)
        {
            int randNum;
            if (boxOnGoalCount > appearedHiddenBlockCount)
            {
                Random random = new Random();
                randNum = random.Next(10);

                if (isObjectsCollide(hiddenBlock[randNum].positionX, hiddenBlock[randNum].positionY, player.positionX, player.positionY))
                {
                    return false;
                }
                for (int i = 0; i < box.Length; i++)
                {
                    if (isObjectsCollide(hiddenBlock[randNum].positionX, hiddenBlock[randNum].positionY, block[i].positionX, block[i].positionY))
                    {
                        return false;
                    }
                }

                hiddenBlock[randNum].isHidden = false;
                appearedHiddenBlockCount++;
            }
            
            return true;
        }


        void OnObjectBlocked(Action action)
        {
            action.Invoke();
        }

        void PushOut(ref int x, ref int y)
        {
            Direction pushOutDirection;

            switch (player.playerMoveDir)
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

            MoveObject(ref x, ref y, pushOutDirection);
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

        void MoveObject(ref int x, ref int y, Direction dir)
        {
            if (dir == Direction.Up)
            {
                y = y - 1;
            }
            else if (dir == Direction.Down)
            {
                y = y + 1;
            }
            else if (dir == Direction.Left)
            {
                x = x - 1;
            }
            else if (dir == Direction.Right)
            {
                x = x + 1;
            }
        }
    }
}