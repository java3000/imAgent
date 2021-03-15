using System;

namespace ImAgent.Helpers
{
    public static class Helper
    {
        public static void PrintConsoleMessage(MessageType type, params string[] text)
        {
            Console.ResetColor();

            switch(type)
            {
                case MessageType.ERROR: Console.ForegroundColor = ConsoleColor.Red; break;
                case MessageType.INFO: Console.ForegroundColor = ConsoleColor.Blue; break;
                case MessageType.SUCCESS: Console.ForegroundColor = ConsoleColor.Green; break;
                case MessageType.WARNING: Console.ForegroundColor = ConsoleColor.Yellow; break;
            }

            foreach (var item in text)
            {
                Console.WriteLine(item); 
            }

            Console.ResetColor();
        }
    }
}
