class TestCode
{
    static void Main()
    {
        int someValue = 10;

        Action someDelegate = () =>
        {
            // 현재 범위 내에 someValue라는 이름을 찾을 수 없다
            // 범위 밖의 someValue를 캡쳐해 내부적으로 someValue의 주소 값(ref) 사용한다
            Console.WriteLine($"Outer Variable : {someValue}");
        };

        someDelegate();
    }

}
