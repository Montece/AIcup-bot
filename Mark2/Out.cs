using System;

namespace AICUP.Mark2
{
    public static class Out
    {
        public static void Print(string text)
        {
            Console.WriteLine("[" + DateTime.Now.ToShortTimeString() + "]: " + text);
        }

        public static void Print(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Print(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
