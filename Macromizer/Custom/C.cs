using System;

namespace Macromizer.Custom
{
    public static class C
    {
        public static void ClearLine(int iLineNumber)
        {
            int prevLeft = Console.CursorLeft;
            int prevTop = Console.CursorTop;
            Console.SetCursorPosition(0, iLineNumber);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(prevLeft, prevTop);
        }

        public static void Down(int i = 1)
        {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop + i);
        }

        public static void NextLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop + 1);
        }

        public static void Up(int i = 1)
        {
            if (Console.CursorTop > 0)
            {
                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - i);
            }
        }

        public static void Left(int i = 1)
        {
            if (Console.CursorLeft > 0)
            {
                Console.SetCursorPosition(Console.CursorLeft - i, Console.CursorTop);
            }
        }

        public static void Right(int i = 1)
        {
            Console.SetCursorPosition(Console.CursorLeft + i, Console.CursorTop);
        }
        
        public static void SetTemporaryColor(ConsoleColor iColor, Func<bool> Action)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = iColor;
            Action();
            Console.ForegroundColor = prevColor;
        }
    }
}
