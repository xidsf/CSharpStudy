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
        int playerPositionX = 1; //Player관련 변수
        int playerPositionY = 1;
        int playerMoveDistance = 1;
        bool isPlayerGhost = false;

        int[] boxPositionX = { 3, 5, 2, 4, 8 }; //Box 위치 변수
        int[] boxPositionY = { 4, 6, 2, 10, 8 };


        int[] blockPositionX = { 4, 5, 6, 9, 9 }; //Block위치 변수
        int[] blockPositionY = { 4, 4, 5, 7, 8 };

        int[] goalPositionX = { 10, 10, 10, 10, 10 }; //Goal 변수
        int[] goalPositionY = { 10, 11, 12, 13, 14 };

        int itemPositionX = 15; //Item 위치 변수
        int itemPositionY = 4;

        ConsoleKeyInfo inputKeyInfo;
        ConsoleKey key;

        const int BOX_IS_NOT_INTERACTED = -1000;
        const int BOX_IS_BLOCKED_BY_SOMETHING = -2000; //박스 관련 상수

        int tempPlayerX = playerPositionX; //플레이어 움직임 임시 변수
        int tempPlayerY = playerPositionY;
        Direction playerMoveDir = Direction.Right;

        int InteractedBoxIndex; //박스 관련 임시변수
        int tempBoxPositionX;
        int tempBoxPositionY;


        //화면 세팅 초기화
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
            isBoxCanMove = MoveBoxPosition(out InteractedBoxIndex, out tempBoxPositionX, out tempBoxPositionY);

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
            void DrawObject(int x, int y, string icon)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(icon);
            }

            //콘솔창 클리어
            Console.Clear();

            DrawObject(playerPositionX, playerPositionY, "▼");//플레이어 그리기

            //블록(고스트로 통과된 상태면 다른 아이콘으로 출력)
            int blockCount = blockPositionX.Length;
            for (int i = 0; i < blockCount; i++)
            {
                if (playerPositionX == blockPositionX[i] && playerPositionY == blockPositionY[i])
                {
                    DrawObject(blockPositionX[i], blockPositionY[i], "▣");
                }
                else
                {
                    DrawObject(blockPositionX[i], blockPositionY[i], "□");
                }
            }

            //아이템
            if (!isPlayerGhost)
            {
                DrawObject(itemPositionX, itemPositionY, "♨");
            }

            //골
            int goalCount = goalPositionX.Length;
            for (int i = 0; i < goalCount; i++)
            {
                DrawObject(goalPositionX[i], goalPositionY[i], "☆");
            }

            //박스
            int boxCount = boxPositionX.Length;
            for (int i = 0; i < boxCount; i++)
            {
                for (int j = 0; j < boxCount; j++)
                {
                    if (boxPositionX[i] == goalPositionX[j] && boxPositionY[i] == goalPositionY[j])
                    {
                        DrawObject(boxPositionX[i], boxPositionY[i], "★");
                        break;
                    }
                    else
                    {
                        DrawObject(boxPositionX[i], boxPositionY[i], "◇");
                    }
                }

            }
        }

        void Input()
        {
            inputKeyInfo = Console.ReadKey(); //사용자의 입력 받기
            key = inputKeyInfo.Key;
        }


        //오브젝트가 범위 밖으로 나는지 확인하는 함수
        bool IsObjectOutOfBound(int objectPositionX, int objectPositionY)
            => (objectPositionX < 0 || objectPositionY < 0) ? true : false;

        //오브젝트가 다른 오브젝트와 충돌하는지 확인하는 함수
        bool isObjectsCollide(int ObjectPositionX1, int ObjectPositionY1, int ObjectPositionX2, int ObjectPositionY2)
            => (ObjectPositionX1 == ObjectPositionX2 && ObjectPositionY1 == ObjectPositionY2) ? true : false;

        //오브젝트가 블럭과 충돌하는지 확인하는 함수
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

        //오브젝트 이동
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




        //플레이어 위치를 이동시켜서 유효한 움직임이면 true반환
        bool MovePlayerPosition()
        {
            tempPlayerX = playerPositionX;
            tempPlayerY = playerPositionY;
            if (inputKeyInfo.Key == ConsoleKey.RightArrow)//사용자의 입력에 따른 플레이어 이동 계산
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

        //박스의 움직임에 따라서 움직인 박스의 인덱스, BOX_IS_NOT_INTERACTED, BOX_IS_BLOCKED_BY_SOMETHING 반환
        int MoveBoxPosition(out int InteractedBoxIndex, out int tempBoxPositionX, out int tempBoxPositionY)
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

            //임시 박스 이동
            MoveObject(ref tempBoxPositionX, ref tempBoxPositionY, playerMoveDir, playerMoveDistance);

            //임시 박스가 막혀서 움직이지 않으면 BOX_IS_BLOCKED_BY_SOMETHING 반환
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
