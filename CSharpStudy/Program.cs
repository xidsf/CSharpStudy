using System.ComponentModel.Design;

class Program
{
    
    static void Main(string[] args)
    {
        Player player = new Player(1, 1);

        Box[] box = new Box[5];
        box[0] = new Box(3, 5);
        box[1] = new Box(5, 6);
        box[2] = new Box(2, 2);
        box[3] = new Box(4, 10);
        box[4] = new Box(8, 8);

        Block[] block = new Block[5];
        block[0] = new Block(4, 4);
        block[1] = new Block(5, 4);
        block[2] = new Block(6, 5);
        block[3] = new Block(9, 7);
        block[4] = new Block(9, 8);

        Block[] hiddenBlock = new Block[10];
        for (int i = 0; i < 5; i++)
        {
            hiddenBlock[i] = new Block(9, 10 + i, true);
        }
        for (int i = 5; i < 10; i++)
        {
            hiddenBlock[i] = new Block(11, 10 + i - 5, true);
        }
        int appearedHiddenBlockCount = 0;

        Goal[] goal = new Goal[5];
        for (int i = 0; i < goal.Length; i++)
        {
            goal[i] = new Goal(10, 10 + i);
        }

        Item item = new Item(15, 4);


        InitGameState();

        while (true)
        {
            Render();

            ConsoleKey key = Input();

            player.Move(key);

            CollisionCheck();

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
        }

        void Render()
        {
            Console.Clear();
            DrawObject(player.positionX, player.positionY, player.playerIcon);

            for (int i = 0; i < block.Length; i++)
            {
                if (block[i].isInPlayer)
                {
                    DrawObject(block[i].positionX, block[i].positionY, block[i].playerInIcon);
                }
                else
                {
                    DrawObject(block[i].positionX, block[i].positionY, block[i].Icon);
                }
            }

            if (!player.isGhost)
            {
                DrawObject(item.positionX, item.positionY, item.itemIcon);
            }

            for (int i = 0; i < goal.Length; i++)
            {
                DrawObject(goal[i].positionX, goal[i].positionY, goal[i].Icon);
            }

            for (int i = 0; i < box.Length; i++)
            {
                for (int j = 0; j < goal.Length; j++)
                {
                    if (box[i].isOnGoal)
                    {
                        DrawObject(box[i].positionX, box[i].positionY, box[i].boxOnIcon);
                        break;
                    }
                    else
                    {
                        DrawObject(box[i].positionX, box[i].positionY, box[i].Icon);
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

        void CollisionCheck()
        {
            Action<Direction>? pushOut;
            int interactedBoxIndex;

            MovePushedBox(out interactedBoxIndex);
            pushOut = CalcBlockCollision(interactedBoxIndex);

            if (pushOut != null)
            {
                pushOut.Invoke(ObjectManager.GetReverseDir(player.playerMoveDir));
            }
        }

        void MovePushedBox(out int interactedBoxIndex)
        {
            for (int i = 0; i < box.Length; i++)
            {
                if (player.isCollide(box[i]))
                {
                    interactedBoxIndex = i;
                    return;
                }
            }
            interactedBoxIndex = -1;
        }

        Action<Direction>? CalcBlockCollision(int movedBoxIndex)
        {
            Action<Direction>? action = null;

            if (player.isGhost == false)
            {
                for (int i = 0; i < block.Length; i++)
                {
                    if (player.isCollide(block[i].positionX, block[i].positionY))
                    {
                        action = player.Move;
                        return action;
                    }
                }
            }

            if (movedBoxIndex != -1)
            {
                for (int j = 0; j < block.Length; j++)
                {
                    if (box[movedBoxIndex].isCollide(block[j].positionX, block[j].positionY))
                    {
                        action = player.Move;
                        action += box[movedBoxIndex].Move;
                        return action;
                    }
                }

                for (int i = 0; i < box.Length; i++)
                {
                    if (i == movedBoxIndex)
                    {
                        continue;
                    }

                    if (box[movedBoxIndex].isCollide(box[i].positionX, box[i].positionY))
                    {
                        action = player.Move;
                        action += box[movedBoxIndex].Move;
                        return action;
                    }
                }
            }

            return action;
        }

        bool IsAllBoxesOnTheGoals()
        {
            int countOfBoxOnTheGoal = 0;

            for (int i = 0; i < goal.Length; i++)
            {
                for (int j = 0; j < box.Length; j++)
                {
                    if (box[j].isCollide(goal[i].positionX, goal[i].positionY))
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

                if (player.isCollide(hiddenBlock[randNum].positionX, hiddenBlock[randNum].positionY))
                {
                    return false;
                }
                if (hiddenBlock[randNum].isHidden != true)
                {
                    return false;
                }

                hiddenBlock[randNum].isHidden = false;
                appearedHiddenBlockCount++;
            }
            
            return true;
        }
    }
}