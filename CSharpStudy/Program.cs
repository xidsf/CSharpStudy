using System.Numerics;
using System.Reflection.Metadata.Ecma335;

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
        bool isPlayerGhost = false;

        int[] boxPositionX = { 3, 5, 2, 4, 8 }; 
        int[] boxPositionY = { 4, 6, 2, 10, 8 };


        int[] blockPositionX = { 4, 5, 6, 9, 9 }; 
        int[] blockPositionY = { 4, 4, 5, 7, 8 };

        int[] goalPositionX = { 10, 10, 10, 10, 10 }; 
        int[] goalPositionY = { 10, 11, 12, 13, 14 };

        int itemPositionX = 15; //Item 위치 변수
        int itemPositionY = 4;

        ConsoleKeyInfo inputKeyInfo;
        ConsoleKey key;

        const int BOX_IS_NOT_INTERACTED = -1000;
        const int BOX_IS_BLOCKED_BY_SOMETHING = -2000; 

        int tempPlayerX = playerPositionX; 
        int tempPlayerY = playerPositionY;
        Direction playerMoveDir = Direction.Right;

        int InteractedBoxIndex; 
        int tempBoxPositionX;
        int tempBoxPositionY;


        
        Console.ResetColor();
        Console.CursorVisible = false;
        Console.Title = "Sokoban";
        Console.BackgroundColor = ConsoleColor.Gray;
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Clear();

        isPlayerGhost = false;

        bool isPlayerCanMove;
        int isBoxCanMove;

        while (true)
        {
            Render();
            Input();

            isPlayerCanMove = MovePlayerPosition();
            isBoxCanMove = MoveBoxPosition();

            if (isBoxCanMove == BOX_IS_BLOCKED_BY_SOMETHING || isPlayerCanMove == false) { continue; }

            if (isPlayerCanMove && isBoxCanMove >= 0)
            {
                playerPositionX = tempPlayerX;
                playerPositionY = tempPlayerY;

                boxPositionX[InteractedBoxIndex] = tempBoxPositionX;
                boxPositionY[InteractedBoxIndex] = tempBoxPositionY;
            }
            else if (isPlayerCanMove && isBoxCanMove == BOX_IS_NOT_INTERACTED)
            {
                playerPositionX = tempPlayerX;
                playerPositionY = tempPlayerY;
            }

            CheckPlayerGetItem();

            if(IsBoxesOnTheGoals())
            {
                break;
            }
        }

        

        void Render()
        {
            string playerIcon = "▼";
            string blockIcon_Idle = "□";
            string blockIcon_WithPlayer = "▣";
            string itemIcon = "♨";
            string goalIcon_Idle = "☆";
            string goalIcon_WithBox = "★";
            string boxIcon = "◇";


            void DrawObject(int x, int y, string icon)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(icon);
            }

            
            Console.Clear();

            DrawObject(playerPositionX, playerPositionY, playerIcon);

            
            int blockCount = blockPositionX.Length;
            for (int i = 0; i < blockCount; i++)
            {
                if (playerPositionX == blockPositionX[i] && playerPositionY == blockPositionY[i])
                {
                    DrawObject(blockPositionX[i], blockPositionY[i], blockIcon_WithPlayer);
                }
                else
                {
                    DrawObject(blockPositionX[i], blockPositionY[i], blockIcon_Idle);
                }
            }

            
            if (!isPlayerGhost)
            {
                DrawObject(itemPositionX, itemPositionY, itemIcon);
            }

           
            int goalCount = goalPositionX.Length;
            for (int i = 0; i < goalCount; i++)
            {
                DrawObject(goalPositionX[i], goalPositionY[i], goalIcon_Idle);
            }

            
            int boxCount = boxPositionX.Length;
            for (int i = 0; i < boxCount; i++)
            {
                for (int j = 0; j < boxCount; j++)
                {
                    if (boxPositionX[i] == goalPositionX[j] && boxPositionY[i] == goalPositionY[j])
                    {
                        DrawObject(boxPositionX[i], boxPositionY[i], goalIcon_WithBox);
                        break;
                    }
                    else
                    {
                        DrawObject(boxPositionX[i], boxPositionY[i], boxIcon);
                    }
                }

            }
        }

        void Input()
        {
            inputKeyInfo = Console.ReadKey();
            key = inputKeyInfo.Key;
        }


        
        bool IsObjectOutOfBound(int objectPositionX, int objectPositionY)
            => (objectPositionX < 0 || objectPositionY < 0) ? true : false;

        
        bool isObjectsCollide(int ObjectPositionX1, int ObjectPositionY1, int ObjectPositionX2, int ObjectPositionY2)
            => (ObjectPositionX1 == ObjectPositionX2 && ObjectPositionY1 == ObjectPositionY2) ? true : false;

        
        bool isObjectCollideBlock(int objectPositionX, int objectPositionY)
        {
            int blockCount = blockPositionX.Length;
            for (int i = 0; i < blockCount; i++)
            {
                if (isObjectsCollide(objectPositionX, objectPositionY, blockPositionX[i], blockPositionY[i]))
                {
                    return true;
                }
            }
            return false;
        }

        
        void MoveObject(ref int objectX, ref int objectY, Direction dir, int distance)
        {
            if (dir == Direction.Up)
            {
                objectY = objectY - distance;
            }
            else if (dir == Direction.Down)
            {
                objectY = objectY + distance;
            }
            else if (dir == Direction.Left)
            {
                objectX = objectX - distance;
            }
            else if (dir == Direction.Right)
            {
                objectX = objectX + distance;
            }
        }

        
        
        bool MovePlayerPosition()
        {
            tempPlayerX = playerPositionX;
            tempPlayerY = playerPositionY;
            if (inputKeyInfo.Key == ConsoleKey.RightArrow)
            {
                playerMoveDir = Direction.Right;
            }
            else if (inputKeyInfo.Key == ConsoleKey.LeftArrow)
            {
                playerMoveDir = Direction.Left;
            }
            else if (inputKeyInfo.Key == ConsoleKey.UpArrow)
            {
                playerMoveDir = Direction.Up;
            }
            else if (inputKeyInfo.Key == ConsoleKey.DownArrow)
            {
                playerMoveDir = Direction.Down;
            }

            MoveObject(ref tempPlayerX, ref tempPlayerY, playerMoveDir, playerMoveDistance);

            if (IsObjectOutOfBound(tempPlayerX, tempPlayerY))
            {
                return false;
            }

            if (!isPlayerGhost)
            {
                if (isObjectCollideBlock(tempPlayerX, tempPlayerY))
                {
                    return false;
                }
            }
            return true;
        }

        
        int MoveBoxPosition()
        {
            InteractedBoxIndex = BOX_IS_NOT_INTERACTED;
            tempBoxPositionX = BOX_IS_NOT_INTERACTED;
            tempBoxPositionY = BOX_IS_NOT_INTERACTED;

            int boxCount = boxPositionX.Length;

            for (int i = 0; i < boxCount; i++)
            {
                if (isObjectsCollide(boxPositionX[i], boxPositionY[i], tempPlayerX, tempPlayerY))
                {
                    InteractedBoxIndex = i;
                    tempBoxPositionX = boxPositionX[i];
                    tempBoxPositionY = boxPositionY[i];
                    break;
                }
            }

            if (InteractedBoxIndex == BOX_IS_NOT_INTERACTED) { return BOX_IS_NOT_INTERACTED; }

            
            MoveObject(ref tempBoxPositionX, ref tempBoxPositionY, playerMoveDir, playerMoveDistance);

            
            if (IsObjectOutOfBound(tempBoxPositionX, tempBoxPositionY))
            {
                return BOX_IS_BLOCKED_BY_SOMETHING;
            }

            if (isObjectCollideBlock(tempBoxPositionX, tempBoxPositionY))
            {
                return BOX_IS_BLOCKED_BY_SOMETHING;
            }

            for (int i = 0; i < boxCount; i++)
            {
                if (InteractedBoxIndex == i) continue;

                if (isObjectsCollide(tempBoxPositionX, tempBoxPositionY, boxPositionX[i], boxPositionY[i]))
                {
                    return BOX_IS_BLOCKED_BY_SOMETHING;
                }
            }

            return InteractedBoxIndex;
        }

        void CheckPlayerGetItem()
        {
            if (!isPlayerGhost)
            {
                if (playerPositionX == itemPositionX && playerPositionY == itemPositionY)
                {
                    isPlayerGhost = true;
                }
            }
        }

        bool IsBoxesOnTheGoals()
        {
            int countOfBoxOnTheGoal = 0;

            int boxCount = boxPositionX.Length;
            int goalCount = goalPositionX.Length;

            for (int i = 0; i < goalCount; i++)
            {
                for (int j = 0; j < boxCount; j++)
                {
                    if (isObjectsCollide(goalPositionX[i], goalPositionY[i], boxPositionX[j], boxPositionY[j]))
                    {
                        countOfBoxOnTheGoal++;
                    }
                }
            }
            if (countOfBoxOnTheGoal == goalCount) { return true; }
            else { return false; }
        }

    }
}
